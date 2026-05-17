namespace Wumpus
{
    public class Predicate
    {
        public string Name { get; }
        public List<string> Args { get; }

        public Predicate(string name, params string[] args)
        {
            Name = name;
            Args = args.ToList();
        }

        public override string ToString() => $"{Name}({string.Join(",", Args)})";
    }

}
