namespace Wumpus.Agents
{
    public class VisualizationService
    {
        private readonly World world;
        private readonly FolKnowledgeBase kb;
        private readonly int visionRadius;

        public VisualizationService(World world, FolKnowledgeBase kb, int visionRadius)
        {
            this.world = world;
            this.kb = kb;
            this.visionRadius = visionRadius;
        }

        public void DrawWorld((int x, int y) agentPos)
        {
            Console.Clear();
            var (ax, ay) = agentPos;

            for (int y = 0; y < world.Size; y++)
            {
                for (int x = 0; x < world.Size; x++)
                {
                    if (Math.Abs(ax - x) + Math.Abs(ay - y) > visionRadius)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("? ");
                        Console.ResetColor();
                        continue;
                    }

                    if (ax == x && ay == y)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("A ");
                        Console.ResetColor();
                        continue;
                    }

                    // Здесь можно использовать либо реальные данные мира,
                    // либо знания агента (HasFact) — в зависимости от цели.
                    bool breeze = world.HasBreeze(x, y);
                    bool stench = world.HasStench(x, y);
                    bool light = world.HasLight(x, y);

                    if (breeze && stench && light)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write("BS");
                        Console.ResetColor();
                        Console.Write(" ");
                        continue;
                    }
                    else if (breeze)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("B ");
                        Console.ResetColor();
                        continue;
                    }
                    else if (stench)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write("S ");
                        Console.ResetColor();
                        continue;
                    }
                    else if (light)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("L ");
                        Console.ResetColor();
                        continue;
                    }

                    switch (world.Map[x, y])
                    {
                        case CellContent.Empty:
                            Console.Write(". ");
                            break;
                        case CellContent.Pit:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("P ");
                            Console.ResetColor();
                            break;
                        case CellContent.Wumpus:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("W ");
                            Console.ResetColor();
                            break;
                        case CellContent.Exit:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("E ");
                            Console.ResetColor();
                            break;
                        default:
                            Console.Write("? ");
                            break;
                    }
                }
                Console.WriteLine();
            }
        }
    }
}