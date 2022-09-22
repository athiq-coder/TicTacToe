using System;
using System.Collections.Generic;

namespace TicTacToe_2
{ 
    class Board
    {
        Coin[] tiles;
        BoardState boardState;
        int filledPlaces;
        public const int BOARDSIZE = 9;

        List<int> allPositions;
        Stack<int> lastPositions;
        Stack<int> deletedPositions;
        Stack<Coin> deletedCoins;

        public Board()
        { 
           this.tiles = new Coin[BOARDSIZE];
           this.boardState = BoardState.Empty;
            this.allPositions = new List<int>();
           this.lastPositions = new Stack<int>(BOARDSIZE);
           this.deletedPositions = new Stack<int>(BOARDSIZE);
           this.deletedCoins = new Stack<Coin>(BOARDSIZE);
           this.filledPlaces = 0;

            for (int i=0; i < BOARDSIZE; i++)
            {
                this.tiles[i] = new Coin(Choice.N);
            }
        }

        public bool PlaceCoin(int position,Coin coin)
        {
            if (IsFree(position))
            {
                this.tiles[position] = coin;
                this.filledPlaces++;
                this.lastPositions.Push(position);
                this.allPositions.Add(position);
                UpdateBoardState();
                return true;
            }
            return false;
        }

        public void DisplayBoard()
        {
            int count = 0;

            for (int i = 0; i < Board.BOARDSIZE; i++)
            {
                if (count == 3)
                {
                    Console.WriteLine();
                    Console.WriteLine("----------");
                    count = 0;
                }

                count++;
                Console.Write("| " + this.tiles[i].GetChoice().ToString().ToUpper());
            }
            Console.WriteLine();
        }

        void UpdateBoardState()
        {
            if (this.filledPlaces == 0)
            {
                this.boardState = BoardState.Empty;
            }

            else if (this.filledPlaces < 5)
            {
                 this.boardState = BoardState.PartiallyFilled;
            }

            else if (this.filledPlaces < 9)
            {
                this.boardState = BoardState.HalfFilled;
            }

            else if (this.filledPlaces == 9)
            {
                 this.boardState = BoardState.Full;
            }
        }

        public void UndoLastPosition()
        {
            if(this.boardState == BoardState.Empty)
            {
                return;
            }
            int pos = this.lastPositions.Pop();
            this.deletedPositions.Push(pos);
            this.deletedCoins.Push(this.tiles[pos]);
            this.tiles[pos] = new Coin(Choice.N);
            this.filledPlaces--;
            UpdateBoardState();
        }

        public void RedoLastPosition()
        {
            if(this.lastPositions.Count == 0)
            {
                return;
            }
            int pos = this.deletedPositions.Pop();
            Coin deletedCoin = this.deletedCoins.Pop();
            this.tiles[pos] = deletedCoin;
            this.filledPlaces++;
            UpdateBoardState();
        }

        public void ReverseReplay()
        {
          int startIndex = this.allPositions.Count - 1;

            for(int i = startIndex; i >=0; i--)
            {
                int count = 0;

                for(int j = startIndex; j >= 0; j--)
                {
                    if (count == 3)
                    {
                        Console.WriteLine();
                        Console.WriteLine("----------");
                        count = 0;
                    }
                    if (j >= i)
                    {
                        Console.Write("| " + this.tiles[j].GetChoice().ToString().ToUpper());
                    }
                    if(j < i)
                    {
                        Console.Write("| " + Choice.N);
                    }
                    count++;
                }
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                System.Threading.Thread.Sleep(1000);
            }
            
        }

        public BoardState GetBoardState() { return this.boardState; }
        
        public bool IsFree(int position) { return this.tiles[position].GetChoice() == Choice.N; }
        
        public bool HasCoin(int position,Coin coin) { return this.tiles[position] == coin; }
    
    };

    class Coin
    {
        Choice player;

        public Coin(Choice player) { this.player = player; }

        public Choice GetChoice() { return this.player; }
    };
    
    class TicTacToe
    {
        Board gameBoard;
        Coin firstPlayer;
        Coin secondPlayer;
        Player currentPlayer;
        Player tossWinner;
        State gameState;

        public TicTacToe()
        {
            this.gameBoard = new Board();
            this.gameState = State.Noresult;
        }

        public void TossUp()
        {
            Random r = new Random();
            SetConsoleColor(ConsoleColor.DarkYellow);
            Console.WriteLine("************************************");
            SetConsoleColor(ConsoleColor.DarkMagenta);
            if ((Player)r.Next(1, 3) == Player.First)
            {
                Console.WriteLine("* YAYYY! First player won the toss *");
                SetConsoleColor(ConsoleColor.DarkYellow);
                Console.WriteLine("************************************");
                this.tossWinner = Player.First;
            }
            else
            {
                Console.WriteLine("* YAYYY! Second player won the toss *");
                SetConsoleColor(ConsoleColor.DarkYellow);
                Console.WriteLine("************************************");
                this.tossWinner = Player.Second;
            }
            SetPlayerChoice();
        }

        public void ShowPlayersChoice()
        {
            PrintMessage(string.Format("   PLAYER CHOICES"));
            PrintMessage(string.Format("   PLAYER 1 : {0}", this.firstPlayer.GetChoice().ToString().ToUpper()));
            PrintMessage(string.Format("   PLAYER 2 : {0}", this.secondPlayer.GetChoice().ToString().ToUpper()));
        }

        public void PlayGame()
        {
            this.currentPlayer = WhoShouldPlayFirst(); // Tosswinner will commence

            while (true)
            {
                bool isCoinPlaced = false;

                int position = getUserInput();

                isCoinPlaced = this.gameBoard.PlaceCoin(position,getCurrentPlayerChoice(this.currentPlayer));
                
                this.gameBoard.DisplayBoard();

                if (isGameDraw() && isCoinPlaced)
                {
                    this.gameState = State.Draw;
                    break;
                }

                if (isHalfFilled() && isCoinPlaced)
                {
                    this.gameState = CheckWinner(position,getCurrentPlayerChoice(currentPlayer));

                    if (this.gameState == State.Win)
                    {
                        break;
                    }
                }

                ChangePlayer();
            }

            AnnounceResults();
        }

        public void ReversePlay()
        {
            this.gameBoard.ReverseReplay();
        }

        bool isHalfFilled()
        {
            return this.gameBoard.GetBoardState() == BoardState.HalfFilled;
        }

        bool isGameDraw()
        {
            return this.gameBoard.GetBoardState() == BoardState.Full;
        }

        State CheckWinner(int recentMove,Coin coin)
        {
            if( recentMove == 0 ) 
            {
                if (gameBoard.HasCoin(3, coin) && gameBoard.HasCoin(6, coin) || gameBoard.HasCoin(4, coin) && gameBoard.HasCoin(8, coin))
                {
                    return State.Win;
                }
            }

            if (recentMove == 1 ) 
            {
                if (gameBoard.HasCoin(0, coin) && gameBoard.HasCoin(2, coin) || gameBoard.HasCoin(4, coin) && gameBoard.HasCoin(7, coin))
                {
                    return State.Win;
                }
            }

            if( recentMove == 2) 
            {
                if (gameBoard.HasCoin(0, coin) && gameBoard.HasCoin(1, coin) || gameBoard.HasCoin(4, coin) && gameBoard.HasCoin(6, coin) || gameBoard.HasCoin(5, coin) && gameBoard.HasCoin(8, coin))
                {
                    return State.Win;
                }
            }

            if ( recentMove == 3) 
            {
                if (gameBoard.HasCoin(0, coin) && gameBoard.HasCoin(6, coin) || gameBoard.HasCoin(4, coin) && gameBoard.HasCoin(5, coin))
                {
                    return State.Win;
                }
            }

            if ( recentMove == 4) 
            {
                if (gameBoard.HasCoin(1, coin) && gameBoard.HasCoin(7, coin) || gameBoard.HasCoin(0, coin) && gameBoard.HasCoin(8, coin) || gameBoard.HasCoin(3, coin) && gameBoard.HasCoin(5, coin) || gameBoard.HasCoin(2, coin) && gameBoard.HasCoin(6, coin))
                {
                    return State.Win;
                }
            }

            if ( recentMove == 5) 
            {
                if (gameBoard.HasCoin(2, coin) && gameBoard.HasCoin(8, coin) || gameBoard.HasCoin(3, coin) && gameBoard.HasCoin(4, coin))
                {
                    return State.Win;
                }
            }

            if (recentMove == 6)
            {
                if (gameBoard.HasCoin(7, coin) && gameBoard.HasCoin(8, coin) || gameBoard.HasCoin(0, coin) && gameBoard.HasCoin(3, coin) || gameBoard.HasCoin(4, coin) && gameBoard.HasCoin(2, coin))
                { 
                    return State.Win;
                }
            }

            if ( recentMove == 7) 
            {
                if (gameBoard.HasCoin(1, coin) && gameBoard.HasCoin(4, coin) || gameBoard.HasCoin(6, coin) && gameBoard.HasCoin(8, coin))
                {
                    return State.Win;
                }
            }

            if ( recentMove == 8) 
            {
                if (gameBoard.HasCoin(2, coin) && gameBoard.HasCoin(5, coin) || gameBoard.HasCoin(0, coin) && gameBoard.HasCoin(4, coin) || gameBoard.HasCoin(6, coin) && gameBoard.HasCoin(7, coin))
                {
                    return State.Win;
                }
            }

            return State.Noresult;
        }

        void AnnounceResults()
        {
            Console.WriteLine();
            Console.WriteLine();

            if (this.gameState == State.Win)
            {
                PrintMessage(string.Format("Winner is {0}", this.currentPlayer));
            }else
            {
                PrintMessage(string.Format("The game is draw"));
            }
        }

        Player WhoShouldPlayFirst() { return this.tossWinner; }

        int getUserInput()
        {
            Console.WriteLine();
            Console.WriteLine("Enter Position: ");
            return int.Parse(Console.ReadLine()) - 1;
        }

        void ChangePlayer()
        {
          this.currentPlayer = this.currentPlayer == Player.First ? Player.Second : Player.First;
        }

        Coin getCurrentPlayerChoice(Player currentPlayer)
        {
            return currentPlayer == Player.First ? this.firstPlayer : this.secondPlayer;
        }

        void SetPlayerChoice()
        {
            PrintMessage("   Enter X or O");

            Choice WinnerChoice = (Choice)char.Parse(Console.ReadLine());

            Choice LoserChoice = (WinnerChoice == Choice.o) ? Choice.x : Choice.o;

            if(this.tossWinner == Player.First)
            {
                this.firstPlayer = new Coin(WinnerChoice);
                this.secondPlayer = new Coin(LoserChoice);
            }
            else
            {
                this.secondPlayer = new Coin(WinnerChoice);
                this.firstPlayer = new Coin(LoserChoice);
            }
            ShowPlayersChoice();
        }

        void SetConsoleColor(ConsoleColor color) { Console.ForegroundColor = color; }

        void PrintMessage(string message)
        {
            SetConsoleColor(ConsoleColor.Cyan);
            Console.WriteLine("----------------------------");
            SetConsoleColor(ConsoleColor.DarkMagenta);
            Console.WriteLine("| "+message+"          |");
            SetConsoleColor(ConsoleColor.Cyan);
            Console.WriteLine("----------------------------");
            Console.ResetColor();
        }
    };
}
