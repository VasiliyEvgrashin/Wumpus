using Wumpus;
using Wumpus.Agents;

int size = 30;
WorldGenerator worldGenerator = new WorldGenerator();
var world = worldGenerator.Generate(size, 10, 10);
var kb = new FolKnowledgeBase();
Helper.FillRules(kb);
var agent = new Agent(world, kb, 0, 0);

int len = size * size;
for (int step = 1; step <= len; step++)
{
    bool moved = agent.Step();
    if (!moved)
    {
        break;
    }
}
Console.WriteLine();
Console.WriteLine($"Done. KB count {kb.Facts.Count()}. Press Enter.");
Console.ReadLine();