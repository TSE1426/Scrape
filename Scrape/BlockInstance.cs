using System.Collections.Generic;
using System.Windows.Controls;
using Scrape.Backend;

namespace Scrape
{
    // A block dropped in the code area: the visual border + the backend node(s) it represents.
    public class BlockInstance
    {
        public Border Border;            // visual block on the canvas
        public Node Node;                // primary backend node
        public InFlowPin InFlow;         // where snapping from above plugs in
        public OutFlowPin OutFlow;       // where snapping from below plugs in
        public List<Slot> Slots;         // slots used at run time to wire constants/get-variable nodes
        public List<Node> AllNodes;      // every backend node this block owns (for clean delete)

        public BlockInstance Above;      // block snapped above us (null if top)
        public BlockInstance Below;      // block snapped below us (null if bottom)

        public BlockInstance()
        {
            Slots = new List<Slot>();
            AllNodes = new List<Node>();
        }
    }
}
