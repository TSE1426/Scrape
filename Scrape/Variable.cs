using Scrape.Backend;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scrape
{
    public class Variable : Tile
    {
        public string Name;

        public Variable(string name)
        {
            Name = name;
            b.Content = name;
        }

        public virtual Value.ValueType GetValueType()
        {
            throw new InvalidOperationException();
        }

        public override Border CreateBlock()
        {
            Border block = base.CreateBlock();
            return block;
        }
    }

    public class numberVariable : Variable
    {
        public numberVariable(string name) : base(name)
        {
            b.Background = Brushes.LightBlue;
        }

        public override Value.ValueType GetValueType() => Value.ValueType.Number;

        public override Border CreateBlock()
        {
            Border block = base.CreateBlock();
            block.Background = Brushes.LightBlue;
            return block;
        }
    }

    public class stringVariable : Variable
    {
        public stringVariable(string name) : base(name)
        {
            b.Background = Brushes.LightCoral;
        }

        public override Value.ValueType GetValueType() => Value.ValueType.String;

        public override Border CreateBlock()
        {
            Border block = base.CreateBlock();
            block.Background = Brushes.LightCoral;
            return block;
        }
    }

    public class boolVariable : Variable
    {
        public boolVariable(string name) : base(name)
        {
            b.Background = Brushes.LightGreen;
        }

        public override Value.ValueType GetValueType() => Value.ValueType.Boolean;

        public override Border CreateBlock()
        {
            Border block = base.CreateBlock();
            block.Background = Brushes.LightGreen;
            return block;
        }
    }
}
