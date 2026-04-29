using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.VisualBasic;

namespace Scrape;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private AssignmentTile assignmentTile;
    private Slot selectedSlot = null; // Selected slot for variables to go into in the code
    private List<Variable> variables; // list of variables, will be used to store variables created by the user
    private CodeAreaManager codeAreaManager;
    
    public MainWindow()
    {
        InitializeComponent();
        assignmentTile = new AssignmentTile(Slot_Click, Slot_DoubleClick);
        assignmentTile.SetupPaletteButton(PaletteTile_Click);
        codeAreaManager = new CodeAreaManager(Codearea);
        this.KeyDown += MainWindow_KeyDown; //key event attacher
        variables = new List<Variable>();
        InitPalette();
        VariableGridUpdate();
    }
    // Event handler for key presses; from here each keypress calls a routine, pretty simple
    private void MainWindow_KeyDown(object sender, KeyEventArgs e) {
        
    }

    // PALETTE INITIALIZATION CODE
    private void InitPalette()
    {
        Grid.SetColumn(assignmentTile.b, 0);
        Grid.SetRow(assignmentTile.b, 2);    
        TilePalette.Children.Add(assignmentTile.b);      
    }
    // END OF PALETTE INITIALIZATION CODE

    // PALETTE TILE DRAG AND DROP CODE
    private void FillSelectedSlotWithNumber()
    {
        if (selectedSlot == null)
        {
            MessageBox.Show("Select the value slot first.");
            return;
        }

        if (selectedSlot.Type != SlotType.NumberOrVariable)
        {
            MessageBox.Show("Only the second slot can take a fixed number.");
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
            selectedSlot = null;
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
                    selectedSlot = null;
                }
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

        selectedSlot = clickedSlot;
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

        selectedSlot = clickedSlot;
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
    private void clearVarColumn()
    {
        // Remove old variable buttons from column 2 (3rd column)
        for (int i = TilePalette.Children.Count - 1; i >= 0; i--)
        {
            UIElement child = TilePalette.Children[i];

            if (child is Button button && Grid.GetColumn(button) == 2)
            {
                TilePalette.Children.RemoveAt(i);
            }
        }
    }
    private void VariableGridUpdate()
    {
        foreach (Variable v in variables)
        {
            TilePalette.Children.Remove(v.b);
        }
        int row = 2;
        foreach (Variable v in variables)
        {
            if (v is numberVariable)
            {
                v.SetupPaletteButton(PaletteTile_Click);
                Grid.SetColumn(v.b, 2);
                Grid.SetRow(v.b, row++);
                TilePalette.Children.Add(v.b);
            }
        }
        foreach (Variable v in variables)
        {
            if (v is stringVariable)
            {
                v.b.Tag = v;
                v.b.Click -= PaletteTile_Click;
                v.b.Click += PaletteTile_Click;

                Grid.SetColumn(v.b, 2);
                Grid.SetRow(v.b, row++);
                TilePalette.Children.Add(v.b);
            }
        }
        foreach (Variable v in variables)
        {
            if (v is boolVariable)
            {
                v.b.Tag = v;
                v.b.Click -= PaletteTile_Click;
                v.b.Click += PaletteTile_Click;

                Grid.SetColumn(v.b, 2);
                Grid.SetRow(v.b, row++);
                TilePalette.Children.Add(v.b);
            }
        }

        
    }
    private void NewVarButton_Click(object sender, RoutedEventArgs e) // the new variable button shows the menu we made
    {
        clearVarColumn();
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
}
