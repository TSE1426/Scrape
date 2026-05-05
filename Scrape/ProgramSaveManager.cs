using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Scrape
{
    public static class ProgramSaveManager
    {
        public static string Save(MainWindow main)
        {
            SaveData data = new SaveData();

            foreach (var v in main.GetVariables())
            {
                data.Variables.Add(new SavedVariable
                {
                    Name = v.Name,
                    Type = v switch
                    {
                        numberVariable => "Number",
                        stringVariable => "String",
                        boolVariable => "Boolean",
                        _ => "Unknown"
                    }
                });
            }

            foreach (var s in main.GetSprites())
            {
                double x = s.visual == null ? 0 : Canvas.GetLeft(s.visual);
                double y = s.visual == null ? 0 : Canvas.GetTop(s.visual);

                if (double.IsNaN(x)) x = 0;
                if (double.IsNaN(y)) y = 0;

                data.Sprites.Add(new SavedSprite
                {
                    Name = s.Name,
                    Shape = s.Shape.ToString(),
                    Color = s.Color.ToString(),
                    X = x,
                    Y = y
                });
            }

            foreach (var block in main.GetCodeAreaManager().Instances)
            {
                if (block.IsLocked) continue;
                if (string.IsNullOrEmpty(block.SaveType)) continue;

                double x = Canvas.GetLeft(block.Border);
                double y = Canvas.GetTop(block.Border);

                if (double.IsNaN(x)) x = 0;
                if (double.IsNaN(y)) y = 0;

                SavedBlock saved = new SavedBlock
                {
                    Type = block.SaveType,
                    X = x,
                    Y = y
                };

                foreach (var slot in block.Slots)
                {
                    saved.Slots.Add(SaveSlot(slot));
                }

                if (block.ExtraSaveControl is ComboBox combo && combo.SelectedItem != null)
                {
                    saved.ExtraValue = combo.SelectedItem.ToString();
                }

                data.Blocks.Add(saved);
            }

            return JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        private static SavedSlot SaveSlot(Slot slot)
        {
            if (slot.Value == null)
            {
                return new SavedSlot { Kind = "Empty", Value = "" };
            }

            if (slot.Value is Variable v)
            {
                return new SavedSlot { Kind = "Variable", Value = v.Name };
            }

            if (slot.Value is Sprite s)
            {
                return new SavedSlot { Kind = "Sprite", Value = s.Name };
            }

            if (slot.Value is double d)
            {
                return new SavedSlot { Kind = "Number", Value = d.ToString() };
            }

            if (slot.Value is bool b)
            {
                return new SavedSlot { Kind = "Boolean", Value = b.ToString() };
            }

            if (slot.Value is string str)
            {
                return new SavedSlot { Kind = "String", Value = str };
            }

            return new SavedSlot { Kind = "Empty", Value = "" };
        }
    }
}