//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Windows.Controls;

//namespace Scrape
//{
//    public abstract class Tile
//    {
//        public Button b;
//    }
//}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
    }
}

