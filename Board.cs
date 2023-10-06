using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectFourMinimax
{
    public class Board
    {
        // Minimum variables needed for reconstructing the board
        public ulong Player1Bitboard;
        public ulong Player2Bitboard;

        // Variables derived from bitboards for easier board manipulation
        public bool IsPlayer1Turn;
        public int[] ColumnHeights;
        public int MovesPlayedPlayer1;
        public int MovesPlayedPlayer2;
        
        public Board()
        {
            Player1Bitboard = 0UL;
            Player2Bitboard = 0UL;

            IsPlayer1Turn = true;
            ColumnHeights = new int[Constants.Columns];
            MovesPlayedPlayer1 = 0;
            MovesPlayedPlayer2 = 0;
        }
        
        public void MakeMove(int move)
        {
            int stackHeight = ColumnHeights[move];
            
            if (stackHeight >= Constants.Rows)
            {
                throw new Exception("Failed to make move. Column full.");
            }

            ulong moveMask = 1UL << (move * (Constants.Rows + 1) + stackHeight);
            (IsPlayer1Turn ? ref Player1Bitboard : ref Player2Bitboard) |= moveMask;
            
            if (IsPlayer1Turn)
            {
                MovesPlayedPlayer1++;
            }
            else
            {
                MovesPlayedPlayer2++;
            }
            
            ColumnHeights[move]++;
            IsPlayer1Turn = !IsPlayer1Turn;
        }

        public void UnmakeMove(int move)
        {
            IsPlayer1Turn = !IsPlayer1Turn;
            
            int stackHeight = ColumnHeights[move] - 1;
            
            if (stackHeight < 0)
            {
                throw new Exception("Failed to unmake move. Column empty.");
            }

            ulong moveMask = 1UL << (move * (Constants.Rows + 1) + stackHeight);
            (IsPlayer1Turn ? ref Player1Bitboard : ref Player2Bitboard) &= ~moveMask;
            
            if (IsPlayer1Turn)
            {
                MovesPlayedPlayer1--;
            }
            else
            {
                MovesPlayedPlayer2--;
            }
            
            ColumnHeights[move]--;
        }

        public bool CheckWin(ulong bitboard)
        {
            // Bottom-left to top-right diagonal
            ulong diag1 = bitboard & (bitboard >> (Constants.Rows + 2));
            if ((diag1 & (diag1 >> (2 * (Constants.Rows + 2)))) != 0)
            {
                return true;
            }

            // Bottom-right to top-left diagonal
            ulong diag2 = bitboard & (bitboard >> (Constants.Rows));
            if ((diag2 & (diag2 >> (2 * Constants.Rows))) != 0)
            {
                return true;
            }

            // Horizontal
            ulong horizontal = bitboard & (bitboard >> (Constants.Rows + 1));
            if ((horizontal & (horizontal >> (2 * (Constants.Rows + 1)))) != 0)
            {
                return true;
            }

            // Vertical
            ulong vertical = bitboard & (bitboard >> 1);
            if ((vertical & (vertical >> 2)) != 0)
            {
                return true;
            }

            return false;
        }

        public bool CheckBoardFull()
        {
            return MovesPlayedPlayer1 + MovesPlayedPlayer2 == Constants.Rows * Constants.Columns;
        }

        public void DisplayBoard()
        {
            for (int i = Constants.Rows - 1; i >= 0; i--)
            {
                for (int j = 0; j < Constants.Columns; j++)
                {
                    int bitPosition = j * (Constants.Rows + 1) + i;
                    ulong mask = 1UL << bitPosition;

                    if ((Player1Bitboard & mask) > 0)
                    {
                        Console.Write("x ");
                    }
                    else if ((Player2Bitboard & mask) > 0)
                    {
                        Console.Write("o ");
                    }
                    else
                    {
                        Console.Write(". ");
                    }
                }
                
                Console.WriteLine();
            }
        }

        public void LoadDSEN(string dropSequence)
        {
            Player1Bitboard = 0UL;
            Player2Bitboard = 0UL;
            
            IsPlayer1Turn = true;
            ColumnHeights = new int[Constants.Columns];
            MovesPlayedPlayer1 = 0;
            MovesPlayedPlayer2 = 0;
            
            foreach (char c in dropSequence)
            {
                int column = int.Parse(c.ToString()) - 1;
                int row = ColumnHeights[column];

                if (column < 0 || column >= Constants.Columns)
                {
                    throw new Exception("Failed to load DSEN. Invalid number of columns.");
                }
                
                if (row >= Constants.Rows)
                {
                    throw new Exception("Failed to load DSEN. Invalid number of rows.");
                }

                ulong dropPosition = 1UL << (column * (Constants.Rows + 1) + row);

                (IsPlayer1Turn ? ref Player1Bitboard : ref Player2Bitboard) |= dropPosition;

                if (IsPlayer1Turn)
                {
                    MovesPlayedPlayer1++;
                }
                else
                {
                    MovesPlayedPlayer2++;
                }
                
                ColumnHeights[column]++;
                IsPlayer1Turn = !IsPlayer1Turn;
            }
        }

        public string ExportPseudoDSEN()
        {
            List<int> player1Moves = new List<int>();
            List<int> player2Moves = new List<int>();

            for (int row = 0; row < Constants.Rows; row++)
            {
                for (int col = 0; col < Constants.Columns; col++)
                {
                    int bitPosition = col * (Constants.Rows + 1) + row;
                    ulong mask = 1UL << bitPosition;

                    if ((Player1Bitboard & mask) > 0)
                    {
                        player1Moves.Add(col + 1);
                    }
                    else if ((Player2Bitboard & mask) > 0)
                    {
                        player2Moves.Add(col + 1);
                    }
                }
            }

            if (player2Moves.Count > player1Moves.Count || player1Moves.Count > player2Moves.Count + 1)
            {
                throw new Exception("Failed to reconstruct DSEN. Uneven Moves.");
            }

            StringBuilder pseudoDSEN = new StringBuilder();
            int maxMoves = Math.Max(player1Moves.Count, player2Moves.Count);
            for (int i = 0; i < maxMoves; i++)
            {
                if (i < player1Moves.Count)
                {
                    pseudoDSEN.Append(player1Moves[i]);
                }
                if (i < player2Moves.Count)
                {
                    pseudoDSEN.Append(player2Moves[i]);
                }
            }

            return pseudoDSEN.ToString();
        }
    }
}