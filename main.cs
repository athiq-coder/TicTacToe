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

            game.Undo();

            game.Undo();

            game.Undo();

            game.Undo();

            game.Undo();

            game.Display();

            game.Redo();

            game.Display();

            game.Redo();
        }
    }
}
