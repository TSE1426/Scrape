using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Scrape
{
    internal class TitleScreen // this is for saving / loading
    {
        public static void openProgram()
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("pack://application:,,,/TitleScreen.png");
            bitmap.EndInit(); // for images you have to load a bitmap first its weird
            Image titleimage = new Image
            {
                Source = bitmap,
                Width = 800,
                Height = 450,
                Stretch = Stretch.Fill
            };
            canv.Children.Clear();
            canv.Children.Add(titleimage);
            TitleScreenWindow.Content = canv;

            Button nButton = new Button()
            {
                Content = "New Program",
                Width = 150,
                Height = 50,
                Margin = new Thickness(630, 20, 0, 0) // right gap = 20
            };
            nButton.Click += NButton_Click;

            Button loadButton = new Button()
            {
                Content = "Load Program",
                Width = 150,
                Height = 50,
                Margin = new Thickness(470, 20, 0, 0)
                // second button shifted left by width + spacing
            };
            loadButton.Click += LoadButton_Click;

            canv.Children.Add(nButton);
            canv.Children.Add(loadButton);
            TitleScreenWindow.ShowDialog(); // This makes it wait until window is closed before going into the program.

        }
        public static string loadedText = ""; // this is for storing the loaded text so it can be used in the main window after the title screen is closed
        private static void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Load Program";
            dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (dlg.ShowDialog() == true)
            {
                string filePath = dlg.FileName;

                loadedText = File.ReadAllText(filePath);

                MessageBox.Show("Loaded:\n" + loadedText);

                TitleScreenWindow.Close(); // continue to main window
            }

        }

        private static void NButton_Click(object sender, RoutedEventArgs e)
        {
            TitleScreenWindow.Close();
        }

        private static Canvas canv = new Canvas
        {
            Width = 800,
            Height = 450
        };
        private static Window TitleScreenWindow = new Window
        {
            Width = 800,
            Height = 450,
            Content = canv
        };
    }
}
