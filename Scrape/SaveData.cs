using System.Collections.Generic;

namespace Scrape
{
    public class SaveData
    {
        public List<SavedVariable> Variables { get; set; } = new();
        public List<SavedSprite> Sprites { get; set; } = new();
        public List<SavedBlock> Blocks { get; set; } = new();
    }

    public class SavedVariable
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }

    public class SavedSprite
    {
        public string Name { get; set; }
        public string Shape { get; set; }
        public string Color { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class SavedBlock
    {
        public string Type { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public List<SavedSlot> Slots { get; set; } = new();
        public string ExtraValue { get; set; }
    }

    public class SavedSlot
    {
        public string Kind { get; set; }
        public string Value { get; set; }
    }
}