using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COMP472_Color_Puzzle
{
    class GameState
    {
        private string DEBUG_BOARD_INFO = "e r r r r r b w b b w y b r y";

        /* This is a list that will represent the board, the int is the index on the baord and the char is the color */
        //List<KeyValuePair<int, char>> TheBoard = new List<KeyValuePair<int, char>>();
        //Dictionary<int, char> TheBoard = new Dictionary<int, char>();
        List<char> TheBoard = new List<char>();
        private List<char> MoveHistory = new List<char>();
        public int moveCounter { get; private set; }
        public int OptimalCounter { get; private set; }
        public int EmptyIndex { get; private set; }
        public bool ActualMove { get; set; }
        public int boardsize { get; private set; }
        private StringBuilder moves;
        private StringBuilder currentState;
        private int currentEval;

        /* Private Class Values */

        public string GetMoveHistory()
        {
            return moves.ToString();
        }

        public void PushMove(char move)
        {
            MoveHistory.Add(move);
        }

        public GameState()
        {
            currentState = new StringBuilder();
            moves = new StringBuilder();
            currentEval = 0;
        }

        public GameState(string Board_info)
        {
            currentState = new StringBuilder();
            moves = new StringBuilder();
            BuildBoard(Board_info);
        }

        public void BuildBoard(string Board_Info)
        {
            TheBoard.Clear();
            EmptyIndex = -1;
            /*remove all spaces*/
            string data = Board_Info.Replace(" ", string.Empty);
            boardsize = data.Length;

            for (int i = 0; i != boardsize; ++i)
            {
                //KeyValuePair<int, char> temp = new KeyValuePair<int, char>(i,Board_info[i]);
                TheBoard.Add(data[i]);
                EmptyIndex = (data[i] == 'e') ? i : EmptyIndex;
            }
            if (EmptyIndex == -1) {/* Possible error checking, no empty chip in board */ }
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
                 ++OptimalCounter;  // moveCounter is only toggled on actual game moves
            
            }
        }

        public char GetChip(int index)
        {
            return TheBoard[index];
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
            if (true)
            {
                Console.Clear();
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++");
                for (int i = 0; i != 3; ++i)
                {
                    Console.WriteLine("+_______________________________________+");
                    Console.WriteLine("+ | {0} | + | {1} | + | {2} | + | {3} | + | {4} | +", TheBoard[(i * 5) + 0], TheBoard[(i * 5) + 1], TheBoard[(i * 5) + 2], TheBoard[(i * 5) + 3], TheBoard[(i * 5) + 4]);
                    Console.WriteLine("+_______________________________________+");
                }
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++\n\n");

                Console.Write("Move History: ");
            }
            Console.WriteLine(moves.ToString());
            if (true)
            {
                Console.WriteLine("Found Path: {0} ", OptimalCounter);
                Console.WriteLine("Move Counter: {0} ", moveCounter);
            }
        }
    }
}