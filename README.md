Wumpus World Agent

<img width="547" height="597" alt="image" src="https://github.com/user-attachments/assets/53be42b8-6d3a-4c3e-a7f0-c000625f0f8c" />

A logical agent for the classic Wumpus World. The agent explores an unknown grid, analyzes sensor data, updates its internal knowledge base, and makes decisions using logical inference. The project includes a full world simulation, a reasoning engine, and console visualization.
Project Overview

The project includes:
- generation of an N×N world with pits, the Wumpus, and gold
- a sensor model (Breeze, Stench, Glitter)
- an agent with a dynamic knowledge base
- inference of safe and dangerous cells using FOL‑style rules
- path planning (BFS/A*)
- step‑by‑step world exploration with hypothesis updates
- console visualization of the world and the agent’s knowledge

The agent sequentially:
- reads percepts
- updates its knowledge
- performs logical inference
- selects the next target
- builds a route
- moves through the world
The project demonstrates how a logical agent operates under uncertainty and can serve as a foundation for experimenting with reasoning algorithms, probabilistic models, and extended behavior rules.

FOL Engine Capability Constraints
- Predicates: Name(arg1, arg2, …) with string arguments.
- Facts: ground predicates stored explicitly.
- Horn Rules: P1 ∧ P2 ∧ … ∧ Pn ⇒ Q (single head, conjunctive body).
- Forward Chaining: deterministic, iterative, until fixpoint.
- Unification: simple positional matching, no occurs‑check, no types.
- Ground Reasoning: all inference over ground predicates only.
- Explicit Negation: only via named predicates (NotX, NoY).
- No functions, no nested terms.

Risk Evaluation via Weighted Logical Rules

This mechanism converts the activation of logical rules into a numerical risk estimate for a given fact (e.g., Pit(x,y) or Wumpus(x,y)).
It is a deterministic computation based solely on rule weights and rule applicability.

C#
