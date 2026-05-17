namespace Wumpus.Agents
{
    public class BaseAgent
    {
        protected readonly World world;
        protected readonly FolKnowledgeBase kb;

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
    }
}
