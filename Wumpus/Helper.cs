namespace Wumpus
{
    public class Helper
    {
        public static void FillRules(FolKnowledgeBase kb)
        {
            // ---------------------------------------------------------
            // BASIC RULES (with weights)
            // ---------------------------------------------------------

            // NoBreeze(x,y) ∧ Adjacent → NotPit(x2,y2)
            kb.TellRule(new FolRule(
                new[] { new Predicate("NoBreeze", "x", "y"), new Predicate("Adjacent", "x", "y", "x2", "y2") },
                new Predicate("NotPit", "x2", "y2"),
                weight: 3.0
            ));

            // Breeze(x,y) ∧ Adjacent → PossiblePit(x2,y2)
            kb.TellRule(new FolRule(
                new[] { new Predicate("Breeze", "x", "y"), new Predicate("Adjacent", "x", "y", "x2", "y2") },
                new Predicate("PossiblePit", "x2", "y2"),
                weight: 2.0
            ));

            // NoStench(x,y) ∧ Adjacent → NotWumpus(x2,y2)
            kb.TellRule(new FolRule(
                new[] { new Predicate("NoStench", "x", "y"), new Predicate("Adjacent", "x", "y", "x2", "y2") },
                new Predicate("NotWumpus", "x2", "y2"),
                weight: 3.0
            ));

            // Stench(x,y) ∧ Adjacent → PossibleWumpus(x2,y2)
            kb.TellRule(new FolRule(
                new[] { new Predicate("Stench", "x", "y"), new Predicate("Adjacent", "x", "y", "x2", "y2") },
                new Predicate("PossibleWumpus", "x2", "y2"),
                weight: 2.0
            ));

            // Light(x,y) ∧ Adjacent → PossibleExit(x2,y2)
            kb.TellRule(new FolRule(
                new[] { new Predicate("Light", "x", "y"), new Predicate("Adjacent", "x", "y", "x2", "y2") },
                new Predicate("PossibleExit", "x2", "y2"),
                weight: 2.0
            ));

            // NoLight(x,y) ∧ Adjacent → NotExit(x2,y2)
            kb.TellRule(new FolRule(
                new[] { new Predicate("NoLight", "x", "y"), new Predicate("Adjacent", "x", "y", "x2", "y2") },
                new Predicate("NotExit", "x2", "y2"),
                weight: 3.0
            ));


            // ---------------------------------------------------------
            // SAFE rules
            // ---------------------------------------------------------

            // NotPit(x,y) ∧ NotWumpus(x,y) → Safe(x,y)
            kb.TellRule(new FolRule(
                new[] { new Predicate("NotPit", "x", "y"), new Predicate("NotWumpus", "x", "y") },
                new Predicate("Safe", "x", "y"),
                weight: 1.5
            ));

            // NoBreeze(x,y) ∧ NoStench(x,y) → Safe(x,y)
            kb.TellRule(new FolRule(
                new[] { new Predicate("NoBreeze", "x", "y"), new Predicate("NoStench", "x", "y") },
                new Predicate("Safe", "x", "y"),
                weight: 2.0
            ));


            // ---------------------------------------------------------
            // IMPROVEMENTS
            // ---------------------------------------------------------

            // PossiblePit(x,y) ∧ NotPit(x,y) → Remove(PossiblePit)
            kb.TellRule(new FolRule(
                new[] { new Predicate("PossiblePit", "x", "y"), new Predicate("NotPit", "x", "y") },
                new Predicate("Remove", "PossiblePit", "x", "y"),
                weight: 0.5
            ));

            // PossibleWumpus(x,y) ∧ NotWumpus(x,y) → Remove(PossibleWumpus)
            kb.TellRule(new FolRule(
                new[] { new Predicate("PossibleWumpus", "x", "y"), new Predicate("NotWumpus", "x", "y") },
                new Predicate("Remove", "PossibleWumpus", "x", "y"),
                weight: 0.5
            ));


            // ---------------------------------------------------------
            // LOGIC OF A SINGLE POSSIBLE PIT
            // ---------------------------------------------------------

            // Breeze + only one candidate → Pit
            kb.TellRule(new FolRule(
                new[]
                {
            new Predicate("Breeze", "x", "y"),
            new Predicate("Adjacent", "x", "y", "x2", "y2"),
            new Predicate("PossiblePit", "x2", "y2"),
            new Predicate("AllOtherNeighborsNotPit", "x", "y", "x2", "y2")
                },
                new Predicate("Pit", "x2", "y2"),
                weight: 4.0
            ));

            // Stench + only one candidate → Wumpus
            kb.TellRule(new FolRule(
                new[]
                {
            new Predicate("Stench", "x", "y"),
            new Predicate("Adjacent", "x", "y", "x2", "y2"),
            new Predicate("PossibleWumpus", "x2", "y2"),
            new Predicate("AllOtherNeighborsNotWumpus", "x", "y", "x2", "y2")
                },
                new Predicate("Wumpus", "x2", "y2"),
                weight: 4.0
            ));
        }
    }
}