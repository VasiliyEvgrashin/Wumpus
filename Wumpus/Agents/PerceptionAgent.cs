namespace Wumpus.Agents
{
    public class PerceptionAgent : BaseAgent
    {
        protected PerceptionAgent(World world, FolKnowledgeBase kb, int startX, int startY)
            : base(world, kb, startX, startY)
        {
        }

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
    }
}
