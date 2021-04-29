using System;
using System.Windows.Forms;

namespace Chomp
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();

            numericUpDown1.Value = 10;
            numericUpDown2.Value = 10;

            numericUpDown1.ValueChanged += NumericUpDown1_ValueChanged;
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown2.Value = numericUpDown1.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var width = (int)numericUpDown1.Value;
            var height = (int)numericUpDown2.Value;

            var minDimension = 2;
            var maxDimension = 50;

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

            // Close();

            var game = new Form1(width, height);
            game.Show();
        }
    }
}
