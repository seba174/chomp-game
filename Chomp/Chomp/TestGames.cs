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
        private  Strategy? strategy1 = null;
        private  Strategy? strategy2 = null;

        public TestGames(int minWidth, int maxWidth, int minHeight, int maxHeight, int gamesCount, string str1, string str2)
        {
            InitializeComponent();

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
                this.strategy1 = Strategy.Random;
            if (str2 == "losowa")
                this.strategy2 = Strategy.Random;

            Text = $"Tryb testowy ({this.gamesCount} gier)";
        }

        private async void startButton_Click(object sender, EventArgs e)
        {
            gameLabel.Visible = true;
            startButton.Visible = false;

            var dict = new Dictionary<int, int>()
            {
                { 1, 0 },
                { 2, 0 }
            };

            var random = new Random();
            for (int i = 1; i <= gamesCount; i++)
            {
                await Task.Run(() =>
                {
                    var width = random.Next(minWidth, maxWidth);
                    var height = random.Next(minHeight, maxHeight);

                    var board = new Board
                    {
                        Width = width,
                        Height = height
                    };
                    var game = new Game(board, this.strategy1, this.strategy2);

                    while (!board.IsEndOfTheGame())
                    {
                        game.MakeMove();
                    }

                    var winner = game.MoveOfFirstPlayer.Value ? 2 : 1;
                    dict[winner]++;
                });
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
    }
}
