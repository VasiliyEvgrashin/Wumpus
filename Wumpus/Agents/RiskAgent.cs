namespace Wumpus.Agents
{
    public class RiskAgent : FogAgent
    {
        public RiskAgent(World world, FolKnowledgeBase kb, int startX, int startY)
            : base(world, kb, startX, startY)
        {
        }

        protected List<((int x, int y) pos, double risk)> GetRiskAssessment()
        {
            var unknown = GetUnknownNeighbors();
            var result = new List<((int, int), double)>();

            foreach (var cell in unknown)
            {
                double pitRisk = EstimatePitRisk(cell);
                double wumpusRisk = EstimateWumpusRisk(cell);
                result.Add((cell, pitRisk + wumpusRisk));
            }

            return result;
        }

        protected List<(int x, int y)> GetUnknownNeighbors()
        {
            var result = new HashSet<(int, int)>();

            foreach (var v in Visited)
            {
                foreach (var n in GetNeighbors(v))
                {
                    if (Visited.Contains(n))
                        continue;

                    bool known =
                        HasFact("Safe", n) ||
                        HasFact("Pit", n) ||
                        HasFact("Wumpus", n) ||
                        HasFact("NotPit", n) ||
                        HasFact("NotWumpus", n) ||
                        HasFact("PossiblePit", n) ||
                        HasFact("PossibleWumpus", n) ||
                        HasFact("Light", n) ||
                        HasFact("Visited", n);

                    if (!known)
                        result.Add(n);
                }
            }

            return result.ToList();
        }

        protected double EstimatePitRisk((int x, int y) cell)
        {
            if (HasFact("Pit", cell)) return 1.0;
            if (HasFact("NotPit", cell)) return 0.0;

            var pPit = kb.EstimateProbability(
                new Predicate("Pit", cell.x.ToString(), cell.y.ToString()));

            var pPossiblePit = kb.EstimateProbability(
                new Predicate("PossiblePit", cell.x.ToString(), cell.y.ToString()));

            double p = Math.Max(pPit, 0.8 * pPossiblePit);

            if (p == 0.0)
            {
                bool nearBreeze = GetNeighbors(cell).Any(n => HasFact("Breeze", n));
                p = nearBreeze ? 0.5 : 0.1;
            }

            return Math.Clamp(p, 0.0, 1.0);
        }

        protected double EstimateWumpusRisk((int x, int y) cell)
        {
            if (HasFact("Wumpus", cell)) return 1.0;
            if (HasFact("NotWumpus", cell)) return 0.0;

            var pW = kb.EstimateProbability(
                new Predicate("Wumpus", cell.x.ToString(), cell.y.ToString()));

            var pPossibleW = kb.EstimateProbability(
                new Predicate("PossibleWumpus", cell.x.ToString(), cell.y.ToString()));

            double p = Math.Max(pW, 0.8 * pPossibleW);

            if (p == 0.0)
            {
                bool nearStench = GetNeighbors(cell).Any(n => HasFact("Stench", n));
                p = nearStench ? 0.5 : 0.05;
            }

            return Math.Clamp(p, 0.0, 1.0);
        }
    }
}
