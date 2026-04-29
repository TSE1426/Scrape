using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Scrape
{
    public class BlockDragManager
    {
        private bool isDragging = false;
        private UIElement draggedBlock = null;
        private Point clickPosition;
        private readonly Canvas canvas;

        public BlockDragManager(Canvas targetCanvas)
        {
            canvas = targetCanvas;
        }

        public void Attach(UIElement block)
        {
            block.MouseLeftButtonDown += Block_MouseLeftButtonDown;
            block.MouseMove += Block_MouseMove;
            block.MouseLeftButtonUp += Block_MouseLeftButtonUp;
        }

        private void Block_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedBlock = sender as UIElement;
            if (draggedBlock == null) return;

            isDragging = true;
            clickPosition = e.GetPosition(canvas);
            draggedBlock.CaptureMouse();
        }

        private void Block_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging || draggedBlock == null) return;

            Point currentPosition = e.GetPosition(canvas);

            double left = Canvas.GetLeft(draggedBlock);
            double top = Canvas.GetTop(draggedBlock);

            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;

            double offsetX = currentPosition.X - clickPosition.X;
            double offsetY = currentPosition.Y - clickPosition.Y;

            Canvas.SetLeft(draggedBlock, left + offsetX);
            Canvas.SetTop(draggedBlock, top + offsetY);

            clickPosition = currentPosition;
        }

        private void Block_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (draggedBlock != null)
            {
                draggedBlock.ReleaseMouseCapture();
            }

            isDragging = false;
            draggedBlock = null;
        }
    }
}