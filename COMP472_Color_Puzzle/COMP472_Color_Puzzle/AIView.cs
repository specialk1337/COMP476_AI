using System;
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
        private Dictionary<string, int> closedList; // board config to move taken
        private List<BoardToScore> openList;
        private Dictionary<string, int> evaluatedSoFar;
        private bool solutionFound = false;
        private static string possibleMoves = "udlr";

        public AIView(GameCommand command)
        {
            _command = command;
            _state = _command.getState();
            solutionFound = false;
            legalMoves = new Dictionary<char, string>(4);
            legalMoves.Add('u', string.Empty);
            legalMoves.Add('d', string.Empty);
            legalMoves.Add('l', string.Empty);
            legalMoves.Add('r', string.Empty);
            closedList = new Dictionary<string, int>();
            openList = new List<BoardToScore>();
            evaluatedSoFar = new Dictionary<string, int>();
        }

        public void play()
        {
            bool DmitriChanges = true;

            BoardToScore currentMove = new BoardToScore(_state.ToString(), 0, "");
            
            // remove once DmitriChanges are fully integrated
            string initialState = _state.ToString();
            string lastMoveString = string.Empty;
            char bestMove = 'n';
            int bestMoveValue = (int)score(initialState);
            int currentMoveValue = 0;

            while (true)
            {
                if (DmitriChanges)
                {
                    foreach (char direction in currentMove.moves)
                    {
                        switch (direction)
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
                        }
                    }
                    closedList.Add(currentMove.boardConfig, currentMove.value);
                    currentMove.moves = string.Empty;
                }

                if (DEBUG_MODE)
                {
                    _command.Draw();
                }

                if (_command.VerifyBoard())
                {
                    if (DEBUG_MODE)
                    {
                    Console.WriteLine("SUCCESS");
                    Console.ReadKey();
                    }
                    return;
                }

                if (DmitriChanges)
                {
                    generateSuccessors(currentMove);
                    if (openList.Count > 1)
                    {
                        openList.Sort();
                    }
                    currentMove = openList.Last();
                    openList.Clear();
                    evaluatedSoFar.Clear();
                }
                
                else
                {   // old way of doing things
                    generateSuccessors();

                    foreach (char move in legalMoves.Keys)
                    {
                        // if no legal move exists, go to next move
                        if (string.IsNullOrEmpty(legalMoves[move]))
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(lastMoveString) && legalMoves[move] == lastMoveString)
                            continue;

                        currentMoveValue = (int)score(legalMoves[move]);
                        if (currentMoveValue >= bestMoveValue)
                        {
                            bestMoveValue = currentMoveValue;
                            bestMove = move;
                        }

                        if (DEBUG_MODE)
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
                }

                if (DEBUG_MODE)
                {
                    Console.ReadKey();
                }
            }
        }

        private void generateSuccessors(BoardToScore current)
        {
            _state.ActualMove = false;
            
            foreach (char direction in possibleMoves)
            {
                evaluateMove(current, direction);

                if (solutionFound)
                {
                    break;
                }
            }
            _state.ActualMove = true;
        }

        private void evaluateMove(BoardToScore current, char direction)
        {
            string possibleMove;
            int possibleMoveScore;

            switch (direction)
            {
                case 'u':
                    if (!_command.MoveUp())
                        return;
                    break;
                case 'd':
                    if (!_command.MoveDown())
                        return;
                    break;
                case 'l':
                    if (!_command.MoveLeft())
                        return;
                    break;
                case 'r':
                    if (!_command.MoveRight())
                        return;
                    break;
            }

            possibleMove = _state.ToString();
            if (!evaluatedSoFar.ContainsKey(possibleMove)) // to optimize
            {
                possibleMoveScore = score(possibleMove);

                if (DEBUG_MODE)
                {
                    Console.WriteLine("direction: {0}, board: {1}, score: {2}", direction, possibleMove, possibleMoveScore);
                }


                if (possibleMoveScore == 25000)
                {
                    solutionFound = true;
                    openList.Clear();
                }
                else
                {
                    evaluatedSoFar.Add(current.boardConfig + direction, possibleMoveScore);
                }

                if (!closedList.ContainsKey(possibleMove))
                {
                    openList.Add(new BoardToScore(possibleMove, possibleMoveScore, current.moves + direction));
                }
            }

            switch (direction)
            {
                case 'u':
                    _command.MoveDown();
                    break;
                case 'd':
                    _command.MoveUp();
                    break;
                case 'l':
                    _command.MoveRight();
                    break;
                case 'r':
                    _command.MoveLeft();
                    break;
            }
        }

// OLD CODE:
//===============================================================================
        private void generateSuccessors()
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
//===============================================================================

        private int score(String Board_Info)
        {
            int MaxScore = 25000;
            int currentScore = 0;

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
        public string boardConfig { get; set; }
        public int value { get; set; }
        public string moves { get; set; }

        public BoardToScore(string boardConfig, int value, string moves)
        {
            this.boardConfig = boardConfig;
            this.value = value;
            this.moves = moves;
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
