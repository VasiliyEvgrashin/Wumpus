namespace Wumpus.Agents
{
    public class Agent : FogAgent
    {
        public HashSet<(int x, int y)> Visited { get; } = new();
        private int stepCounter = 0;

        public Agent(World world, FolKnowledgeBase kb, int startX, int startY)
            : base(world, kb, startX, startY)
        {
            Visited.Add(Position);
            TellVisited(Position);
        }

        // ---------------------------------------------------------
        // SENSORS
        // ---------------------------------------------------------
        public void Perceive()
        {
            var (x, y) = Position;
            var breeze = world.HasBreeze(x, y);
            var stench = world.HasStench(x, y);
            var light = world.HasLight(x, y);

            kb.TellFact(new Predicate(breeze ? "Breeze" : "NoBreeze", x.ToString(), y.ToString()));
            kb.TellFact(new Predicate(stench ? "Stench" : "NoStench", x.ToString(), y.ToString()));
            kb.TellFact(new Predicate(light ? "Light" : "NoLight", x.ToString(), y.ToString()));
        }

        // ---------------------------------------------------------
        // ONE STEP
        // ---------------------------------------------------------
        public bool Step()
        {
            var next = ChooseNextMove();
            if (next == null)
                return false;

            MoveTo(next.Value);

            Perceive();
            kb.Infer();

            if (HasFact("Safe", next.Value))
                kb.RemoveAllFactsExceptSafeAndVisited(next.Value.x, next.Value.y);

            stepCounter++;
            if (stepCounter % 10 == 0)
            {
                int maxDist = visionRadius * 2;
                kb.CleanupFarCells(Position, maxDist);
            }

            DrawWorld();

            if (world.Map[next.Value.x, next.Value.y] == CellContent.Exit)
            {
                Console.WriteLine("Agent reached EXIT");
                return false;
            }

            return true;
        }

        private bool IsNeighbor((int x, int y) a, (int x, int y) b)
            => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) == 1;

        // ---------------------------------------------------------
        // SELECTING THE NEXT MOVE (with BFS over SAFE)
        // ---------------------------------------------------------
        private (int x, int y)? ChooseNextMove()
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

        // ---------------------------------------------------------
        // BFS over SAFE cells to the target, returning the NEXT step
        // ---------------------------------------------------------
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

        // ---------------------------------------------------------
        // SAFE
        // ---------------------------------------------------------
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

        // ---------------------------------------------------------
        // RISK
        // ---------------------------------------------------------
        private List<((int x, int y) pos, double risk)> GetRiskAssessment()
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

        private List<(int x, int y)> GetUnknownNeighbors()
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

        private double EstimatePitRisk((int x, int y) cell)
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

        private double EstimateWumpusRisk((int x, int y) cell)
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

        // ---------------------------------------------------------
        // UTILITIES
        // ---------------------------------------------------------
        private bool HasFact(string name, (int x, int y) pos)
        {
            return kb.Facts.Any(f =>
                f.Name == name &&
                f.Args[0] == pos.x.ToString() &&
                f.Args[1] == pos.y.ToString());
        }

        private IEnumerable<(int x, int y)> GetNeighbors((int x, int y) p)
        {
            var dirs = new (int dx, int dy)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };

            foreach (var d in dirs)
            {
                int nx = p.x + d.dx;
                int ny = p.y + d.dy;

                if (nx >= 0 && nx < world.Size && ny >= 0 && ny < world.Size)
                    yield return (nx, ny);
            }
        }

        private int Manhattan((int x, int y) a, (int x, int y) b)
            => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);

        private int CountUnknownNeighbors((int x, int y) p)
            => GetNeighbors(p).Count(n => !Visited.Contains(n));

        protected new void MoveTo((int x, int y) next)
        {
            base.MoveTo(next);

            Visited.Add(next);
            TellVisited(next);

            if (HasFact("Safe", next))
                kb.RemoveAllFactsExceptSafeAndVisited(next.x, next.y);
        }

        private void TellVisited((int x, int y) pos)
        {
            kb.TellFact(new Predicate("Visited", pos.x.ToString(), pos.y.ToString()));
        }
    }
}