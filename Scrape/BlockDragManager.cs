using System;
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
        private readonly Action<UIElement> onDropped;

        public BlockDragManager(Canvas targetCanvas, Action<UIElement> onDropped = null)
        {
            canvas = targetCanvas;
            this.onDropped = onDropped;
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

            double newLeft = left + offsetX;
            double newTop = top + offsetY;

            // keep the block inside the canvas
            var fe = draggedBlock as FrameworkElement;
            double w = fe?.ActualWidth ?? 0;
            double h = fe?.ActualHeight ?? 0;

            if (canvas.ActualWidth > 0)
                newLeft = Math.Max(0, Math.Min(newLeft, canvas.ActualWidth - w));
            else
                newLeft = Math.Max(0, newLeft);

            if (canvas.ActualHeight > 0)
                newTop = Math.Max(0, Math.Min(newTop, canvas.ActualHeight - h));
            else
                newTop = Math.Max(0, newTop);

            Canvas.SetLeft(draggedBlock, newLeft);
            Canvas.SetTop(draggedBlock, newTop);

            clickPosition = currentPosition;
        }

        private void Block_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIElement dropped = draggedBlock;

            if (draggedBlock != null)
            {
                draggedBlock.ReleaseMouseCapture();
            }

            isDragging = false;
            draggedBlock = null;

            if (dropped != null)
            {
                onDropped?.Invoke(dropped);
            }
        }
    }
}
