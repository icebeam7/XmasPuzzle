using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace XmasPuzzle
{
    public class PuzzleTile : ContentView
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public string Tag { get; set; }

        public PuzzleTile(int row, int col, ImageSource imageSource)
        {
            Row = row;
            Col = col;
            Tag = $"{row}_{col}";
            Padding = new Thickness(1);
            Content = new Image
            {
                Source = imageSource
            };
        }
    }
}
