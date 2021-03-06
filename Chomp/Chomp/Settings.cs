using System;
using System.Windows.Forms;

namespace Chomp
{
    public partial class Settings : Form
    {
        private const int minDimension = 2;
        private const int maxDimension = 20;
        private const int maxGames = 100;

        public Settings()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            strategy_demo_1.SelectedIndex = 0;
            strategy_demo_2.SelectedIndex = 0;
            strategy_test_1.SelectedIndex = 0;
            strategy_test_2.SelectedIndex = 0;

            numericUpDown1.Value = 10;
            numericUpDown2.Value = 10;

            minWidthControl.Value = 10;
            maxWidthControl.Value = 10;
            minHeightControl.Value = 10;
            maxHeightControl.Value = 10;
            gamesCountControl.Value = 10;

            numericUpDown1.Maximum = maxDimension;
            numericUpDown2.Maximum = maxDimension;
            minWidthControl.Maximum = maxDimension;
            maxWidthControl.Maximum = maxDimension;
            minHeightControl.Maximum = maxDimension;
            maxHeightControl.Maximum = maxDimension;

            numericUpDown1.Minimum = minDimension;
            numericUpDown2.Minimum = minDimension;
            minWidthControl.Minimum = minDimension;
            maxWidthControl.Minimum = minDimension;
            minHeightControl.Minimum = minDimension;
            maxHeightControl.Minimum = minDimension;

            gamesCountControl.Maximum = 100;
            gamesCountControl.Minimum = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int width = (int)numericUpDown1.Value;
            int height = (int)numericUpDown2.Value;
            string str_1 = (string)strategy_demo_1.SelectedItem;
            string str_2 = (string)strategy_demo_2.SelectedItem;
            int depth_1 = (int)numericDepthPlayer1_demo.Value;
            int depth_2 = (int)numericDepthPlayer2_demo.Value;

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

            var game = new Form1(width, height, str_1, str_2, depth_1, depth_2);
            game.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int minWidth = (int)minWidthControl.Value;
            int maxWidth = (int)maxWidthControl.Value;
            int minHeight = (int)minHeightControl.Value;
            int maxHeight = (int)maxHeightControl.Value;
            int gamesCount = (int)gamesCountControl.Value;
            string str_1 = (string)strategy_test_1.SelectedItem;
            string str_2 = (string)strategy_test_2.SelectedItem;
            int depth_1 = (int)numericDepthPlayer1_test.Value;
            int depth_2 = (int)numericDepthPLayer2_test.Value;

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

            var game = new TestGames(minWidth, maxWidth, minHeight, maxHeight, gamesCount, str_1, str_2, depth_1, depth_2);
            game.Show();
        }
    }
}
