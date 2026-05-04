using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.VisualBasic;
using Scrape.Backend;

namespace Scrape;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private AssignmentTile assignmentTile;
    private ForLoopTile forLoopTile;
    private MoveSpriteTile moveSpriteTile;
    private IfTile ifTile;
    private Slot selectedSlot = null; // currently selected slot in the code area
    private List<Variable> variables; // list of variables, will be used to store variables created by the user
    private List<Sprite> sprites; // list of sprites the user has made
    private BlockDragManager outputDragManager; // lets us drag sprites in the output area
    private CodeAreaManager codeAreaManager;
    
    public MainWindow()
    {
        InitializeComponent();
        assignmentTile = new AssignmentTile(Slot_Click, Slot_DoubleClick);
        assignmentTile.SetupPaletteButton(PaletteTile_Click);
        forLoopTile = new ForLoopTile(Slot_Click, Slot_DoubleClick);
        forLoopTile.SetupPaletteButton(PaletteTile_Click);
        moveSpriteTile = new MoveSpriteTile(Slot_Click, Slot_DoubleClick);
        moveSpriteTile.SetupPaletteButton(PaletteTile_Click);
        ifTile = new IfTile(Slot_Click, Slot_DoubleClick);
        ifTile.SetupPaletteButton(PaletteTile_Click);
        codeAreaManager = new CodeAreaManager(Codearea);
        this.KeyDown += MainWindow_KeyDown; //key event attacher
        variables = new List<Variable>();
        sprites = new List<Sprite>();
        outputDragManager = new BlockDragManager(Outputarea);
        InitPalette();
        VariableGridUpdate();
    }
    // Event handler for key presses; from here each keypress calls a routine, pretty simple
    private void MainWindow_KeyDown(object sender, KeyEventArgs e) {
        
    }

    // PALETTE INITIALIZATION CODE
    private void InitPalette()
    {
        TilesPanel.Children.Add(assignmentTile.b);
        TilesPanel.Children.Add(forLoopTile.b);
        TilesPanel.Children.Add(moveSpriteTile.b);
        TilesPanel.Children.Add(ifTile.b);
    }
    // END OF PALETTE INITIALIZATION CODE

    // PALETTE TILE DRAG AND DROP CODE
    private void SetSelectedSlot(Slot slot)
    {
        if (selectedSlot == slot)
        {
            ClearSelectedSlot();
            return;
        }

        ClearSelectedSlot();
        selectedSlot = slot;
        if (selectedSlot != null)
        {
            selectedSlot.Button.BorderBrush = System.Windows.Media.Brushes.DodgerBlue;
            selectedSlot.Button.BorderThickness = new Thickness(2);
        }
    }

    private void ClearSelectedSlot()
    {
        if (selectedSlot != null)
        {
            selectedSlot.Button.BorderBrush = System.Windows.Media.Brushes.Gray;
            selectedSlot.Button.BorderThickness = new Thickness(1);
        }

        selectedSlot = null;
    }

    private void FillSelectedSlotWithNumber()
    {
        if (selectedSlot == null)
        {
            MessageBox.Show("Select the value slot first.");
            return;
        }

        if (selectedSlot.Type != SlotType.NumberOrVariable)
        {
            MessageBox.Show("This slot does not take a number.");
            return;
        }

        string input = Interaction.InputBox("Enter a number:", "Fixed Value", "");

        if (input == "")
        {
            return; // user cancelled or left it blank
        }

        if (double.TryParse(input, out double number))
        {
            selectedSlot.SetNumber(number);
        }
        else
        {
            MessageBox.Show("Enter a valid number.");
        }
    }
    private void PaletteTile_Click(object sender, RoutedEventArgs e)
    {
        Button clickedButton = sender as Button;
        if (clickedButton == null) return;

        Tile tile = clickedButton.Tag as Tile;
        if (tile == null) return;

        if (tile is Variable variable)
        {
            if (selectedSlot != null)
            {
                if (selectedSlot.Type == SlotType.VariableOnly ||
                    selectedSlot.Type == SlotType.NumberOrVariable)
                {
                    selectedSlot.SetVariable(variable);
                }
            }
            return;
        }

        if (tile is Sprite sprite)
        {
            if (selectedSlot != null && selectedSlot.Type == SlotType.SpriteOnly)
            {
                selectedSlot.SetSprite(sprite);
            }
            return;
        }

        codeAreaManager.AddTileBlock(tile);
    }
    private void Slot_Click(object sender, RoutedEventArgs e)
    {
        Button clickedButton = sender as Button;
        if (clickedButton == null) return;

        Slot clickedSlot = clickedButton.Tag as Slot;
        if (clickedSlot == null) return; 

        SetSelectedSlot(clickedSlot);
    }
    private void Slot_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        Button clickedButton = sender as Button;
        if (clickedButton == null) return;

        Slot clickedSlot = clickedButton.Tag as Slot;
        if (clickedSlot == null) return;

        if (clickedSlot.Type != SlotType.NumberOrVariable)
        {
            return;
        }

        SetSelectedSlot(clickedSlot);
        FillSelectedSlotWithNumber();
    }
    // END OF PALETTE TILE DRAG AND DROP CODE

    //RUN / STOP BUTTON CODE
    private void RunButton_Click(object sender, RoutedEventArgs e)
    {
        var b = (Button)sender;
        b.Background = System.Windows.Media.Brushes.LightGreen; // change the run button to green when it's running
        PaletteBorder.Visibility = Visibility.Collapsed;
        CodeBorder.Visibility = Visibility.Collapsed;

        Grid.SetColumn(OutputBorder, 0);
        Grid.SetColumnSpan(OutputBorder, 3);

        OutputBorder.Margin = new Thickness(10);

        // build a fresh evaluation context, register sprite movement callback,
        // give variables a default value, then run the code area's graph.
        var ctx = new EvaluationContext();
        ctx.OnMoveSprite = (name, dx, dy) =>
        {
            var sp = sprites.Find(s => s.Name == name);
            sp?.MoveBy(dx, dy);
        };
        foreach (var v in variables)
        {
            Value def;
            if (v is stringVariable) def = new Value("");
            else if (v is boolVariable) def = new Value(false);
            else def = new Value(0.0);
            ctx.Set(v.Name, def);
        }

        try
        {
            codeAreaManager.Run(ctx);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error while running: " + ex.Message);
        }

        if (ctx.Errors.Count > 0)
        {
            string msg = "";
            foreach (var (node, message) in ctx.Errors)
            {
                msg += node.Label + ": " + message + "\n";
            }
            MessageBox.Show(msg);
        }
    }
    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        RunButton.Background = System.Windows.Media.Brushes.Orange; // change the run button back to gray when it's stopped
        PaletteBorder.Visibility = Visibility.Visible;
        CodeBorder.Visibility = Visibility.Visible;

        Grid.SetColumn(OutputBorder, 2);
        Grid.SetColumnSpan(OutputBorder, 1);

        OutputBorder.Margin = new Thickness(5, 10, 10, 10);
    }
    // END OF RUN / STOP BUTTON CODE

    // VARIABLE CREATION UI CODE
    private void VariableGridUpdate()
    {
        VariablesPanel.Children.Clear();

        // show numbers, then strings, then booleans
        foreach (Variable v in variables)
        {
            if (v is numberVariable) AddVariableRow(v);
        }
        foreach (Variable v in variables)
        {
            if (v is stringVariable) AddVariableRow(v);
        }
        foreach (Variable v in variables)
        {
            if (v is boolVariable) AddVariableRow(v);
        }
    }
    private void AddVariableRow(Variable v)
    {
        v.SetupPaletteButton(PaletteTile_Click);

        // [variable button (fills row)] [X delete button]
        Grid row = new Grid { Margin = new Thickness(0, 1, 0, 1) };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        Grid.SetColumn(v.b, 0);
        row.Children.Add(v.b);

        Button del = new Button
        {
            Content = "X",
            Width = 25,
            Height = 30,
            Margin = new Thickness(2, 5, 5, 5),
            Background = System.Windows.Media.Brushes.Salmon
        };
        del.Click += (s, e) => DeleteVariable(v);
        Grid.SetColumn(del, 1);
        row.Children.Add(del);

        VariablesPanel.Children.Add(row);
    }
    private void DeleteVariable(Variable v)
    {
        variables.Remove(v);
        VariableGridUpdate();
    }
    private void NewVarButton_Click(object sender, RoutedEventArgs e) // the new variable button shows the menu we made
    {
        NewVarPanel.Visibility = Visibility.Visible;
    }
    private void SaveVarButton_Click(object sender, RoutedEventArgs e) // the save button makes a new variable
    {
        string varName = VarNameTextBox.Text.Trim(); // trim for whitespace
        if (string.IsNullOrWhiteSpace(varName)) // if name is missing
        {
            MessageBox.Show("Enter a variable name.");
            return;
        } else if (variables.Exists(v => v.Name == varName)) // if name is already taken
        {
            MessageBox.Show("Variable name already exists. Choose a different name.");
            return;
        }
        ComboBoxItem selectedItem = VarTypeComboBox.SelectedItem as ComboBoxItem; // dropdown menu for datatype
        string dataType = selectedItem.Content.ToString(); // assign datatype
        switch (dataType) // make the variable, add it to the list of variables, and close the menu
        {
            case "Number":
                variables.Add(new numberVariable(varName));
                break;
            case "String":
                variables.Add(new stringVariable(varName));
                break;
            case "Boolean":
                variables.Add(new boolVariable(varName));
                break;        
        }
        CancelVarButton_Click(sender, e);
    }
    private void CancelVarButton_Click(object sender, RoutedEventArgs e) // Cancel button, or close the menu down after a var is made
    {
        VarNameTextBox.Clear();
        VarTypeComboBox.SelectedIndex = 0;
        NewVarPanel.Visibility = Visibility.Collapsed;
        VariableGridUpdate(); // update the variable list on the side of the screen
    }
    // END OF VARIABLE CREATION UI CODE

    // SPRITE CREATION UI CODE
    private void SpriteGridUpdate()
    {
        SpritesPanel.Children.Clear();
        foreach (Sprite s in sprites)
        {
            AddSpriteRow(s);
        }
    }
    private void AddSpriteRow(Sprite sp)
    {
        sp.SetupPaletteButton(PaletteTile_Click);

        Grid row = new Grid { Margin = new Thickness(0, 1, 0, 1) };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        Grid.SetColumn(sp.b, 0);
        row.Children.Add(sp.b);

        Button del = new Button
        {
            Content = "X",
            Width = 25,
            Height = 30,
            Margin = new Thickness(2, 5, 5, 5),
            Background = System.Windows.Media.Brushes.Salmon
        };
        del.Click += (s, e) => DeleteSprite(sp);
        Grid.SetColumn(del, 1);
        row.Children.Add(del);

        SpritesPanel.Children.Add(row);
    }
    private void DeleteSprite(Sprite sp)
    {
        sprites.Remove(sp);
        if (sp.visual != null)
        {
            Outputarea.Children.Remove(sp.visual);
        }
        SpriteGridUpdate();
    }
    private void NewSpriteButton_Click(object sender, RoutedEventArgs e) // open the sprite menu
    {
        NewSpritePanel.Visibility = Visibility.Visible;
    }
    private void SaveSpriteButton_Click(object sender, RoutedEventArgs e) // make the sprite
    {
        string spriteName = SpriteNameTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(spriteName))
        {
            MessageBox.Show("Enter a sprite name.");
            return;
        }
        else if (sprites.Exists(s => s.Name == spriteName))
        {
            MessageBox.Show("Sprite name already exists. Choose a different name.");
            return;
        }

        // pick the shape
        ComboBoxItem shapeItem = SpriteShapeComboBox.SelectedItem as ComboBoxItem;
        SpriteShape shape;
        switch (shapeItem.Content.ToString())
        {
            case "Circle": shape = SpriteShape.Circle; break;
            case "Triangle": shape = SpriteShape.Triangle; break;
            default: shape = SpriteShape.Square; break;
        }

        // pick the color
        ComboBoxItem colorItem = SpriteColorComboBox.SelectedItem as ComboBoxItem;
        System.Windows.Media.Brush color;
        switch (colorItem.Content.ToString())
        {
            case "Blue": color = System.Windows.Media.Brushes.CornflowerBlue; break;
            case "Green": color = System.Windows.Media.Brushes.MediumSeaGreen; break;
            case "Yellow": color = System.Windows.Media.Brushes.Gold; break;
            default: color = System.Windows.Media.Brushes.IndianRed; break;
        }

        Sprite newSprite = new Sprite(spriteName, shape, color);
        sprites.Add(newSprite);

        // put the visual on the output area and make it draggable
        var visual = newSprite.CreateVisual();
        Canvas.SetLeft(visual, 50 + (sprites.Count * 10));
        Canvas.SetTop(visual, 50 + (sprites.Count * 10));
        Outputarea.Children.Add(visual);
        outputDragManager.Attach(visual);

        CancelSpriteButton_Click(sender, e);
    }
    private void CancelSpriteButton_Click(object sender, RoutedEventArgs e) // close the sprite menu
    {
        SpriteNameTextBox.Clear();
        SpriteShapeComboBox.SelectedIndex = 0;
        SpriteColorComboBox.SelectedIndex = 0;
        NewSpritePanel.Visibility = Visibility.Collapsed;
        SpriteGridUpdate();
    }
    // END OF SPRITE CREATION UI CODE
}
