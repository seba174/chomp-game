using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chomp
{
    public partial class Form1 : Form
    {
        private Bitmap basicBoard;
        private readonly Board board;
        private readonly Game game;


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


            public int GetHorizontalLastField(int y = 0)
            {
                int count = 0;
                while (count < this.Width - 1 && !this.ChoosenFields.Contains((count + 1, y)))
                    count++;
                return count;
            }

            public int GetVerticalLastField(int x = 0)
            {
                int count = 0;
                while (count < this.Height - 1 && !this.ChoosenFields.Contains((x, count + 1)))
                    count++;
                return count;
            }

            public bool IsRemainedNxNBoard()
            {
                if (Height == Width)
                    return true;
                if (ChoosenFields.Count == 0)
                    return Height == Width;
                if (ChoosenFields.Contains((1, 1)))
                    return true;
                int i = 0;

                if (GetVerticalLastField() == GetHorizontalLastField())
                {
                    return true;
                }
                //while (i < Width && i < Height)
                //{
                //    if (ChoosenFields.Contains((i, i)))
                //    {
                //        for (int j = i - 1; j >= 0; j--)
                //        {
                //            if (!ChoosenFields.Contains((j, i)) || !ChoosenFields.Contains((i, j)))
                //            {
                //                return false;
                //            }

                //        }
                //        return true;
                //    }
                //    i++;
                //}

                return false;
            }

            public bool IsWinningNxNBoard()
            {
                if (ChoosenFields.Contains((1, 1)))
                {
                    if (GetVerticalLastField() == GetHorizontalLastField())
                    {
                        return false;
                    }
                    return true;
                }

                if (GetVerticalLastField() == GetHorizontalLastField())
                {
                    return true;
                }
                return false;
            }

            public bool IsLosingNxNBoard()
            {
                if (ChoosenFields.Contains((1, 1)))
                {
                    if (GetVerticalLastField() == GetHorizontalLastField())
                    {
                        return true;
                    }
                    return false;
                }

                return false;
            }


            public bool IsRemained2xNBoard()
            {
                if (Height == 2 || Width == 2)
                    return true;
                if (ChoosenFields.Count == 0)
                    return Height == 2 || Width == 2;
                if (ChoosenFields.Contains((2, 0)))
                    return true;
                if (ChoosenFields.Contains((0, 2)))
                    return true;
                return false;
                //for (int i = 0; i < Height; i++)
                //    if (!ChoosenFields.Contains((2, i)))
                //    {
                //        for (int j = 0; j < Width; j++)
                //            if (!ChoosenFields.Contains((j, 2)))
                //                return false;
                //        return true;
                //    }
                //return true;
            }

            public bool IsWinning2xNBoard()
            {
                if (Width == 2 || ChoosenFields.Contains((2, 0)))
                  {
                    var max0 = GetVerticalLastField();
                    var max1 = GetVerticalLastField(1);
                    if (max0 == max1 + 1)
                        return false;
                    return true;
                  }
                if (Height == 2 || ChoosenFields.Contains((0, 2)))
                {
                    var max0 = GetHorizontalLastField();
                    var max1 = GetHorizontalLastField(1);
                    if (max0 == max1 + 1)
                        return false;
                    return true;
                }
                return false;
            }


            public bool IsLosing2xNBoard()
            {
                if (Width == 2 || ChoosenFields.Contains((2, 0)))
                {
                    var max0 = GetVerticalLastField();
                    var max1 = GetVerticalLastField(1);
                    if (max0 == max1 + 1)
                        return true;
                    return false;
                }
                if (Height == 2 || ChoosenFields.Contains((0, 2)))
                {
                    var max0 = GetHorizontalLastField();
                    var max1 = GetHorizontalLastField(1);
                    if (max0 == max1 + 1)
                        return true;
                    return false;
                }
                return false;
            }

            public bool IsAvailableMove(int x, int y)
            {
                if (x >= Width || y >= Height)
                {
                    return false;
                }

                return !ChoosenFields.Contains((x, y));
            }

            public Board MakeNextMove((int x, int y) move)
            {
                var newBoard = new Board()
                {
                    Width = Width,
                    Height = Height,
                    MovesList = new List<(int x, int y)>(MovesList),
                    ChoosenFields = new HashSet<(int x, int y)>(ChoosenFields)
                };

                newBoard.MakeMove(move.x, move.y);
                return newBoard;
            }

            public bool IsEndOfTheGame()
            {
                return ChoosenFields.Count == (Width * Height - 1);
            }

            public bool IsMoveOfFirstPlayer()
            {
                return MovesList.Count % 2 == 0;
            }

            public HashSet<(int x, int y)> GetAllPossibleMoves()
            {
                HashSet<(int x, int y)> allPossibbleMoves = new(Width * Height - 1 - ChoosenFields.Count);

                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        if (i == 0 && j == 0)
                        {
                            continue;
                        }

                        if (!ChoosenFields.Contains((i, j)))
                        {
                            allPossibbleMoves.Add((i, j));
                        }
                    }
                }

                return allPossibbleMoves;
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
        }

        public class Game
        {
            private readonly Board _board;
            private readonly Player _player1;
            private readonly Player _player2;

            public Player GetPlayerTurn()
            {
                return MoveOfFirstPlayer.HasValue ? MoveOfFirstPlayer.Value ? _player1 : _player2 : null;
            }

            public bool? MoveOfFirstPlayer { get; set; }

            public Game(Board board, Strategy? strategy1, Strategy? strategy2, int depth1=4, int depth2=4)
            {
                _board = board;
                _player1 = new Player(_board, strategy1, depth1);
                _player2 = new Player(_board, strategy2, depth2);
            }

            public async Task MakeMoveAsync()
            {
                if (MoveOfFirstPlayer == null || MoveOfFirstPlayer.Value)
                {
                    await _player1.MakeMoveAsync();
                    MoveOfFirstPlayer = false;
                    return;
                }

                await _player2.MakeMoveAsync();
                MoveOfFirstPlayer = !MoveOfFirstPlayer;
            }
        }

        public class Player
        {
            private readonly Board _board;
            private readonly AlphaBetaStrategy _alphaBeta;
            private readonly Strategy strategy;
            private bool? amIFirstPlayer;

            public Player(Board board, Strategy? forcedStrategy, int depth = 4)
            {
                _board = board;
                strategy = forcedStrategy == null ? GetStrategy() : (Strategy)forcedStrategy;
                _alphaBeta = new AlphaBetaStrategy(depth, SimpleEvaluator.Evaluate);
            }

            public async Task MakeMoveAsync()
            {
                if (amIFirstPlayer == null)
                {
                    amIFirstPlayer = _board.MovesList.Count == 0;
                }

                switch (strategy)
                {
                    case Strategy.NN:
                        //Make_NN_Move();
                        await Make_NM_MoveAsync();
                        break;
                    case Strategy.TwoN:
                        //Make_TwoN_Move();
                        await Make_NM_MoveAsync();
                        break;
                    case Strategy.NM:
                        await Make_NM_MoveAsync();
                        break;
                    case Strategy.Random:
                        Make_Random_Move();
                        break;
                    case Strategy.Semirandom:
                        Make_Semirandom_Move();
                        break;

                }
            }

            private void Make_NN_Move()
            {
                if (_board.IsAvailableMove(1, 1))
                {
                    _board.MakeMove(1, 1);
                }
                else
                {
                    var lastX = _board.GetHorizontalLastField();
                    var lastY = _board.GetVerticalLastField();
                    if (lastX != lastY)
                    {
                        if (lastX < lastY)
                            _board.MakeMove(0, lastX + 1);
                        else
                            _board.MakeMove(lastY + 1, 0);
                    }
                    else
                    {
                        Make_Random_Move();
                        //var lastMoveOfSecondPlayer = _board.MovesList.Last();
                        //if (_board.IsAvailableMove(lastMoveOfSecondPlayer.y, lastMoveOfSecondPlayer.x))
                        //    _board.MakeMove(lastMoveOfSecondPlayer.y, lastMoveOfSecondPlayer.x);
                        //else
                        //    Make_Random_Move();
                    }
                }
            }


            private async Task Make_NM_MoveAsync()
            {
                int player_to_maximize = _board.IsMoveOfFirstPlayer() ? 1 : 2;
                var allPossibbleMoves = await Task.Run(() =>
                {
                    return _alphaBeta.GetBestMoves(_board, player_to_maximize);
                });

                var random = new Random();
                int choosenMoveIndex = random.Next(0, allPossibbleMoves.Count() - 1);
                (int x, int y) = allPossibbleMoves.Skip(choosenMoveIndex).FirstOrDefault();

                _board.MakeMove(x, y);
            }

            private void Make_Random_Move()
            {
                var allPossibbleMoves = _board.GetAllPossibleMoves();

                var random = new Random();
                int choosenMoveIndex = random.Next(0, allPossibbleMoves.Count - 1);
                (int x, int y) = allPossibbleMoves.Skip(choosenMoveIndex).FirstOrDefault();

                _board.MakeMove(x, y);
            }

            private void Make_Semirandom_Move()
            {
                if (_board.IsRemainedNxNBoard())
                {
                    Make_NN_Move();
                    return;
                }
                else if (_board.IsRemained2xNBoard())
                {
                    Make_TwoN_Move();
                    return;
                }
                else
                {
                    Make_Random_Move();
                }

            }

            private class SimpleEvaluator
            {
                public const double winValue = 100000;

                public static double Evaluate(Board board)
                {
                    if (board.IsEndOfTheGame())
                    {
                        return -winValue;
                    }

                    var possibleMoves = board.GetAllPossibleMoves();
                    if (possibleMoves.Count == 1)
                    {
                        return winValue;
                    }

                    if (board.IsWinningNxNBoard() || board.IsWinning2xNBoard())
                    {
                        return winValue;
                    }

                    if (board.IsLosing2xNBoard() || board.IsLosingNxNBoard())
                    {
                        return -winValue;
                    }


                    int goodMoves = 0;
                    foreach ((int x, int y) move in possibleMoves)
                    {
                        Board newBoard = board.MakeNextMove(move);
                        if (!IsBadPosition(board))
                        {
                            goodMoves++;
                        }
                    }
                    return goodMoves;
                }

                private static bool IsBadPosition(Board board)
                {
                    return false;
                    //return board.ChoosenFields.Count % 2 == 1;
                }
            }

            private class AlphaBetaStrategy
            {
                private int _maxDepth { get; set; }
                private Func<Board, double> _evaluation { get; set; }
                private double _eps { get; set; }

                private const double INIT_VALUE = 50000000;

                public AlphaBetaStrategy(int max_depth, Func<Board, double> evaluation, double eps = 0)
                {
                    _maxDepth = max_depth;
                    _evaluation = evaluation;
                    _eps = eps;
                }

                public IEnumerable<(int x, int y)> GetBestMoves(Board board, int player_to_maximize)
                {
                    var allPossibbleMoves = board.GetAllPossibleMoves();

                    double alpha = -INIT_VALUE; //double.MinValue;
                    double beta = INIT_VALUE; //double.MaxValue;

                    var moves = new List<((int x, int y), double)>();


                    foreach ((int x, int y) move in allPossibbleMoves)
                    {
                        double value = EvalMove(board, move, _maxDepth, alpha, beta, is_first_step: true, player_to_maximize);
                        moves.Add((move, value));
                        alpha = Math.Max(alpha, value);
                    }

                    var bestMoves = moves.Where(x => x.Item2 == alpha).Select(x => x.Item1).ToList();
                    return bestMoves.Any() ? bestMoves : allPossibbleMoves;
                }

                private double EvalMove(Board board, (int x, int y) move, int depth, double alpha, double beta, bool is_first_step, int player_to_maximize)
                {
                    var newBoard = board.MakeNextMove(move);
                    return AlphaBeta(newBoard, depth - 1, alpha, beta, is_first_step, player_to_maximize);
                }

                private double AlphaBeta(Board board, int depth, double alpha, double beta, bool is_first_step, int player_to_maximize)
                {
                    int currentPlayerTurn = board.IsMoveOfFirstPlayer() ? 1 : 2;
                    bool is_maximizing = currentPlayerTurn == player_to_maximize;

                    var posible_moves = board.GetAllPossibleMoves();
                    bool is_game_over = board.IsEndOfTheGame();

                    double value2 = _evaluation(board);


                    if (is_game_over || depth == 0 || Math.Abs(value2) == SimpleEvaluator.winValue) 
                    {
                        if (!is_maximizing)
                        {
                            value2 *= -1;
                        }
                        return value2;
                    }

                    if (is_maximizing)
                    {
                        double value = -INIT_VALUE;
                        foreach ((int x, int y) move in posible_moves)
                        {
                            value = Math.Max(value, EvalMove(board, move, depth, alpha, beta, false, player_to_maximize));
                            alpha = Math.Max(alpha, value);
                            if (ShouldCut(alpha, beta, _eps, is_first_step))
                            {
                                break;
                            }
                        }
                        return value;
                    }
                    else
                    {
                        double value = INIT_VALUE;
                        foreach ((int x, int y) move in posible_moves)
                        {
                            value = Math.Min(value, EvalMove(board, move, depth, alpha, beta, false, player_to_maximize));
                            beta = Math.Min(beta, value);
                            if (ShouldCut(alpha, beta, _eps, is_first_step))
                            {
                                break;
                            }
                        }
                        return value;
                    }
                }

                private static bool ShouldCut(double alpha, double beta, double eps, bool is_first_step)
                {
                    //return false;
                    if (is_first_step)
                    {
                        if (Compare(alpha, beta, eps) > 0)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (alpha >= beta)
                        {
                            return true;
                        }
                    }
                    return false;
                }

                private static int Compare(double alpha, double beta, double eps)
                {
                    if (Math.Abs(alpha - beta) <= eps)
                    {
                        return 0;
                    }
                    else
                    {
                        return alpha < beta ? -1 : 1;
                    }
                }
            }

            private void Make_TwoN_Move()
            {
                if (_board.Width == 2 && _board.IsAvailableMove(1, _board.Height - 1))
                {
                    _board.MakeMove(1, _board.Height - 1);
                    return;
                }
                else if (_board.Height == 2 && _board.IsAvailableMove(_board.Width - 1, 1))
                {
                    _board.MakeMove(_board.Width - 1, 1);
                    return;
                }
                else
                {
                    if (_board.Width == 2 || (this.strategy == Strategy.Semirandom && _board.GetHorizontalLastField() == 1))
                    {
                        int leftRowLastBottom = _board.ChoosenFields.Where(a => a.x == 0).DefaultIfEmpty((x: 0, y: _board.Height)).Min(a => a.y);
                        int rightRowLastBottom = _board.ChoosenFields.Where(a => a.x == 1).DefaultIfEmpty((x: 0, y: _board.Height)).Min(a => a.y);

                        if (rightRowLastBottom == leftRowLastBottom)
                        {
                            if (_board.IsAvailableMove(1, rightRowLastBottom - 1))
                            {
                                _board.MakeMove(1, rightRowLastBottom - 1);
                            }
                            else
                            {
                                Make_Random_Move();
                            }
                        }

                        if (leftRowLastBottom < rightRowLastBottom)
                        {
                            if (_board.IsAvailableMove(1, leftRowLastBottom - 1))
                            {
                                _board.MakeMove(1, leftRowLastBottom - 1);
                            }
                            else
                            {
                                Make_Random_Move();
                            }
                        }

                        if (leftRowLastBottom > rightRowLastBottom)
                        {
                            if (_board.IsAvailableMove(0, rightRowLastBottom + 1))
                            {
                                _board.MakeMove(0, rightRowLastBottom + 1);
                            }
                            else
                            {
                                Make_Random_Move();
                            }
                        }
                        return;
                    }
                    else
                    {
                        int topRowLastLeft = _board.ChoosenFields.Where(a => a.y == 0).DefaultIfEmpty((x: _board.Width, y: 0)).Min(a => a.x);
                        int bottomRowLastLeft = _board.ChoosenFields.Where(a => a.y == 1).DefaultIfEmpty((x: _board.Width, y: 0)).Min(a => a.x);

                        if (bottomRowLastLeft == topRowLastLeft)
                        {
                            if (_board.IsAvailableMove(bottomRowLastLeft - 1, 1))
                            {
                                _board.MakeMove(bottomRowLastLeft - 1, 1);
                            }
                            else
                            {
                                Make_Random_Move();
                            }
                        }

                        if (topRowLastLeft < bottomRowLastLeft)
                        {
                            if (_board.IsAvailableMove(topRowLastLeft - 1, 1))
                            {
                                _board.MakeMove(topRowLastLeft - 1, 1);
                            }
                            else
                            {
                                Make_Random_Move();
                            }
                        }

                        if (topRowLastLeft > bottomRowLastLeft)
                        {
                            if (_board.IsAvailableMove(bottomRowLastLeft + 1, 0))
                            {
                                _board.MakeMove(bottomRowLastLeft + 1, 0);
                            }
                            else
                            {
                                Make_Random_Move();
                            }
                        }
                        return;
                    }
                }
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
            NM,
            Random,
            Semirandom
        }

        public Form1(int width, int height, string strategy1, string strategy2, int depth1, int depth2)
        {
            InitializeComponent();
            closeButton.Visible = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            Strategy? str_1 = null;
            Strategy? str_2 = null;

            if (strategy1 == "losowa")
            {
                str_1 = Strategy.Random;
            }
            else if (strategy1 == "półlosowa")
            {
                str_1 = Strategy.Semirandom;
            }
            if (strategy2 == "losowa")
            {
                str_2 = Strategy.Random;
            }
            else if (strategy2 == "półlosowa")
            {
                str_2 = Strategy.Semirandom;
            }

            board = new Board
            {
                Width = width,
                Height = height
            };
            game = new Game(board, str_1, str_2, depth1, depth2);

            UpdateBoard();
            UpdateLabel();
        }

        private void UpdateAfterPlayerMove()
        {
            UpdateBoard();
            UpdateLabel();

            if (board.IsEndOfTheGame())
            {
                int winner = game.MoveOfFirstPlayer.Value ? 2 : 1;
                button1.Enabled = false;
                label1.Text = $"Wygrał gracz: {winner}";
                closeButton.Visible = true;
            }
        }

        private void UpdateBoard()
        {
            int widthOfTile = pictureBox1.Size.Width / board.Width;
            int heightOfTile = pictureBox1.Size.Height / board.Height;

            widthOfTile = Math.Min(widthOfTile, heightOfTile);
            heightOfTile = widthOfTile;

            basicBoard = new Bitmap(widthOfTile * board.Width, heightOfTile * board.Height);

            using (Graphics graphics = Graphics.FromImage(basicBoard))
            {
                using SolidBrush brushGoal = new(Color.SaddleBrown);
                using SolidBrush brushLastPiece = new(Color.Red);
                using SolidBrush choosenField = new(Color.Gray);

                using SolidBrush brush = new(Color.White);
                using Pen penBorder = new(Color.Black, 2);

                for (int x = 0; x < board.Width; x++)
                {
                    for (int y = 0; y < board.Height; y++)
                    {
                        graphics.FillRectangle(board.ChoosenFields.Contains((x, y)) ? choosenField : brushGoal, new Rectangle(x * widthOfTile, y * heightOfTile, widthOfTile, heightOfTile));
                        graphics.DrawRectangle(penBorder, new Rectangle(x * widthOfTile, y * heightOfTile, widthOfTile, heightOfTile));
                    }
                }

                graphics.FillRectangle(brushLastPiece, new Rectangle(0, 0, widthOfTile, heightOfTile));
                graphics.DrawRectangle(penBorder, new Rectangle(0, 0, widthOfTile, heightOfTile));
            }

            pictureBox1.Image = basicBoard;
            pictureBox1.Refresh();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            await game.MakeMoveAsync();
            UpdateAfterPlayerMove();

            if (!board.IsEndOfTheGame())
            {
                button1.Enabled = true;
            }
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
