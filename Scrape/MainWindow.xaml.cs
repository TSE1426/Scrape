using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Scrape;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    Canvas mycanv = new Canvas();

    public MainWindow()
    {
        
        InitializeComponent();      
        Content = mycanv;
        reDrawScreen();
        this.KeyDown += MainWindow_KeyDown;
    }

    public void reDrawScreen()
    {
        mycanv.Children.Clear();
        Random RND = new Random();
        List<TextBlock> textblocks = new List<TextBlock>();
        for (int i = 0; i < 100; i++)
        {
            textblocks.Add(new TextBlock());
            textblocks[i].Text = "Hello, World!";
            Canvas.SetLeft(textblocks[i], RND.Next(0, (int)Width));
            Canvas.SetTop(textblocks[i], RND.Next(0, (int)Height));

            mycanv.Children.Add(textblocks[i]);
        }
    }

    private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        reDrawScreen();
    }
}
