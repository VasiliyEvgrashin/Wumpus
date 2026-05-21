namespace Wumpus.Agents
{
    public class Agent : StrategyAgent
    {
        private readonly FogOfWarService fog;
        private readonly VisualizationService viz;

        private int stepCounter = 0;

        public Agent(World world, FolKnowledgeBase kb, int startX, int startY, int vision = 10)
            : base(world, kb, startX, startY, vision)
        {
            fog = new FogOfWarService(world, kb, vision);
            viz = new VisualizationService(world, kb, vision);

            fog.UpdateAdjacency(Position);
            kb.Infer();
        }

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

            viz.DrawWorld(Position);

            if (world.Map[next.Value.x, next.Value.y] == CellContent.Exit)
            {
                Console.WriteLine("Agent reached EXIT");
                return false;
            }

            return true;
        }

        protected override void MoveTo((int x, int y) next)
        {
            base.MoveTo(next);

            if (HasFact("Safe", next))
                kb.RemoveAllFactsExceptSafeAndVisited(next.x, next.y);

            fog.UpdateAdjacency(next);
        }
    }
}