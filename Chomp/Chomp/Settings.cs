using System;
using System.Windows.Forms;

namespace Chomp
{
    public partial class Settings : Form
    {
        private const int minDimension = 2;
        private const int maxDimension = 50;
        private const int maxGames = 100;

        public Settings()
        {
            InitializeComponent();

            numericUpDown1.Value = 10;
            numericUpDown2.Value = 10;

            minWidthControl.Value = 10;
            maxWidthControl.Value = 10;
            minHeightControl.Value = 10;
            maxHeightControl.Value = 10;
            gamesCountControl.Value = 10;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var width = (int)numericUpDown1.Value;
            var height = (int)numericUpDown2.Value;

            if (width < minDimension || height < minDimension)
            {
                MessageBox.Show($"Minimalna szerokość i wysokość planszy to {minDimension}");
                return;
            }

            if (width > maxDimension || height > maxDimension)
            {
                MessageBox.Show($"Maksymalna szerokość i wysokość planszy to {maxDimension}");
                return;
            }

            var game = new Form1(width, height);
            game.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var minWidth = (int)minWidthControl.Value;
            var maxWidth = (int)maxWidthControl.Value;
            var minHeight = (int)minHeightControl.Value;
            var maxHeight = (int)maxHeightControl.Value;
            var gamesCount = (int)gamesCountControl.Value;

            if (minWidth > maxWidth)
            {
                MessageBox.Show($"Minimalna szerokość nie może być większa niż maksymalna szerokość");
                return;
            }

            if (minHeight > maxHeight)
            {
                MessageBox.Show($"Minimalna wysokość nie może być większa niż maksymalna wysokość");
                return;
            }

            if (minWidth < minDimension || minHeight < minDimension)
            {
                MessageBox.Show($"Minimalna szerokość i wysokość planszy to {minDimension}");
                return;
            }

            if (maxWidth > maxDimension || maxHeight > maxDimension)
            {
                MessageBox.Show($"Maksymalna szerokość i wysokość planszy to {maxDimension}");
                return;
            }

            if (gamesCount < 1 || gamesCount > maxGames)
            {
                MessageBox.Show($"Liczba gier powinna być z przedziału [1, {maxGames}]");
            }

            var game = new TestGames(minWidth, maxWidth, minHeight, maxHeight, gamesCount);
            game.Show();
        }
    }
}
