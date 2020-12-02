using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XmasPuzzle
{
    public partial class MainPage : ContentPage
    {
        // Number of tiles horizontally and vertically,
        static readonly int NUM = 4;

        // Array of tiles, and empty row & column.
        PuzzleTile[,] tiles = new PuzzleTile[NUM, NUM];
        int emptyRow = NUM - 1;
        int emptyCol = NUM - 1;

        double tileSize;
        bool isBusy;

        void SetupPuzzle()
        {
            for (int row = 0; row < NUM; row++)
            {
                for (int col = 0; col < NUM; col++)
                {
                    if (row == NUM - 1 && col == NUM - 1)
                        break;

                    var imageName = $"XmasPuzzle.Images.Picture{row}{col}.jpg";
                    var imageSource = ImageSource.FromResource(imageName);

                    var tile = new PuzzleTile(row, col, imageSource);

                    // Add tap recognition.
                    var tapGestureRecognizer = new TapGestureRecognizer
                    {
                        Command = new Command(OnTileTapped),
                        CommandParameter = tile
                    };
                    tile.GestureRecognizers.Add(tapGestureRecognizer);

                    tiles[row, col] = tile;
                    absoluteLayout.Children.Add(tile);
                }
            }

        }

        public MainPage()
        {
            InitializeComponent();

            SetupPuzzle();
        }

        private void ContentView_SizeChanged(object sender, EventArgs e)
        {
            var contentView = (ContentView)sender;
            var width = contentView.Width;
            var height = contentView.Height;

            if (width <= 0 || height <= 0)
                return;

            stackLayout.Orientation = (width < height)
                ? StackOrientation.Vertical
                : StackOrientation.Horizontal;

            tileSize = Math.Min(width, height) / NUM;
            absoluteLayout.WidthRequest = NUM * tileSize;
            absoluteLayout.HeightRequest = NUM * tileSize;

            foreach (var view in absoluteLayout.Children)
            {
                var tile = (PuzzleTile)view;

                var rect = new Rectangle(tile.Col * tileSize, tile.Row * tileSize, tileSize, tileSize);
                AbsoluteLayout.SetLayoutBounds(tile, rect);
            }

        }

        async void OnTileTapped(object parameter)
        {
            if (isBusy)
                return;

            isBusy = true;

            var tappedTile = (PuzzleTile)parameter;
            await ShiftIntoEmpty(tappedTile.Row, tappedTile.Col);
            await CheckWinner();

            isBusy = false;
        }

        async Task ShiftIntoEmpty(int tappedRow, int tappedCol, uint length = 100)
        {
            if (tappedRow == emptyRow && tappedCol != emptyCol)
            {
                var inc = Math.Sign(tappedCol - emptyCol);
                var begCol = emptyCol + inc;
                var endCol = tappedCol + inc;

                for (int col = begCol; col != endCol; col += inc)
                    await AnimateTile(emptyRow, col, emptyRow, emptyCol, length);
            }
            else if (tappedCol == emptyCol && tappedRow != emptyRow)
            {
                var inc = Math.Sign(tappedRow - emptyRow);
                var begRow = emptyRow + inc;
                var endRow = tappedRow + inc;

                for (int row = begRow; row != endRow; row += inc)
                    await AnimateTile(row, emptyCol, emptyRow, emptyCol, length);
            }
        }


        async Task AnimateTile(int row, int col, int newRow, int newCol, uint length)
        {
            var animaTile = tiles[row, col];

            var rect = new Rectangle(emptyCol * tileSize, emptyRow * tileSize, tileSize, tileSize);
            await animaTile.LayoutTo(rect, length);
            AbsoluteLayout.SetLayoutBounds(animaTile, rect);

            tiles[newRow, newCol] = animaTile;
            animaTile.Row = newRow;
            animaTile.Col = newCol;

            tiles[row, col] = null;
            emptyRow = row;
            emptyCol = col;
        }

        async Task CheckWinner()
        {
            bool isWinner = true;

            for (int row = 0; row < NUM; row++)
            {
                for (int col = 0; col < NUM; col++)
                {
                    if (tiles[row, col] == null)
                    {
                        if (row != NUM - 1 || col != NUM - 1)
                        {
                            isWinner = false;
                        }

                        break;
                    }

                    if (tiles[row, col].Tag != $"{row}_{col}")
                    {
                        isWinner = false;
                        break;
                    }
                }
            }

            if (isWinner)
                await DisplayAlert("Congrats!", "Winner", "OK");
        }

        async void OnRandomizeButtonClicked(object sender, EventArgs args)
        {
            var button = (Button)sender;
            button.IsEnabled = false;

            var rand = new Random();

            isBusy = true;

            for (int i = 0; i < 1; i++)
            {
                await ShiftIntoEmpty(rand.Next(NUM), emptyCol, 25);
                await ShiftIntoEmpty(emptyRow, rand.Next(NUM), 25);
            }

            button.IsEnabled = true;

            isBusy = false;
        }

    }
}
