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

            public Game(Board board)
            {
                _board = board;
                _player1 = new Player(_board);
                _player2 = new Player(_board);
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
            private Board _board;
            private AlphaBetaStrategy _alphaBeta;
            private readonly Strategy strategy;
            private bool? amIFirstPlayer;

            public Player(Board board)
            {
                _board = board;
                strategy = GetStrategy();
                _alphaBeta = new AlphaBetaStrategy(4, SimpleEvaluator.Evaluate);
            }

            public async Task MakeMoveAsync()
            {
                if (amIFirstPlayer == null)
                {
                    amIFirstPlayer = _board.MovesList.Count == 0;
                }

                // second player hasnt winning strategy, so always same move
                if (!amIFirstPlayer.Value)
                {
                    await Make_NM_MoveAsync();
                    return;
                }

                // first player strategies
                switch (strategy)
                {
                    case Strategy.NN:
                        Make_NN_Move();
                        break;
                    case Strategy.TwoN:
                        Make_TwoN_Move();
                        break;
                    case Strategy.NM:
                        await Make_NM_MoveAsync();
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
                    var (x, y) = _board.MovesList.Last();
                    _board.MakeMove(y, x);
                }
            }

            private async Task Make_NM_MoveAsync()
            {
                var player_to_maximize = _board.IsMoveOfFirstPlayer() ? 1 : 2;
                var allPossibbleMoves = await Task.Run(() =>
                {
                    return _alphaBeta.GetBestMoves(_board, player_to_maximize);
                });

                var random = new Random();
                var choosenMoveIndex = random.Next(0, allPossibbleMoves.Count() - 1);
                var (x, y) = allPossibbleMoves.Skip(choosenMoveIndex).FirstOrDefault();

                _board.MakeMove(x, y);
            }

            private class SimpleEvaluator
            {
                private const double winValue = 100;

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

                    int goodMoves = 0;
                    foreach (var move in possibleMoves)
                    {
                        var newBoard = board.MakeNextMove(move);
                        if (!IsBadPosition(board))
                        {
                            goodMoves++;
                        }
                    }
                    return goodMoves;
                }

                private static bool IsBadPosition(Board board)
                {
                    return board.ChoosenFields.Count % 2 == 1;
                }
            }

            private class AlphaBetaStrategy
            {
                private int _maxDepth { get; set; }
                private Func<Board, double> _evaluation { get; set; }
                private double _eps { get; set; }

                public AlphaBetaStrategy(int max_depth, Func<Board, double> evaluation, double eps = 0)
                {
                    _maxDepth = max_depth;
                    _evaluation = evaluation;
                    _eps = eps;
                }

                public IEnumerable<(int x, int y)> GetBestMoves(Board board, int player_to_maximize)
                {
                    var allPossibbleMoves = board.GetAllPossibleMoves();

                    double alpha = double.MinValue;
                    double beta = double.MaxValue;

                    var moves = new List<((int x, int y), double)>();

                    foreach (var move in allPossibbleMoves)
                    {
                        var value = EvalMove(board, move, _maxDepth, alpha, beta, is_first_step: true, player_to_maximize);
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
                    var currentPlayerTurn = board.IsMoveOfFirstPlayer() ? 1 : 2;
                    var is_maximizing = currentPlayerTurn == player_to_maximize;

                    var posible_moves = board.GetAllPossibleMoves();
                    var is_game_over = board.IsEndOfTheGame();

                    if (is_game_over || depth == 0)
                    {
                        var value = _evaluation(board);
                        if (!is_maximizing)
                        {
                            value *= -1;
                        }
                        return value;
                    }

                    if (is_maximizing)
                    {
                        var value = double.MinValue;
                        foreach (var move in posible_moves)
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
                        var value = double.MaxValue;
                        foreach (var move in posible_moves)
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

                private bool ShouldCut(double alpha, double beta, double eps, bool is_first_step)
                {
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
                if (_board.MovesList.Count == 0)
                {
                    if (_board.Width == 2)
                    {
                        _board.MakeMove(1, _board.Height - 1);
                    }
                    else
                    {
                        _board.MakeMove(_board.Width - 1, 1);
                    }
                    return;
                }
                else
                {
                    if (_board.Width == 2)
                    {
                        var leftRowLastBottom = _board.ChoosenFields.Where(a => a.x == 0).DefaultIfEmpty((x: 0, y: _board.Height)).Min(a => a.y);
                        var rightRowLastBottom = _board.ChoosenFields.Where(a => a.x == 1).DefaultIfEmpty((x: 0, y: _board.Height)).Min(a => a.y);

                        if (rightRowLastBottom == leftRowLastBottom)
                        {
                            _board.MakeMove(1, rightRowLastBottom - 1);
                        }

                        if (leftRowLastBottom < rightRowLastBottom)
                        {
                            _board.MakeMove(1, leftRowLastBottom - 1);
                        }

                        if (leftRowLastBottom > rightRowLastBottom)
                        {
                            _board.MakeMove(0, rightRowLastBottom + 1);
                        }
                        return;
                    }
                    else
                    {
                        var topRowLastLeft = _board.ChoosenFields.Where(a => a.y == 0).DefaultIfEmpty((x: _board.Width, y: 0)).Min(a => a.x);
                        var bottomRowLastLeft = _board.ChoosenFields.Where(a => a.y == 1).DefaultIfEmpty((x: _board.Width, y: 0)).Min(a => a.x);

                        if (bottomRowLastLeft == topRowLastLeft)
                        {
                            _board.MakeMove(bottomRowLastLeft - 1, 1);
                        }

                        if (topRowLastLeft < bottomRowLastLeft)
                        {
                            _board.MakeMove(topRowLastLeft - 1, 1);
                        }

                        if (topRowLastLeft > bottomRowLastLeft)
                        {
                            _board.MakeMove(bottomRowLastLeft + 1, 0);
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
            NM
        }

        public Form1(int width, int height)
        {
            InitializeComponent();
            closeButton.Visible = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

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

            widthOfTile = Math.Min(widthOfTile, heightOfTile);
            heightOfTile = widthOfTile;

            basicBoard = new Bitmap(widthOfTile * board.Width, heightOfTile * board.Height);

            using (var graphics = Graphics.FromImage(basicBoard))
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
