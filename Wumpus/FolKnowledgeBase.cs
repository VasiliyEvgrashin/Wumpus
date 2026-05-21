namespace Wumpus
{
    public class FolKnowledgeBase
    {
        private readonly List<Predicate> facts = new();
        private readonly List<FolRule> rules = new();

        public IEnumerable<Predicate> Facts => facts;

        public void TellFact(Predicate fact)
        {
            if (!facts.Any(f => f.ToString() == fact.ToString()))
                facts.Add(fact);
        }

        public void TellRule(FolRule rule)
        {
            rules.Add(rule);
        }

        public bool Ask(Predicate query)
        {
            return facts.Any(f => f.ToString() == query.ToString());
        }

        public void RemoveFactsByName(string name)
        {
            facts.RemoveAll(f => f.Name == name);
        }

        // ============================================================
        //  PUBLIC INFER
        // ============================================================
        public void Infer()
        {
            bool added;

            do
            {
                added = false;

                foreach (var rule in rules)
                {
                    var substitutions = UnifyPremises(rule);

                    if (substitutions.Count > 0)
                    {
                        if (ApplyConclusion(rule, substitutions))
                            added = true;
                    }
                }

            } while (added);
        }

        // ============================================================
        //  UNIFY ALL PREMISES OF A RULE
        // ============================================================
        private List<Dictionary<string, string>> UnifyPremises(FolRule rule)
        {
            var substitutions = InitialSubstitutions();

            foreach (var premise in rule.Premises)
            {
                substitutions = UnifyPremiseWithFacts(premise, substitutions);

                if (substitutions.Count == 0)
                    break;
            }

            return substitutions;
        }

        // ============================================================
        //  UNIFY ONE PREMISE AGAINST ALL FACTS
        // ============================================================
        private List<Dictionary<string, string>> UnifyPremiseWithFacts(
            Predicate premise,
            List<Dictionary<string, string>> current)
        {
            var result = new List<Dictionary<string, string>>();

            foreach (var subst in current)
            {
                var appliedPremise = ApplySubst(premise, subst);

                foreach (var fact in facts)
                {
                    var unified = Unifier.Unify(
                        appliedPremise,
                        fact,
                        new Dictionary<string, string>(subst));

                    if (unified != null)
                        result.Add(unified);
                }
            }

            return result;
        }

        // ============================================================
        //  APPLY RULE CONCLUSION
        // ============================================================
        private bool ApplyConclusion(FolRule rule, List<Dictionary<string, string>> substitutions)
        {
            bool added = false;

            foreach (var subst in substitutions)
            {
                var concl = ApplySubst(rule.Conclusion, subst);

                if (!facts.Any(f => f.ToString() == concl.ToString()))
                {
                    facts.Add(concl);
                    added = true;
                }
            }

            return added;
        }

        // ============================================================
        //  APPLY SUBSTITUTION TO PREDICATE
        // ============================================================
        private static Predicate ApplySubst(Predicate p, Dictionary<string, string> subst)
        {
            var newArgs = p.Args
                .Select(a => subst.TryGetValue(a, out var v) ? v : a)
                .ToArray();

            return new Predicate(p.Name, newArgs);
        }

        // ============================================================
        //  INITIAL SUBSTITUTION LIST
        // ============================================================
        private List<Dictionary<string, string>> InitialSubstitutions()
        {
            return new List<Dictionary<string, string>>
        {
            new Dictionary<string, string>()
        };
        }

        public void RemoveAllFactsExceptSafeAndVisited(int x, int y)
        {
            string xs = x.ToString();
            string ys = y.ToString();

            facts.RemoveAll(f =>
                f.Args.Count == 2 &&
                f.Args[0] == xs &&
                f.Args[1] == ys &&
                f.Name != "Safe" &&
                f.Name != "Visited"
            );
        }

        public void GlobalCleanup()
        {
            var safeCells = facts
                .Where(f => f.Name == "Safe" && f.Args.Count == 2)
                .Select(f => (x: f.Args[0], y: f.Args[1]))
                .Distinct()
                .ToList();

            if (safeCells.Count == 0)
                return;

            foreach (var cell in safeCells)
            {
                facts.RemoveAll(f =>
                    f.Args.Count == 2 &&
                    f.Args[0] == cell.x &&
                    f.Args[1] == cell.y &&
                    f.Name != "Safe" &&
                    f.Name != "Visited"
                );
            }

            CleanupImpossiblePossibles();
        }

        private void CleanupImpossiblePossibles()
        {
            var notPit = new HashSet<(string x, string y)>(
                facts.Where(f => f.Name == "NotPit" && f.Args.Count == 2)
                     .Select(f => (f.Args[0], f.Args[1])));

            var notWumpus = new HashSet<(string x, string y)>(
                facts.Where(f => f.Name == "NotWumpus" && f.Args.Count == 2)
                     .Select(f => (f.Args[0], f.Args[1])));

            facts.RemoveAll(f =>
                f.Name == "PossiblePit" &&
                notPit.Contains((f.Args[0], f.Args[1])));

            facts.RemoveAll(f =>
                f.Name == "PossibleWumpus" &&
                notWumpus.Contains((f.Args[0], f.Args[1])));
        }

        public void CleanupFarCells((int x, int y) agentPos, int maxDistance)
        {
            string ax = agentPos.x.ToString();
            string ay = agentPos.y.ToString();

            facts.RemoveAll(f =>
                f.Args.Count == 2 &&
                f.Name != "Safe" && f.Name != "Visited" &&
                (Math.Abs(int.Parse(f.Args[0]) - agentPos.x) +
                 Math.Abs(int.Parse(f.Args[1]) - agentPos.y)) > maxDistance
            );

            facts.RemoveAll(f =>
                f.Args.Count == 2 &&
                (f.Name == "Safe" || f.Name == "Visited") &&
                (Math.Abs(int.Parse(f.Args[0]) - agentPos.x) +
                 Math.Abs(int.Parse(f.Args[1]) - agentPos.y)) > maxDistance
            );
        }

        public double EstimateProbability(Predicate query)
        {
            double score = 0.0;

            foreach (var rule in rules)
            {
                if (rule.Conclusion.Name != query.Name)
                    continue;

                if (rule.Conclusion.Args.Count != query.Args.Count)
                    continue;

                bool match = true;
                for (int i = 0; i < query.Args.Count; i++)
                {
                    if (rule.Conclusion.Args[i] != query.Args[i] &&
                        !rule.Conclusion.Args[i].StartsWith("x") &&
                        !rule.Conclusion.Args[i].StartsWith("y"))
                    {
                        match = false;
                        break;
                    }
                }

                if (!match)
                    continue;

                bool premisesHold = rule.Premises.All(p => Facts.Any(f => f.ToString() == p.ToString()));

                if (premisesHold)
                    score += rule.Weight;
            }

            return 1.0 / (1.0 + Math.Exp(-score));
        }
    }
}
