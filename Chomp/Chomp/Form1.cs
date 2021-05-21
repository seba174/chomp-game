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

            public Board MakeNextMove((int x, int y) move)
            {
                var newBoard = new Board()
                {
                    Width = this.Width,
                    Height = this.Height,
                    MovesList = new List<(int x, int y)>(this.MovesList),
                    ChoosenFields = new HashSet<(int x, int y)>((this.ChoosenFields))
                };
                newBoard.MovesList.Add((move.x, move.y));
                MarkFieldsAsChoosen(newBoard.ChoosenFields, move.x, move.y);
                return newBoard;
            }

            private void MarkFieldsAsChoosen(int x, int y)
            {
                MarkFieldsAsChoosen(this.ChoosenFields, x, y);
            }

            private void MarkFieldsAsChoosen(HashSet<(int x, int y)> choosenFields, int x, int y)
            {
                for (int i = x; i < Width; i++)
                {
                    for (int j = y; j < Height; j++)
                    {
                        choosenFields.Add((i, j));
                    }
                }
            }

            public bool IsEndOfTheGame()
            {
                return ChoosenFields.Count == (Width * Height - 1);
            }

            public bool IsMoveOfFirstPlayer()
            {
                return MovesList.Count % 2 == 0;
            }

            public IEnumerable<(int x, int y)> GetAllPossibleMoves()
            {
                HashSet<(int x, int y)> allPossibbleMoves = new HashSet<(int, int)>();

                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        if (i == 0 && j == 0)
                        {
                            continue;
                        }
                        allPossibbleMoves.Add((i, j));
                    }
                }

                foreach (var pair in ChoosenFields)
                {
                    allPossibbleMoves.Remove(pair);
                }

                return allPossibbleMoves;
            }
        }

        public class Game
        {
            private Board _board;
            private Player _player1;
            private Player _player2;

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
            private AlphaBeta _alphaBeta;
            private readonly Strategy strategy;
            private bool? amIFirstPlayer;

            public Player(Board board)
            {
                _board = board;
                strategy = GetStrategy();
                _alphaBeta = new AlphaBeta(3, SimpleEvaluator.Evaluate);
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
                    case Strategy.TwoN:
                        Make_TwoN_Move();
                        break;
                    case Strategy.NM:
                        Make_NM_Move();
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

            // TODO proper implementation
            private void Make_NM_Move()
            {
                // for now random move
                var allPossibbleMoves = _board.GetAllPossibleMoves();
                //var allPossibbleMoves = _alphaBeta.GetBestMoves(_board, amIFirstPlayer.Value);

                var random = new Random();
                var choosenMoveIndex = random.Next(0, allPossibbleMoves.Count() - 1);
                var choosenMove = allPossibbleMoves.Skip(choosenMoveIndex).FirstOrDefault();

                _board.MakeMove(choosenMove.x, choosenMove.y);

            }

            private class SimpleEvaluator
            {
                public static double Evaluate(Board board)
                {
                    var possibleMoves = board.GetAllPossibleMoves();
                    if (possibleMoves.Any())
                    {
                        return double.MaxValue;
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
                    return board.ChoosenFields.Count() % 2 == 1;
                }
            }

            private class AlphaBeta
            {
                private int _max_depth { get; set; }
                private Func<Board, double> _evaluation { get; set; }
                private double _eps { get; set; }

                public AlphaBeta(int max_depth, Func<Board, double> evaluation, double eps = 0)
                {
                    this._max_depth = max_depth;
                    this._evaluation = evaluation;
                    this._eps = eps;
                }

                public IEnumerable<(int x, int y)> GetBestMoves(Board board, bool is_first_player)
                {
                    var allPossibbleMoves = board.GetAllPossibleMoves();

                    double alpha = double.MinValue; 
                    double beta = double.MaxValue;

                    var moves = new List<((int x, int y), double)>();

                    foreach (var move in allPossibbleMoves)
                    {
                        var value = eval_move(board, move, _max_depth, ref alpha, ref beta, is_first_step: true, is_first_player);
                        moves.Add((move, value));
                        alpha = Math.Max(alpha, value);
                    }

                    var bestMoves = moves.Where(x => x.Item2 == alpha).Select(x => x.Item1).ToList();
                    return bestMoves.Any() ? bestMoves : allPossibbleMoves;
                }

                private double eval_move(Board board, (int x, int y) move, int depth, ref double alpha, ref double beta, bool is_first_step, bool is_first_player)
                {
                    var newBoard = board.MakeNextMove(move);
                    return alpha_beta(newBoard, depth - 1, ref alpha, ref beta, is_first_step, is_first_player);
                }

                private double alpha_beta(Board board, int depth, ref double alpha, ref double beta, bool is_first_step, bool is_first_player)
                {
                    var is_maximizing = board.IsMoveOfFirstPlayer() == is_first_player;
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
                            value = Math.Max(value, eval_move(board, move, depth, ref alpha, ref beta, is_first_step, is_first_player));
                            alpha = Math.Max(alpha, value);
                            if (should_cut(alpha, beta, _eps, is_first_step))
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
                            value = Math.Min(value, eval_move(board, move, depth, ref alpha, ref beta, is_first_step, is_first_player));
                            beta = Math.Min(beta, value);
                            if (should_cut(alpha, beta, _eps, is_first_step))
                            {
                                break;
                            }
                        }
                        return value;
                    }
                }

                private bool should_cut(double alpha, double beta, double eps, bool is_first_step)
                {
                    return false;
                    if (is_first_step)
                    {
                        if (compare(alpha, beta, eps) > 0)
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

                private int compare(double alpha, double beta, double eps)
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
