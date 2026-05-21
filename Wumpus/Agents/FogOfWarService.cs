namespace Wumpus.Agents
{
    public class FogOfWarService
    {
        private readonly World world;
        private readonly FolKnowledgeBase kb;
        private readonly int visionRadius;

        public FogOfWarService(World world, FolKnowledgeBase kb, int visionRadius)
        {
            this.world = world;
            this.kb = kb;
            this.visionRadius = visionRadius;
        }

        public void UpdateAdjacency((int x, int y) center)
        {
            kb.RemoveFactsByName("Adjacent");
            var (cx, cy) = center;

            for (int x = cx - visionRadius; x <= cx + visionRadius; x++)
            {
                for (int y = cy - visionRadius; y <= cy + visionRadius; y++)
                {
                    if (x < 0 || x >= world.Size || y < 0 || y >= world.Size)
                        continue;

                    foreach (var (nx, ny) in world.Adjacent(x, y))
                    {
                        if (nx < 0 || nx >= world.Size || ny < 0 || ny >= world.Size)
                            continue;

                        if (Math.Abs(cx - x) + Math.Abs(cy - y) <= visionRadius &&
                            Math.Abs(cx - nx) + Math.Abs(cy - ny) <= visionRadius)
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
    }
}