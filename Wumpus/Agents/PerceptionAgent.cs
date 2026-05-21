namespace Wumpus.Agents
{
    public abstract class PerceptionAgent : BaseAgent
    {
        protected PerceptionAgent(World world, FolKnowledgeBase kb, int startX, int startY, int vision = 10)
            : base(world, kb, startX, startY, vision)
        {
            TellVisited(Position);
        }

        protected void Perceive()
        {
            var (x, y) = Position;
            var breeze = world.HasBreeze(x, y);
            var stench = world.HasStench(x, y);
            var light = world.HasLight(x, y);

            kb.TellFact(new Predicate(breeze ? "Breeze" : "NoBreeze", x.ToString(), y.ToString()));
            kb.TellFact(new Predicate(stench ? "Stench" : "NoStench", x.ToString(), y.ToString()));
            kb.TellFact(new Predicate(light ? "Light" : "NoLight", x.ToString(), y.ToString()));
        }

        protected void TellVisited((int x, int y) pos)
        {
            kb.TellFact(new Predicate("Visited", pos.x.ToString(), pos.y.ToString()));
        }

        protected override void MoveTo((int x, int y) next)
        {
            base.MoveTo(next);
            TellVisited(next);
        }
    }
}