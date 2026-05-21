namespace Wumpus.Agents
{
    public class BaseAgent
    {
        protected readonly World world;
        protected readonly FolKnowledgeBase kb;
        public HashSet<(int x, int y)> Visited { get; } = new();
        protected readonly int visionRadius;
        public (int x, int y) Position { get; set; }

        protected BaseAgent(World world, FolKnowledgeBase kb, int startX, int startY, int vision = 10)
        {
            visionRadius = vision;
            this.world = world;
            this.kb = kb;
            Position = (startX, startY);
        }

        protected bool InVision(int cx, int cy, int x, int y)
            => Math.Abs(cx - x) + Math.Abs(cy - y) <= visionRadius;

        protected bool HasFact(string name, (int x, int y) pos)
        {
            return kb.Facts.Any(f =>
                f.Name == name &&
                f.Args[0] == pos.x.ToString() &&
                f.Args[1] == pos.y.ToString());
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
    }
}
