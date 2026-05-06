using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Scrape
{
    public enum SpriteShape
    {
        Square,
        Circle,
        Triangle
    }

    // Sprite is a Tile so it can be clicked in the palette like a Variable
    public class Sprite : Tile
    {
        public string Name;
        public SpriteShape Shape;
        public Brush Color;
        public Shape visual;   // shape shown in the output area

        public Sprite(string name, SpriteShape shape, Brush color)
        {
            Name = name;
            Shape = shape;
            Color = color;

            b.Content = name;
            b.Background = color;
        }

        // make the shape that will be placed on the output canvas
        public Shape CreateVisual()
        {
            Shape s;

            if (Shape == SpriteShape.Circle)
            {
                s = new Ellipse { Width = 50, Height = 50 };
            }
            else if (Shape == SpriteShape.Triangle)
            {
                Polygon t = new Polygon();
                t.Points.Add(new Point(25, 0));
                t.Points.Add(new Point(50, 50));
                t.Points.Add(new Point(0, 50));
                s = t;
            }
            else // Square
            {
                s = new Rectangle { Width = 50, Height = 50 };
            }

            s.Fill = Color;
            s.Stroke = Brushes.Black;
            s.StrokeThickness = 1;
            s.Tag = this;

            visual = s;
            return s;
        }

        // move the sprite on the output canvas by (dx, dy)
        public void MoveBy(double dx, double dy)
        {
            if (visual == null) return;

            double left = Canvas.GetLeft(visual);
            double top = Canvas.GetTop(visual);
            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;

            Canvas.SetLeft(visual, left + dx);
            Canvas.SetTop(visual, top + dy);
        }
    }
}
