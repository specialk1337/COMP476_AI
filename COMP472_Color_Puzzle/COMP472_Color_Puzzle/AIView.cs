﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COMP472_Color_Puzzle
{
    class AIView
    {
        private GameCommand _command;
        private GameState _state;

        private Dictionary<char, string> legalMoves;

        private bool DEBUG_MODE = true;

        //removed for now
        //private Dictionary<string, int> closedList;
        //private PriorityQueue<BoardToScore> openList;
        
        public AIView(GameCommand command)
        {
            _command = command;
            _state = _command.getState();
            legalMoves = new Dictionary<char,string>(4);
            legalMoves.Add('u', string.Empty);
            legalMoves.Add('d', string.Empty);
            legalMoves.Add('l', string.Empty);
            legalMoves.Add('r', string.Empty);
        }

        public void play()
        {
            string initialState = _state.ToString();
            string lastMoveString = string.Empty;
            char bestMove = 'n'; // 'n' means no move, 'u', 'd', 'l', 'r' are self-explanatory ;-)
            int bestMoveValue = (int)score(initialState);
            int currentMoveValue = 0;

            if (DEBUG_MODE)
            _command.Draw();
            // Debugging:
            if (DEBUG_MODE)
            Console.WriteLine("Initial state: {0} score: {1}", initialState, bestMoveValue);
            

            while (true)
            {
                if (DEBUG_MODE)
                _command.Draw();
                // if puzzle is solved
                if (_command.VerifyBoard())
                {
                    Console.WriteLine("SUCCESS");
                    return;
                }

                GenerateLegalMoves();

                foreach (char move in legalMoves.Keys)
                {
                    // if no legal move exists, go to next move
                    if (string.IsNullOrEmpty(legalMoves[move]))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(lastMoveString) && legalMoves[move] == lastMoveString)
                        continue;
                    
                    // if move solves puzzle, exit
                    //if (legalMoves[move].EndsWith("WIN"))
                    //{
                    //    legalMoves[move] = legalMoves[move].Substring(0, legalMoves[move].IndexOf("WIN"));
                    //    bestMove = move;
                    //    bestMoveValue = 1000000;
                    //    break;
                    //}

                    currentMoveValue = (int)score(legalMoves[move]);
                    if (currentMoveValue >= bestMoveValue)
                    {
                        bestMoveValue = currentMoveValue;
                        bestMove = move;
                    }

                    if(DEBUG_MODE)
                    Console.WriteLine("Considering option ({0}): {1} , score: {2}", move, legalMoves[move], currentMoveValue);
                }

                lastMoveString = _command.getState().ToString();
                
                switch (bestMove)
                {
                    case 'u':
                        _command.MoveUp();
                        break;
                    case 'd':
                        _command.MoveDown();
                        break;
                    case 'l':
                        _command.MoveLeft();
                        break;
                    case 'r':
                        _command.MoveRight();
                        break;
                    case 'n':
                        Console.WriteLine("FAILURE");
                        return;
                }
                
                // clean-up
                
                currentMoveValue = bestMoveValue;
                bestMove = 'n';
                bestMoveValue = 0;

                if(DEBUG_MODE)
                    Console.ReadKey();
            }
        }

        private void GenerateLegalMoves()
        {
            string possibleMove;

            _state.ActualMove = false;
            if (_command.MoveUp())
            {
                possibleMove = _state.ToString();
                legalMoves['u'] = possibleMove;
                _command.MoveDown();
            }
            else
            {
                legalMoves['u'] = string.Empty;
            }

            if (_command.MoveDown())
            {
                possibleMove = _state.ToString();
                legalMoves['d'] = possibleMove;
                _command.MoveUp();
            }
            else
            {
                legalMoves['d'] = string.Empty;
            }

            if (_command.MoveLeft())
            {
                possibleMove = _state.ToString();
                legalMoves['l'] = possibleMove;
                _command.MoveRight();
            }
            else
            {
                legalMoves['l'] = string.Empty;
            }

            if (_command.MoveRight())
            {
                possibleMove = _state.ToString();
                legalMoves['r'] = possibleMove;
                _command.MoveLeft();
            }
            else
            {
                legalMoves['r'] = string.Empty;
            }
            _state.ActualMove = true;
        }

        private float score(String Board_Info)
        {
            float MaxScore = 25000;
            float currentScore = 0;

            string data = Board_Info.Replace(" ", string.Empty);
            int boardsize = data.Length;
            int eCol = 0;


            if (_command.VerifyBoard(Board_Info))
                return MaxScore;
            if (boardsize % 3 != 0)
            {
                Console.WriteLine("ERROR DETECTED Score requested on bad format board");
                return 0;
            }

            bool[] ColSolved = new bool[5];
            for (int i = 0; i != boardsize/3; ++i)
            {
                
                if (boardsize == 3)
                {
                    currentScore += (data[0] == data[2]) ? 2000 : 0;
                    currentScore += (data[0] == data[1] && data[2] == 'e') ? 1500 : 0;
                    currentScore += (data[1] == data[2] && data[0] == 'e') ? 1500 : 0;
                    return currentScore;
                    
                }

                int factoredBoardSize = boardsize / 3;

                if (data[i] == 'e' ||
                    data[i + 1 * factoredBoardSize] == 'e' ||
                    data[i + 2 * factoredBoardSize] == 'e')
                    eCol = i;

                /* Score the column*/

                // top row [i] == bottom row [i]
                if (data[i] == data[i + 2 * factoredBoardSize])
                {
                    currentScore += 2100;
                    ColSolved[i] = true;
                }

                // top row [i] == middle row [i]
                if (data[i] == data[i + 1 * factoredBoardSize])
                {
                    currentScore += 5;
                    
                    // and bottom row [i] == 'e'
                    if (data[i + 2 * factoredBoardSize] == 'e')
                    {
                        currentScore += 995;
                    }
                }

                // bottom row [i] == middle row [i]
                if (data[i + 2 * factoredBoardSize] == data[i + 1 * factoredBoardSize])
                {
                    currentScore+=5;
                    
                    // and top row [i]
                    if(data[i] == 'e')
                    {
                        currentScore +=995;
                    }
                }

                /*End Column*/

                // if we are at leftmost column
                if (i == 0)
                {
                    // top row [i] == bottom row [i] + 1 (to the right), and bottom row [i] == 'e'
                    if (data[i] == data[i + 2 * factoredBoardSize + 1] && data[i + 2 * factoredBoardSize] =='e' )
                    {
                        currentScore+=999;
                    }
                    // middle row [i] == bottom row [i] and top row[i] == 'e'
                    else if ( data[i +1 ] == data[i + 2 * factoredBoardSize] && data[i] =='e')
                    {
                        currentScore+=999;
                    }
                }

                // Last row
                else if (i == factoredBoardSize -1)
                {
                    // on the right side
                    // middle row [i] == bottom row [i] - 1 (to the left) and bottom row [i] == 'e'
                    if (data[i] == data[i + 2 * factoredBoardSize -1] && data[i + 2 * factoredBoardSize] =='e' )
                    {
                        currentScore+=999;
                    }
                    // ??? rightmost top row == bottom row [i] and first of middle row == 'e'
                    if (data[i - 1] == data[i + 2 * factoredBoardSize] && data[i] =='e')
                    {
                        currentScore+=999;
                    }
                }

                else
                {
                    // top row [i] == bottom row [i] - 1 (to the left) and bottom row [i] == 'e'
                    if (data[i] == data[i + 2 * factoredBoardSize - 1] && data[i + 2 * factoredBoardSize] =='e' )
                    {
                        currentScore+=999;
                    }

                    // top row [i] + 1 (to the right) == bottom row [i] and top row [i] == 'e'
                    if (data[i + 1] == data[i + 2 * factoredBoardSize] && data[i] =='e')
                    {
                        currentScore+=999;
                    }

                    // on the right side
                    // top row [i] == bottom row [i] - 1 (to the left) and bottom row [i] == 'e'
                    if (data[i] == data[i + 2 * factoredBoardSize - 1] && data[i + 2 * factoredBoardSize] =='e' )
                    {
                        currentScore+=999;
                    }

                    // top row [i] - 1 (to the left) == bottom row [i]  and top row [i] == 'e'
                    else if (data[i - 1] == data[i + 2 * factoredBoardSize] && data[i] =='e')
                    {
                        currentScore+=999;
                    }
                }
            }

            /* Give a weight to moveing closer to unsolved col */
            
            for (int i = 0; i != 5; ++i)
            {
                if (!ColSolved[i])
                    currentScore += 80 - (Math.Abs(i - eCol) * 10);
            }

            return currentScore;
        }
    }

    // might need for PriorityQueue
    class BoardToScore : IComparable<BoardToScore>
    {
        public string boardConfig { get; private set; }
        public int value { get; set; }

        public BoardToScore(string boardConfig, int value)
        {
            this.boardConfig = boardConfig;
            this.value = value;
        }

        public int CompareTo(BoardToScore other)
        {
            if (this.value < other.value)
            {
                return -1;
            }
            else if (this.value > other.value)
            {
                return 1;
            }
            else return 0;
        }
    }
}
