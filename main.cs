using System;
namespace TicTacToe_2
{
    public class main
    {
        public static void Main(string[] args)
        {
            TicTacToe game = new TicTacToe();

            game.TossUp();

            game.PlayGame();

            game.ReversePlay();
        }
    }
}
