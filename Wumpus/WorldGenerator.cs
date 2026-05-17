namespace Wumpus
{
    public class WorldGenerator
    {
        private Random rnd;

        public WorldGenerator()
        {
            byte[] bytes = Guid.NewGuid().ToByteArray();
            int seed = BitConverter.ToInt32(bytes, bytes.Length - 4);
            rnd = new Random(seed);
        }

        public World Generate(int size, int pitCount, int wumpusCount)
        {
            var world = new World(size);
            var path = GenerateGuaranteedPath(size);
            MarkSafeCorridor(world, path);
            var exit = path[^1];
            world.Map[exit.x, exit.y] = CellContent.Exit;
            PlaceRandom(world, CellContent.Pit, pitCount, path);
            PlaceRandom(world, CellContent.Wumpus, wumpusCount, path);
            return world;
        }

        private List<(int x, int y)> GenerateGuaranteedPath(int size)
        {
            var path = new List<(int x, int y)>();
            int x = 0, y = 0;
            path.Add((x, y));
            int targetX = rnd.Next(size / 2, size);
            int targetY = rnd.Next(size / 2, size);
            while (x != targetX || y != targetY)
            {
                bool moveRight = rnd.Next(2) == 0;

                if (moveRight && x < targetX)
                    x++;
                else if (y < targetY)
                    y++;
                else if (x < targetX)
                    x++;

                path.Add((x, y));
            }

            return path;
        }

        private void MarkSafeCorridor(World world, List<(int x, int y)> path)
        {
            int size = world.Size;

            foreach (var (x, y) in path)
            {
                world.Map[x, y] = CellContent.Empty;
                foreach (var (nx, ny) in world.Adjacent(x, y))
                    world.Map[nx, ny] = CellContent.Empty;
            }
        }

        private void PlaceRandom(World world, CellContent type, int count, List<(int x, int y)> path)
        {
            int size = world.Size;
            var forbidden = new HashSet<(int, int)>(path);
            foreach (var (x, y) in path)
            {
                foreach (var (nx, ny) in world.Adjacent(x, y))
                    forbidden.Add((nx, ny));
            }

            while (count > 0)
            {
                int x = rnd.Next(size);
                int y = rnd.Next(size);

                if (forbidden.Contains((x, y)))
                    continue;

                if (world.Map[x, y] != CellContent.Empty)
                    continue;

                world.Map[x, y] = type;
                count--;
            }
        }
    }
}