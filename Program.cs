using System.Diagnostics;

namespace ConnectFourMinimax
{
    class Program
    {
        public static void Main()
        {
            Board board = new Board();
            const string dsenString = "443";
            
            board.LoadDSEN(dsenString);
            Console.WriteLine("Loaded original DSEN String: \"" + dsenString + "\"");
            
            Console.WriteLine($"Reconstructed pseudo DSEN: \"{board.ExportPseudoDSEN()}\"");
            Console.WriteLine();

            Console.WriteLine("Current board state:");
            board.DisplayBoard();
            Console.WriteLine();
            
            int lastMoveColumn = dsenString[^1] - '1';
            Console.WriteLine($"Player 1 has made {board.MovesPlayedPlayer1} moves.");
            Console.WriteLine($"Player 2 has made {board.MovesPlayedPlayer2} moves.");
            Console.WriteLine($"The last move was played in column {lastMoveColumn + 1}.");
            Console.WriteLine($"It is Player {(board.IsPlayer1Turn ? 1 : 2)}'s turn.");
            Console.WriteLine();
            
            Console.WriteLine("Stack heights: " + string.Join(", ", board.ColumnHeights));
            
            List<int> validMoves = new List<int>();
            for (int col = 0; col < Constants.Columns; col++)
            {
                if (board.ColumnHeights[col] < Constants.Rows)
                {
                    validMoves.Add(col + 1);
                }
            }
            Console.WriteLine("Valid moves: " + string.Join(", ", validMoves));
            Console.WriteLine();
            
            Console.WriteLine("Player 1 bitboard: " + Convert.ToString((long)board.Player1Bitboard, 2).PadLeft(Constants.Rows * Constants.Columns, '0'));
            Console.WriteLine("Player 2 bitboard: " + Convert.ToString((long)board.Player2Bitboard, 2).PadLeft(Constants.Rows * Constants.Columns, '0'));
            Console.WriteLine();

            Console.WriteLine("Player 1 has won: " + board.CheckWin(board.Player1Bitboard));
            Console.WriteLine("Player 2 has won: " + board.CheckWin(board.Player2Bitboard));
            Console.WriteLine();
            
            TimedAction("Processed 1 million Player 1 and Player 2 win checks", () => {
                board.CheckWin(board.Player1Bitboard);
                board.CheckWin(board.Player2Bitboard);
            }, 1_000_000/2);

            TimedAction("Processed 1 million move and unmove operations", () => {
                board.UnmakeMove(lastMoveColumn);
                board.MakeMove(lastMoveColumn);
            }, 1_000_000/2);
        }

        private static void TimedAction(string description, Action action, int iterations)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                action();
            }

            stopwatch.Stop();
            Console.WriteLine($"{description} in {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        }
    }
}