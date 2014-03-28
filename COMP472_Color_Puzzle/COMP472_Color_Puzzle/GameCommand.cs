using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COMP472_Color_Puzzle
{
    class GameCommand
    {
        private GameState _currentState;
        
        public int getMoveCount()
        {
            return _currentState.moveCounter;
        }

        public GameCommand(GameState currentState)
        {
            _currentState = currentState;
        }

        public GameState getState()
        {
            return _currentState;
        }

        public bool MoveUp()
        {
            if (_currentState.EmptyIndex / (_currentState.boardsize / 3) != 0)
            {
                _currentState.swap(_currentState.EmptyIndex, _currentState.EmptyIndex - (_currentState.boardsize / 3));
                return true;
            }
            return false;
        }

        public bool MoveDown()
        { 
            if (_currentState.EmptyIndex / (_currentState.boardsize / 3) != 2)
            {
                _currentState.swap(_currentState.EmptyIndex, _currentState.EmptyIndex + (_currentState.boardsize / 3));
                return true;
            }
            return false;
        }

        public bool MoveLeft()
        { 
            if (_currentState.EmptyIndex % (_currentState.boardsize / 3) != 0)
            {
                _currentState.swap(_currentState.EmptyIndex, _currentState.EmptyIndex - 1);
                return true;
            }
            return false;
        }

        public bool MoveRight()
        {
            if (_currentState.EmptyIndex % (_currentState.boardsize / 3) != (_currentState.boardsize / 3) - 1)
            {
                _currentState.swap(_currentState.EmptyIndex, _currentState.EmptyIndex + 1);
                return true;
            }
            return false;
        }

        public void Draw()
        {
            _currentState.Draw();
        }

        public bool VerifyBoard(string boardConfig)
        {
            int boardsize = boardConfig.Length;
            for (int i = 0; i != boardsize / 3; ++i)
            {
                if (boardConfig[i] != boardConfig[i + 2 * boardsize / 3])
                {
                    return false;
                }
            }
            return true;
        }

        public string Draw(ref StringBuilder sbMoveTracer)
        {
            string TheBoard = _currentState.ToString();
            sbMoveTracer.AppendLine("+++++++++++++++++++++++++++++++++++++++++\r\n");
            for (int i = 0; i != 3; ++i)
            {
                sbMoveTracer.AppendLine("+_______________________________________+");
                sbMoveTracer.AppendFormat("+ | {0} | + | {1} | + | {2} | + | {3} | + | {4} | +\r\n",
                    TheBoard[(i * 5) + 0], TheBoard[(i * 5) + 1], TheBoard[(i * 5) + 2],
                    TheBoard[(i * 5) + 3], TheBoard[(i * 5) + 4]);
                sbMoveTracer.AppendLine("+_______________________________________+");
            }
            sbMoveTracer.AppendLine("+++++++++++++++++++++++++++++++++++++++++\r\n");

            return sbMoveTracer.ToString();
        }
    }
}