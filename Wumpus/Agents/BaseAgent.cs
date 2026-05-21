namespace Wumpus.Agents
{
    public abstract class BaseAgent
    {
        protected readonly World world;
        protected readonly FolKnowledgeBase kb;

        protected readonly int visionRadius;
        public (int x, int y) Position { get; protected set; }

        protected HashSet<(int x, int y)> visited = new();
        public IReadOnlyCollection<(int x, int y)> Visited => visited;

        protected BaseAgent(World world, FolKnowledgeBase kb, int startX, int startY, int vision = 10)
        {
            this.world = world;
            this.kb = kb;
            visionRadius = vision;
            Position = (startX, startY);
            visited.Add(Position);
        }

        protected bool InVision((int x, int y) c, (int x, int y) p)
            => Math.Abs(c.x - p.x) + Math.Abs(c.y - p.y) <= visionRadius;

        protected bool HasFact(string name, (int x, int y) pos)
        {
            var sx = pos.x.ToString();
            var sy = pos.y.ToString();
            return kb.Facts.Any(f =>
                f.Name == name &&
                f.Args[0] == sx &&
                f.Args[1] == sy);
        }

        protected IEnumerable<(int x, int y)> GetNeighbors((int x, int y) p)
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

        protected int Manhattan((int x, int y) a, (int x, int y) b)
            => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);

        protected bool IsNeighbor((int x, int y) a, (int x, int y) b)
            => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) == 1;

        protected bool IsInside(int x, int y)
            => x >= 0 && x < world.Size && y >= 0 && y < world.Size;

        protected virtual void MoveTo((int x, int y) next)
        {
            Position = next;
            visited.Add(next);
        }
    }
}