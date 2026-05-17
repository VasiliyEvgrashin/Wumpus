namespace Wumpus
{
    public class FolRule
    {
        public List<Predicate> Premises { get; }
        public Predicate Conclusion { get; }

        public FolRule(IEnumerable<Predicate> premises, Predicate conclusion)
        {
            Premises = premises.ToList();
            Conclusion = conclusion;
        }
    }

}
