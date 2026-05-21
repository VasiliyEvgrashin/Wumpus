namespace Wumpus.Agents
{
    public class VisualizationAgent : PerceptionAgent
    {
        protected VisualizationAgent(World world, FolKnowledgeBase kb, int startX, int startY)
            : base(world, kb, startX, startY)
        {
        }

        public void DrawWorld()
        {
            Console.Clear();
            var (ax, ay) = Position;
            for (int y = 0; y < world.Size; y++)
            {
                for (int x = 0; x < world.Size; x++)
                {
                    if (!InVision(ax, ay, x, y))
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
