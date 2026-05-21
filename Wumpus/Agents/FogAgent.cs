namespace Wumpus.Agents
{
    public class FogAgent : VisualizationAgent
    {
        
        public FogAgent(World world, FolKnowledgeBase kb, int startX, int startY)
            : base(world, kb, startX, startY)
        {
            UpdateAdjacency();
        }

        protected void UpdateAdjacency()
        {
            kb.RemoveFactsByName("Adjacent");
            var (cx, cy) = Position;
            for (int x = cx - visionRadius; x <= cx + visionRadius; x++)
            {
                for (int y = cy - visionRadius; y <= cy + visionRadius; y++)
                {
                    if (!IsInside(x, y))
                        continue;
                    foreach (var (nx, ny) in world.Adjacent(x, y))
                    {
                        if (!IsInside(nx, ny))
                            continue;
                        if (InVision(cx, cy, x, y) && InVision(cx, cy, nx, ny))
                        {
                            kb.TellFact(new Predicate(
                                "Adjacent",
                                x.ToString(), y.ToString(),
                                nx.ToString(), ny.ToString()));
                        }
                    }
                }
            }
        }

        protected bool IsInside(int x, int y)
            => x >= 0 && x < world.Size && y >= 0 && y < world.Size;

        protected virtual void MoveTo((int x, int y) next)
        {
            Position = next;
            UpdateAdjacency();
        }
    }
}