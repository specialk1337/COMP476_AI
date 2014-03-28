using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COMP472_Color_Puzzle
{
    class AIView
    {
        private static readonly string possibleMoves = "udlr";
        
        private GameCommand _command;
        private GameState _state;
        private Dictionary<string, int> closedList;
        private SortedList<int, BoardToScore> openList;
        private Dictionary<BoardToScore, int> ignoredList;

        private bool solutionFound = false;
        private readonly string initialBoard;

        public AIView(GameCommand command)
        {
            _command = command;
            _state = _command.getState();
            solutionFound = false;
            closedList = new Dictionary<string, int>();
            openList = new SortedList<int, BoardToScore>();
            ignoredList = new Dictionary<BoardToScore, int>();
            initialBoard = _state.ToString();
        }

        public string play()
        {
            BoardToScore currentMove = new BoardToScore(initialBoard, 0, "");
            openList.Add(currentMove.value, currentMove);

            while (true)
            {
                if (openList.Count == 0)
                {
                    if (ignoredList.Count == 0)
                        return string.Empty;
                    else
                    {
                        foreach (BoardToScore temp in ignoredList.Keys)
                        {
                            if (!closedList.ContainsKey(temp.boardConfig))
                            {
                                openList.Add(temp.value, temp);
                                ignoredList.Remove(temp);
                            }
                        }
                    }
                }

                if (solutionFound)
                {
                    return currentMove.moves;
                }

                currentMove = openList.Last().Value;
                openList.Remove(currentMove.value);
                closedList.Add(currentMove.boardConfig, currentMove.value);

                generateSuccessors(currentMove);
            }
        }

        private void generateSuccessors(BoardToScore current)
        {
            _state.BuildBoard(current.boardConfig);
            BoardToScore temp = null;
            string currentBoard;
            int currentBoardValue;
            char justCameFrom = ' '; // to avoid evaluating the square
            bool toContinue;

            if (current.moves.Length > 0)
            {
                switch (current.moves.Last())
                {
                    case 'u':
                        justCameFrom = 'd';
                        break;
                    case 'd':
                        justCameFrom = 'u';
                        break;
                    case 'l':
                        justCameFrom = 'r';
                        break;
                    case 'r':
                        justCameFrom = 'l';
                        break;
                }
            }
            foreach (char direction in possibleMoves)
            {
                toContinue = (direction == justCameFrom);

                if (!toContinue)
                {
                    switch (direction)
                    {
                        case 'u':
                            toContinue = !_command.MoveUp();
                            break;
                        case 'd':
                            toContinue = !_command.MoveDown();
                            break;
                        case 'l':
                            toContinue = !_command.MoveLeft();
                            break;
                        case 'r':
                            toContinue = !_command.MoveRight();
                            break;
                    }
                }

                if (toContinue)
                {
                    continue;
                }
                else
                {
                    currentBoard = _state.ToString();
                    if (!closedList.ContainsKey(currentBoard))
                    {
                        currentBoardValue = score(currentBoard);
                        if (currentBoardValue == 25000)
                        {
                            openList.Clear();
                            solutionFound = true;
                            openList.Add(currentBoardValue,
                                new BoardToScore(currentBoard, currentBoardValue, current.moves + direction));
                            _state.BuildBoard(initialBoard);
                            break;
                        }

                        temp = new BoardToScore(currentBoard, currentBoardValue, current.moves + direction);
                        if (!openList.ContainsKey(currentBoardValue))
                        {
                            openList.Add(currentBoardValue, temp);
                            temp = null;
                        }
                        else
                        {
                            if (openList[currentBoardValue].moves.Length > (current.moves + direction).Length)
                            {
                                ignoredList.Add(openList[currentBoardValue], currentBoardValue);
                                openList[currentBoardValue] = temp;
                            }
                            else
                            {
                                ignoredList.Add(temp, currentBoardValue);
                            }
                        }

                        _state.BuildBoard(current.boardConfig);
                    }
                }
            }
        }

        private int score(string data)
        {
            int MaxScore = 25000;
            int currentScore = 0;
            int boardsize = data.Length;
            int factoredBoardSize = boardsize / 3;
            int bottomRowOffset = 2 * factoredBoardSize;
            int middleRowOffset = factoredBoardSize;
            int eCol = 0;

            if (_command.VerifyBoard(data))
                return MaxScore;
            
            bool[] ColSolved = new bool[5];
            for (int i = 0; i != factoredBoardSize; ++i)
            {
                if (boardsize == 3)
                {
                    currentScore += (data[0] == data[2]) ? 2000 : 0;
                    currentScore += (data[0] == data[1] && data[2] == 'e') ? 1500 : 0;
                    currentScore += (data[1] == data[2] && data[0] == 'e') ? 1500 : 0;
                    return currentScore;
                }

                if ((data[i] == 'e')                            ||
                    (data[i + middleRowOffset] == 'e')    ||
                    (data[i + bottomRowOffset] == 'e'))
                    eCol = i;

                /* Score the column*/

                // top row [i] == bottom row [i]
                if (data[i] == data[i + bottomRowOffset])
                {
                    currentScore += 2100;
                    ColSolved[i] = true;
                }

                // top row [i] == middle row [i]
                if (data[i] == data[i + middleRowOffset])
                {
                    currentScore += 5;

                    // and bottom row [i] == 'e'
                    if (data[i + bottomRowOffset] == 'e')
                    {
                        currentScore += 995;
                    }
                }

                // bottom row [i] == middle row [i]
                if (data[i + bottomRowOffset] == data[i + middleRowOffset])
                {
                    currentScore += 5;

                    // and top row [i]
                    if (data[i] == 'e')
                    {
                        currentScore += 995;
                    }
                }

                /*End Column*/

                // if we are at leftmost column
                if (i == 0)
                {
                    // top row [i] == bottom row [i] + 1 (to the right), and bottom row [i] == 'e'
                    if (data[i] == data[i + bottomRowOffset + 1] &&
                        data[i + bottomRowOffset] == 'e')
                    {
                        currentScore += 999;
                    }
                    // middle row [i] == bottom row [i] and top row[i] == 'e'
                    else if (data[i + 1] == data[i + bottomRowOffset] && data[i] == 'e')
                    {
                        currentScore += 999;
                    }
                }

                // Last row
                else if (i == factoredBoardSize - 1)
                {
                    // on the right side
                    // middle row [i] == bottom row [i] - 1 (to the left) and bottom row [i] == 'e'
                    if (data[i] == data[i + bottomRowOffset - 1] &&
                        data[i + bottomRowOffset] == 'e')
                    {
                        currentScore += 999;
                    }
                    // ??? rightmost top row == bottom row [i] and first of middle row == 'e'
                    if (data[i - 1] == data[i + bottomRowOffset] && data[i] == 'e')
                    {
                        currentScore += 999;
                    }
                }

                else
                {
                    // top row [i] == bottom row [i] - 1 (to the left) and bottom row [i] == 'e'
                    if (data[i] == data[i + bottomRowOffset - 1] &&
                        data[i + bottomRowOffset] == 'e')
                    {
                        currentScore += 999;
                    }

                    // top row [i] + 1 (to the right) == bottom row [i] and top row [i] == 'e'
                    if (data[i + 1] == data[i + bottomRowOffset] && data[i] == 'e')
                    {
                        currentScore += 999;
                    }

                    // on the right side
                    // top row [i] == bottom row [i] - 1 (to the left) and bottom row [i] == 'e'
                    if (data[i] == data[i + bottomRowOffset - 1] &&
                        data[i + bottomRowOffset] == 'e')
                    {
                        currentScore += 999;
                    }

                    // top row [i] - 1 (to the left) == bottom row [i]  and top row [i] == 'e'
                    else if (data[i - 1] == data[i + bottomRowOffset] && data[i] == 'e')
                    {
                        currentScore += 999;
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
}