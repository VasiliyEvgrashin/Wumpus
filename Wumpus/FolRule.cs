namespace Wumpus
{
    public class FolRule
    {
        public List<Predicate> Premises { get; }
        public Predicate Conclusion { get; }
        public double Weight { get; }

        public FolRule(IEnumerable<Predicate> premises, Predicate conclusion, double weight = 1.0)
        {
            Premises = premises.ToList();
            Conclusion = conclusion;
            Weight = weight;
        }
    }
}
