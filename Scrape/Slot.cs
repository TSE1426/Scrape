using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace Scrape
{
    public enum SlotType
    {
        VariableOnly,
        NumberOrVariable,
        BooleanOnly
    }
    public class Slot
    {
        public Button Button { get; private set; }
        public SlotType Type { get; private set; }
        public object Value { get; private set; }
        private string placeholder;

        public Slot(SlotType type, string placeholder, RoutedEventHandler clickHandler, MouseButtonEventHandler doubleClickHandler)
        {
            Type = type;
            this.placeholder = placeholder;

            Button = new Button
            {
                Content = placeholder,
                MinWidth = 70,
                Margin = new Thickness(2),
                Background = Brushes.White,
                Tag = this
            };

            Button.Click += clickHandler;
            Button.MouseDoubleClick += doubleClickHandler;
        }

        public void SetVariable(Variable variable)
        {
            Value = variable;
            Button.Content = variable.Name;
            Button.Background = variable.b.Background;
        }

        public void SetNumber(double number)
        {
            Value = number;
            Button.Content = number.ToString();
            Button.Background = Brushes.White;
        }

        public void Clear()
        {
            Value = null;
            Button.Content = placeholder;
            Button.Background = Brushes.White;
        }
    }
}