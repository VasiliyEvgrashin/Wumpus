namespace Wumpus
{
    public class Helper
    {
        public static void FillRules(FolKnowledgeBase kb)
        {
            // ---------------------------------------------------------
            // BASIC RULES (yours)
            // ---------------------------------------------------------

            // NoBreeze(x,y) ∧ Adjacent(x,y,x2,y2) → NotPit(x2,y2)
            kb.TellRule(new FolRule(
                new[] { new Predicate("NoBreeze", "x", "y"), new Predicate("Adjacent", "x", "y", "x2", "y2") },
                new Predicate("NotPit", "x2", "y2")
            ));

            // Breeze(x,y) ∧ Adjacent(x,y,x2,y2) → PossiblePit(x2,y2)
            kb.TellRule(new FolRule(
                new[] { new Predicate("Breeze", "x", "y"), new Predicate("Adjacent", "x", "y", "x2", "y2") },
                new Predicate("PossiblePit", "x2", "y2")
            ));

            // NoStench(x,y) ∧ Adjacent(x,y,x2,y2) → NotWumpus(x2,y2)
            kb.TellRule(new FolRule(
                new[] { new Predicate("NoStench", "x", "y"), new Predicate("Adjacent", "x", "y", "x2", "y2") },
                new Predicate("NotWumpus", "x2", "y2")
            ));

            // Stench(x,y) ∧ Adjacent(x,y,x2,y2) → PossibleWumpus(x2,y2)
            kb.TellRule(new FolRule(
                new[] { new Predicate("Stench", "x", "y"), new Predicate("Adjacent", "x", "y", "x2", "y2") },
                new Predicate("PossibleWumpus", "x2", "y2")
            ));

            // Light(x,y) ∧ Adjacent(x,y,x2,y2) → PossibleExit(x2,y2)
            kb.TellRule(new FolRule(
                new[] { new Predicate("Light", "x", "y"), new Predicate("Adjacent", "x", "y", "x2", "y2") },
                new Predicate("PossibleExit", "x2", "y2")
            ));

            // NoLight(x,y) ∧ Adjacent(x,y,x2,y2) → NotExit(x2,y2)
            kb.TellRule(new FolRule(
                new[] { new Predicate("NoLight", "x", "y"), new Predicate("Adjacent", "x", "y", "x2", "y2") },
                new Predicate("NotExit", "x2", "y2")
            ));

            // NotPit(x,y) ∧ NotWumpus(x,y) → Safe(x,y)
            kb.TellRule(new FolRule(
                new[] { new Predicate("NotPit", "x", "y"), new Predicate("NotWumpus", "x", "y") },
                new Predicate("Safe", "x", "y")
            ));


            // ---------------------------------------------------------
            // IMPROVEMENTS (minimal patch)
            // ---------------------------------------------------------

            // PossiblePit(x,y) ∧ NotPit(x,y) → Remove(PossiblePit(x,y))
            kb.TellRule(new FolRule(
                new[] { new Predicate("PossiblePit", "x", "y"), new Predicate("NotPit", "x", "y") },
                new Predicate("Remove", "PossiblePit", "x", "y")
            ));

            // PossibleWumpus(x,y) ∧ NotWumpus(x,y) → Remove(PossibleWumpus(x,y))
            kb.TellRule(new FolRule(
                new[] { new Predicate("PossibleWumpus", "x", "y"), new Predicate("NotWumpus", "x", "y") },
                new Predicate("Remove", "PossibleWumpus", "x", "y")
            ));


            // ---------------------------------------------------------
            // SAFE from absence of sensors
            // ---------------------------------------------------------

            // NoBreeze(x,y) ∧ NoStench(x,y) → Safe(x,y)
            kb.TellRule(new FolRule(
                new[] { new Predicate("NoBreeze", "x", "y"), new Predicate("NoStench", "x", "y") },
                new Predicate("Safe", "x", "y")
            ));


            // ---------------------------------------------------------
            // LOGIC OF A SINGLE POSSIBLE PIT
            // (simplified version for your engine)
            // ---------------------------------------------------------

            // If Breeze(x,y) and PossiblePit(x2,y2) is the only candidate → Pit(x2,y2)
            kb.TellRule(new FolRule(
                new[]
                {
                    new Predicate("Breeze", "x", "y"),
                    new Predicate("Adjacent", "x", "y", "x2", "y2"),
                    new Predicate("PossiblePit", "x2", "y2"),
                    new Predicate("AllOtherNeighborsNotPit", "x", "y", "x2", "y2")
                },
                new Predicate("Pit", "x2", "y2")
            ));

            // Same for Wumpus
            kb.TellRule(new FolRule(
                new[]
                {
                    new Predicate("Stench", "x", "y"),
                    new Predicate("Adjacent", "x", "y", "x2", "y2"),
                    new Predicate("PossibleWumpus", "x2", "y2"),
                    new Predicate("AllOtherNeighborsNotWumpus", "x", "y", "x2", "y2")
                },
                new Predicate("Wumpus", "x2", "y2")
            ));
        }
    }
}