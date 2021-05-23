using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Chomp.Form1;

namespace Chomp
{
    public partial class TestGames : Form
    {
        private readonly int minWidth;
        private readonly int maxWidth;
        private readonly int minHeight;
        private readonly int maxHeight;
        private readonly int gamesCount;
        private readonly Strategy? strategy1 = null;
        private readonly Strategy? strategy2 = null;

        public TestGames(int minWidth, int maxWidth, int minHeight, int maxHeight, int gamesCount, string str1, string str2)
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            gameLabel.Visible = false;
            player1Label.Visible = false;
            player2Label.Visible = false;
            closeButton.Visible = false;

            this.minWidth = minWidth;
            this.maxWidth = maxWidth;
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
            this.gamesCount = gamesCount;

            if (str1 == "losowa")
            {
                strategy1 = Strategy.Random;
            }

            if (str2 == "losowa")
            {
                strategy2 = Strategy.Random;
            }

            Text = $"Tryb testowy ({this.gamesCount} gier)";
        }

        private async void startButton_Click(object sender, EventArgs e)
        {
            gameLabel.Visible = true;
            UpdateGameLabel(0);
            startButton.Visible = false;

            Dictionary<int, int> dict = new()
            {
                { 1, 0 },
                { 2, 0 }
            };

            Random random = new();
            for (int i = 1; i <= gamesCount; i++)
            {
                await Task.Run(async () =>
                {
                    int width = random.Next(minWidth, maxWidth);
                    int height = random.Next(minHeight, maxHeight);

                    Board board = new()
                    {
                        Width = width,
                        Height = height
                    };
                    Game game = new(board, strategy1, strategy2);

                    while (!board.IsEndOfTheGame())
                    {
                        await game.MakeMoveAsync();
                    }

                    int winner = game.MoveOfFirstPlayer.Value ? 2 : 1;
                    dict[winner]++;
                });

                if (i % 5 == 0)
                {
                    UpdateGameLabel(i);
                }
            }

            // So that text won't flash when calculations are super fast
            await Task.Delay(500);

            gameLabel.Visible = false;
            closeButton.Visible = true;
            player1Label.Visible = true;
            player2Label.Visible = true;

            player1Label.Text = $"Gracz 1 wygrał {dict[1]} gier";
            player2Label.Text = $"Gracz 2 wygrał {dict[2]} gier";
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdateGameLabel(int currentProgress)
        {
            gameLabel.Text = $"Trwa gra obliczanie gier (zakończono {currentProgress}/{gamesCount})...";
        }
    }
}
