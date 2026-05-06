# Scrape

Scrape is a visual programming prototype built with WPF. Users drag block tiles into the code area, connect them in vertical chains, fill slots (numbers, variables, sprites), and execute the resulting program through a lightweight backend node graph.

## Repository layout

- `Scrape/`: WPF frontend, block UI, drag/drop, slot editing, and run/stop interactions.
- `Scrape.Backend/`: execution model (`Node`, `NodeGraph`, pins, values, and evaluation context).
- `Scrape.Sandbox/`: minimal console entry project for sandbox testing/integration.

## How execution works

1. **Tile to block instance**
   - Clicking a palette tile creates a `BlockInstance` in the canvas via `CodeAreaManager.AddTileBlock`.
   - Each instance carries UI plus backend node/pin references.

2. **Flow graph wiring**
   - Drag/drop snapping in `CodeAreaManager` detects nearby blocks and connects flow pins (`OutFlow -> InFlow`).
   - Top-of-chain blocks are later connected to `Graph.StartNode.StartPin` during run.

3. **Slot value binding at run time**
   - On run, `CodeAreaManager.Run` creates temporary helper nodes for filled input slots:
     - `GetVariableNode` for variable slots.
     - `ConstantNode<double>` for fixed numeric slots.
   - Helper outputs are connected to target input pins before evaluation.

4. **Evaluation**
   - `NodeGraph.Evaluate(ctx)` calls `StartNode.Evaluate(ctx)`.
   - Base `Node.Evaluate` recursively evaluates non-flow input dependencies first.
   - Node implementations read/write `EvaluationContext` (`Stack`, `Variables`, `Errors`) and run behavior.

5. **Frontend side effects**
   - `MainWindow.RunButton_Click` initializes context defaults for variables and registers callbacks so backend nodes can interact with the frontend.

6. **Cleanup after each run**
   - Temporary helper connections, helper nodes, and start-pin links are removed so each run starts from a clean state.
