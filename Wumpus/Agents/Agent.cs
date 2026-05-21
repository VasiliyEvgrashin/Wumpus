namespace Wumpus.Agents
{
    public class Agent : StrategyAgent
    {
        
        private int stepCounter = 0;

        public Agent(World world, FolKnowledgeBase kb, int startX, int startY)
            : base(world, kb, startX, startY)
        {
            Visited.Add(Position);
            TellVisited(Position);
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

        protected override void MoveTo((int x, int y) next)
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