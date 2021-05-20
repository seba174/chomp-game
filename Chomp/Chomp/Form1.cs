using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Chomp
{
    public partial class Form1 : Form
    {
        private Bitmap basicBoard;
        private Board board;
        private Game game;

        public class Board
        {
            public int Width { get; set; }
            public int Height { get; set; }


            public HashSet<(int x, int y)> ChoosenFields { get; set; } = new HashSet<(int x, int y)>();
            public List<(int x, int y)> MovesList { get; set; } = new List<(int x, int y)>();


            public void MakeMove(int x, int y)
            {
                MovesList.Add((x, y));
                MarkFieldsAsChoosen(x, y);
            }

            private void MarkFieldsAsChoosen(int x, int y)
            {
                for (int i = x; i < Width; i++)
                {
                    for (int j = y; j < Height; j++)
                    {
                        ChoosenFields.Add((i, j));
                    }
                }
            }

            public bool IsEndOfTheGame()
            {
                return ChoosenFields.Count == (Width * Height - 1);
            }
        }

        public class Game
        {
            private Board _board;
            private Player _player1;
            private Player _player2;


            public bool? MoveOfFirstPlayer { get; set; }

            public Game(Board board)
            {
                _board = board;
                _player1 = new Player(_board);
                _player2 = new Player(_board);
            }


            public void MakeMove()
            {
                if (MoveOfFirstPlayer == null || MoveOfFirstPlayer.Value)
                {
                    _player1.MakeMove();
                    MoveOfFirstPlayer = false;
                    return;
                }

                _player2.MakeMove();
                MoveOfFirstPlayer = !MoveOfFirstPlayer;
            }

        }

        public class Player
        {
            private Board _board;
            private readonly Strategy strategy;
            private bool? amIFirstPlayer;


            public Player(Board board)
            {
                _board = board;
                strategy = GetStrategy();

            }

            public void MakeMove()
            {
                if (amIFirstPlayer == null)
                {
                    amIFirstPlayer = _board.MovesList.Count == 0;
                }

                // second player hasnt winning strategy, so always same move
                if (!amIFirstPlayer.Value)
                {
                    Make_NM_Move();
                    return;
                }

                // first player strategies
                switch (strategy)
                {
                    case Strategy.NN:
                        Make_NN_Move();
                        break;
                }
            }

            private void Make_NN_Move()
            {
                if (_board.MovesList.Count == 0)
                {
                    _board.MakeMove(1, 1);
                }
                else
                {
                    var lastMoveOfSecondPlayer = _board.MovesList.Last();
                    _board.MakeMove(lastMoveOfSecondPlayer.y, lastMoveOfSecondPlayer.x);
                }
            }

            private void Make_NM_Move()
            {
                // for now random move

                HashSet<(int x, int y)> allPossibbleMoves = new HashSet<(int, int)>();

                for (int i = 0; i < _board.Width; i++)
                {
                    for (int j = 0; j < _board.Height; j++)
                    {
                        if (i == 0 && j == 0)
                        {
                            continue;
                        }
                        allPossibbleMoves.Add((i, j));
                    }
                }

                foreach (var pair in _board.ChoosenFields)
                {
                    allPossibbleMoves.Remove(pair);
                }


                var random = new Random();
                var choosenMoveIndex = random.Next(0, allPossibbleMoves.Count - 1);
                var choosenMove = allPossibbleMoves.Skip(choosenMoveIndex).FirstOrDefault();

                _board.MakeMove(choosenMove.x, choosenMove.y);

            }

            private Strategy GetStrategy()
            {
                if (_board.Width == _board.Height)
                {
                    return Strategy.NN;
                }

                if (_board.Width == 2 || _board.Height == 2)
                {
                    return Strategy.TwoN;
                }

                return Strategy.NM;
            }
        }

        public enum Strategy
        {
            TwoN,
            NN,
            NM
        }

        public Form1(int width, int height)
        {
            InitializeComponent();
            closeButton.Visible = false;

            board = new Board
            {
                Width = width,
                Height = height
            };
            game = new Game(board);

            UpdateBoard();
            UpdateLabel();
        }

        private void UpdateAfterPlayerMove()
        {
            UpdateBoard();
            UpdateLabel();

            if (board.IsEndOfTheGame())
            {
                var winner = game.MoveOfFirstPlayer.Value ? 2 : 1;
                button1.Enabled = false;
                label1.Text = $"Wygrał gracz: {winner}";
                closeButton.Visible = true;
            }
        }

        private void UpdateBoard()
        {
            int widthOfTile = pictureBox1.Size.Width / board.Width;
            int heightOfTile = pictureBox1.Size.Height / board.Height;

            basicBoard = new Bitmap(widthOfTile * board.Width, heightOfTile * board.Height);

            using (var graphics = Graphics.FromImage(basicBoard))
            {
                SolidBrush brushGoal = new SolidBrush(Color.SaddleBrown);
                SolidBrush choosenField = new SolidBrush(Color.Gray);


                SolidBrush brush = new SolidBrush(Color.White);
                Pen penBorder = new Pen(Color.Black, 2);

                for (int x = 0; x < board.Width; x++)
                {
                    for (int y = 0; y < board.Height; y++)
                    {

                        graphics.FillRectangle(board.ChoosenFields.Contains((x, y)) ? choosenField : brushGoal, new Rectangle(x * widthOfTile, y * heightOfTile, widthOfTile, heightOfTile));
                        graphics.DrawRectangle(penBorder, new Rectangle(x * widthOfTile, y * heightOfTile, widthOfTile, heightOfTile));
                    }
                }

                SolidBrush brushLastPiece = new SolidBrush(Color.Red);
                graphics.FillRectangle(brushLastPiece, new Rectangle(0, 0, widthOfTile, heightOfTile));
                graphics.DrawRectangle(penBorder, new Rectangle(0, 0, widthOfTile, heightOfTile));
            }

            pictureBox1.Image = basicBoard;
            pictureBox1.Refresh();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            game.MakeMove();
            UpdateAfterPlayerMove();
        }

        private void UpdateLabel()
        {
            if (game.MoveOfFirstPlayer is null || game.MoveOfFirstPlayer.Value)
            {
                label1.Text = "Ruch gracza: 1";
            }
            else
            {
                label1.Text = "Ruch gracza: 2";
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
