using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COMP472_Color_Puzzle
{
    class GameState
    {

        List<char> TheBoard = new List<char>(new char[15]);
        
        public int moveCounter { get; private set; }
        public int EmptyIndex { get; private set; }
        public bool ActualMove { get; set; }
        public int boardsize { get; private set; }
        private StringBuilder moves;
        private StringBuilder currentState;

        public string GetMoveHistory()
        {
            return moves.ToString();
        }

        public GameState(string boardConfig)
        {
            currentState = new StringBuilder();
            moves = new StringBuilder();
            BuildBoard(boardConfig);
        }

        public void BuildBoard(string boardConfig)
        {
            TheBoard.Clear();
            EmptyIndex = -1;
            boardsize = boardConfig.Length;

            for (int i = 0; i != boardsize; ++i)
            {
                TheBoard.Add(boardConfig[i]);
                EmptyIndex = (boardConfig[i] == 'e') ? i : EmptyIndex;
            }
        }

        public void swap(int index1, int index2)
        {
            char swap = TheBoard[index1];
            TheBoard[index1] = TheBoard[index2];
            TheBoard[index2] = swap;
            EmptyIndex = index2;
            ++moveCounter;
            
            if (ActualMove)
            {
                moves.Append((char)(index2 + 65)); //ASCI value of A
            }
        }

        public override string ToString()
        {
            currentState.Clear();
            foreach (char boardCell in TheBoard)
            {
                currentState.Append(boardCell);
            }
            return currentState.ToString();
        }

        public void Draw()
        {
            Console.Clear();
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++");
            for (int i = 0; i != 3; ++i)
            {
                Console.WriteLine("+_______________________________________+");
                Console.WriteLine("+ | {0} | + | {1} | + | {2} | + | {3} | + | {4} | +",
                    TheBoard[(i * 5) + 0], TheBoard[(i * 5) + 1], TheBoard[(i * 5) + 2],
                    TheBoard[(i * 5) + 3], TheBoard[(i * 5) + 4]);
                Console.WriteLine("+_______________________________________+");
            }
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++\n\n");

            Console.Write("Move History: " + moves.ToString());
        }

        public void ClearMoveHistroy()
            {
            moves.Clear();
            }
    }
}