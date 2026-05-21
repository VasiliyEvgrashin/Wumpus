namespace Wumpus.Agents
{
    public abstract class StrategyAgent : RiskAgent
    {
        protected StrategyAgent(World world, FolKnowledgeBase kb, int startX, int startY, int vision = 10)
            : base(world, kb, startX, startY, vision)
        {
        }

        protected (int x, int y)? ChooseNextMove()
        {
            var lightNeighbors = GetNeighbors(Position)
                .Where(n => HasFact("Light", n))
                .ToList();

            if (lightNeighbors.Any())
                return lightNeighbors.First();

            var safeUnvisitedNeighbors = GetSafeUnvisitedCells()
                .Where(c => IsNeighbor(Position, c))
                .ToList();

            if (safeUnvisitedNeighbors.Any())
                return ChooseBestSafeCell(safeUnvisitedNeighbors);

            var allUnvisitedSafe = GetSafeUnvisitedCells();
            if (allUnvisitedSafe.Any())
            {
                var target = allUnvisitedSafe
                    .OrderBy(p => Manhattan(Position, p))
                    .First();

                var nextStep = FindStepToTarget(target);
                if (nextStep != null)
                    return nextStep;
            }

            var risky = GetRiskAssessment();
            if (risky.Any())
                return risky.OrderBy(r => r.risk).First().pos;

            return null;
        }

        private (int x, int y)? FindStepToTarget((int x, int y) target)
        {
            var queue = new Queue<(int x, int y)>();
            var cameFrom = new Dictionary<(int, int), (int, int)>();
            var visited = new HashSet<(int, int)>();

            queue.Enqueue(Position);
            visited.Add(Position);

            bool reached = false;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == target)
                {
                    reached = true;
                    break;
                }

                foreach (var n in GetNeighbors(current))
                {
                    if (!HasFact("Safe", n)) continue;
                    if (visited.Contains(n)) continue;

                    visited.Add(n);
                    cameFrom[n] = current;
                    queue.Enqueue(n);
                }
            }

            if (!reached)
                return null;

            var step = target;
            while (cameFrom.TryGetValue(step, out var prev) && prev != Position)
                step = prev;

            return step;
        }

        private List<(int x, int y)> GetSafeUnvisitedCells()
        {
            return kb.Facts
                .Where(f => f.Name == "Safe")
                .Select(f => (int.Parse(f.Args[0]), int.Parse(f.Args[1])))
                .Where(p => !Visited.Contains(p))
                .ToList();
        }

        private (int x, int y) ChooseBestSafeCell(List<(int x, int y)> safe)
        {
            return safe
                .OrderByDescending(p => CountUnknownNeighbors(p))
                .ThenBy(p => HasFact("PossiblePit", p) ? 1 : 0)
                .ThenBy(p => HasFact("PossibleWumpus", p) ? 1 : 0)
                .ThenBy(p => Manhattan(Position, p))
                .First();
        }

        private int CountUnknownNeighbors((int x, int y) p)
            => GetNeighbors(p).Count(n => !Visited.Contains(n));
    }
}