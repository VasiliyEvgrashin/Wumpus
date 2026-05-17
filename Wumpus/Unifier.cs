namespace Wumpus
{
    public static class Unifier
    {
        public static Dictionary<string, string>? Unify(
            Predicate pattern,
            Predicate fact,
            Dictionary<string, string>? subst = null)
        {
            if (pattern.Name != fact.Name || pattern.Args.Count != fact.Args.Count)
                return null;

            subst ??= new Dictionary<string, string>();

            for (int i = 0; i < pattern.Args.Count; i++)
            {
                var p = pattern.Args[i];
                var f = fact.Args[i];

                if (IsVariable(p))
                {
                    if (subst.TryGetValue(p, out var existing))
                    {
                        if (existing != f)
                            return null;
                    }
                    else
                    {
                        subst[p] = f;
                    }
                }
                else
                {
                    if (p != f)
                        return null;
                }
            }

            return new Dictionary<string, string>(subst);
        }

        private static bool IsVariable(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            return char.IsLower(s[0]);
        }
    }

}
