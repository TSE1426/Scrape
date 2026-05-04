using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using Scrape.Backend;

namespace Scrape
{
    public enum SlotType
    {
        VariableOnly,
        NumberOrVariable,
        BooleanOnly,
        BooleanOrVariable,
        SpriteOnly
    }
    public class Slot
    {
        public Button Button { get; private set; }
        public SlotType Type { get; private set; }
        public object Value { get; private set; }
        private string placeholder;

        // Backend pin this slot feeds into (set by the tile that owns this slot)
        public InPin TargetPin;

        // Called whenever the slot value changes. Used by tiles for special slots
        // like the "counter" slot in a for loop that updates a node's identifier.
        public Action<Slot> OnValueSet;

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
            OnValueSet?.Invoke(this);
        }

        public void SetNumber(double number)
        {
            Value = number;
            Button.Content = number.ToString();
            Button.Background = Brushes.White;
            OnValueSet?.Invoke(this);
        }

        public void SetSprite(Sprite sprite)
        {
            Value = sprite;
            Button.Content = sprite.Name;
            Button.Background = sprite.Color;
            OnValueSet?.Invoke(this);
        }

        public void Clear()
        {
            Value = null;
            Button.Content = placeholder;
            Button.Background = Brushes.White;
            OnValueSet?.Invoke(this);
        }
    }
}
