using System;

namespace ConnectFourMinimax;

public class Solver
{
    private readonly Board board;
    
    public Solver(Board board)
    {
        this.board = board;
    }

    private int Minimax(int depth, bool maximizingPlayer)
    {
        bool player1Won = !maximizingPlayer && board.CheckWin(board.Player1Bitboard);
        bool player2Won = maximizingPlayer && board.CheckWin(board.Player2Bitboard);
        bool tie = board.CheckBoardFull();
        
        if (tie || player1Won || player2Won || depth == 0)
        {
            if (player1Won)
            {
                return 22 - board.MovesPlayedPlayer1;
            }
        
            if (player2Won)
            {
                return board.MovesPlayedPlayer2 - 22;
            }

            return 0;
        }

        int bestScore = maximizingPlayer ? -Constants.MaxValue : +Constants.MaxValue;
        
        for (int col = 0; col < Constants.Columns; col++)
        {
            if (board.ColumnHeights[col] < Constants.Rows)
            {
                board.MakeMove(col);
                int score = Minimax(depth-1, !maximizingPlayer);
                board.UnmakeMove(col);
                
                bestScore = maximizingPlayer ? Math.Max(bestScore, score) : Math.Min(bestScore, score);
            }
        }

        return bestScore;
    }
}