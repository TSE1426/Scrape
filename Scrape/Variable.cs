//using System.Windows.Controls;

//namespace Scrape
//{


//    public class Variable : Tile
//    {
//        public string Name;
//        public Variable(string name)
//        {
//            Name = name;
//            b = new Button();
//            b.Content = name;
//            b.Height = 30;
//        }
//    }

//    public class numberVariable : Variable
//    {
//        public numberVariable(string name) : base(name)
//        {
//            b.Background = System.Windows.Media.Brushes.LightBlue; // color for number variables
//        }
//    }

//    public class stringVariable : Variable
//    {
//        public stringVariable(string name) : base(name)
//        {
//            b.Background = System.Windows.Media.Brushes.LightCoral; // color for string variables
//        }
//    }

//    public class boolVariable : Variable
//    {
//        public boolVariable(string name) : base(name)
//        {
//            b.Background = System.Windows.Media.Brushes.LightGreen; // color for boolean variables
//        }
//    }
//}
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

        public override Border CreateBlock()
        {
            Border block = base.CreateBlock();
            block.Background = Brushes.LightGreen;
            return block;
        }
    }
}
