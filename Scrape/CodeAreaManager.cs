using System.Windows.Controls;

namespace Scrape
{
    public class CodeAreaManager
    {
        private readonly Canvas codeArea;
        private readonly BlockDragManager dragManager;

        public CodeAreaManager(Canvas canvas)
        {
            codeArea = canvas;
            dragManager = new BlockDragManager(codeArea);
        }

        public void AddTileBlock(Tile tile)
        {
            Border newBlock = tile.CreateBlock();

            dragManager.Attach(newBlock);

            Canvas.SetLeft(newBlock, 20);
            Canvas.SetTop(newBlock, 20 + (codeArea.Children.Count * 50));

            codeArea.Children.Add(newBlock);
        }
    }
}