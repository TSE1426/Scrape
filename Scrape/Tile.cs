using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Scrape.Backend;

namespace Scrape
{
    public class Tile
    {
        public Button b;

        public Tile()
        {
            b = new Button();
            b.Height = 30;
            b.Margin = new Thickness(5);
        }

        public void SetupPaletteButton(RoutedEventHandler clickHandler)
        {
            b.Tag = this;
            b.Click -= clickHandler;
            b.Click += clickHandler;
        }

        protected Border CreateStyledBlock(Brush background, UIElement content, bool showArrow = true)
        {
            var shell = new StackPanel { Orientation = Orientation.Vertical };

            shell.Children.Add(new Rectangle
            {
                Width = 18,
                Height = 5,
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(12, -1, 0, 0)
            });

            var row = new DockPanel();
            if (showArrow)
            {
                row.Children.Add(new TextBlock
                {
                    Text = "➜",
                    FontSize = 12,
                    Margin = new Thickness(0, 0, 6, 0),
                    VerticalAlignment = VerticalAlignment.Center
                });
            }
            row.Children.Add(content);
            shell.Children.Add(row);

            shell.Children.Add(new Rectangle
            {
                Width = 18,
                Height = 5,
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(24, 0, 0, -1)
            });

            return new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = background,
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(8, 6, 8, 6),
                Child = shell
            };
        }

        public virtual Border CreateBlock()
        {
            Border block = new Border
            {
                Width = 120,
                Height = 40,
                Background = Brushes.LightGray,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5)
            };

            TextBlock text = new TextBlock
            {
                Text = b.Content?.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            block.Child = text;
            return block;
        }

        // Tiles that have a backend representation override this. Default returns null,
        // which makes the code area fall back to CreateBlock() with no node attached.
        public virtual BlockInstance CreateBlockInstance(NodeGraph graph)
        {
            return null;
        }
    }
}
