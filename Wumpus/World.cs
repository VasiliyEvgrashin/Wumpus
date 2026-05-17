namespace Wumpus
{
    public class World
    {
        public int Size { get; }
        public CellContent[,] Map { get; }

        public World(int size)
        {
            Size = size;
            Map = new CellContent[size, size];
        }

        public bool HasBreeze(int x, int y)
        {
            return Adjacent(x, y).Any(c => Map[c.x, c.y] == CellContent.Pit);
        }

        public bool HasStench(int x, int y)
        {
            return Adjacent(x, y).Any(c => Map[c.x, c.y] == CellContent.Wumpus);
        }

        public bool HasLight(int x, int y)
        {
            return Adjacent(x, y).Any(c => Map[c.x, c.y] == CellContent.Exit);
        }

        public IEnumerable<(int x, int y)> Adjacent(int x, int y)
        {
            if (x > 0) yield return (x - 1, y);
            if (x < Size - 1) yield return (x + 1, y);
            if (y > 0) yield return (x, y - 1);
            if (y < Size - 1) yield return (x, y + 1);
        }
    }

}
