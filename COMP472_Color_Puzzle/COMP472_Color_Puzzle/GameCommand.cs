﻿using System;
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
                //    _currentState.PushMove('U');
                return true;
            }
            else
            {
                /* Cannot swap up, empty chip is in top row */
                return false;
            }
        }

        public bool MoveDown()
        { 
            if (_currentState.EmptyIndex / (_currentState.boardsize / 3) != 2)
            {
                _currentState.swap(_currentState.EmptyIndex, _currentState.EmptyIndex + (_currentState.boardsize / 3));
                //    _currentState.PushMove('D');
                return true;
            }
            else
            {
                /* Cannot swap down, empty chip is in bottom row */
                return false;
            }
        }

        public bool MoveLeft()
        { 
            if (_currentState.EmptyIndex % (_currentState.boardsize / 3) != 0)
            {
                _currentState.swap(_currentState.EmptyIndex, _currentState.EmptyIndex - 1);
                //    _currentState.PushMove('L');
                return true;
            }
            else
            {
                /* Cannot swap left */
                return false;
            }
        }

        public bool MoveRight()
        {
            if (_currentState.EmptyIndex % (_currentState.boardsize / 3) != (_currentState.boardsize / 3) - 1)
            {
                _currentState.swap(_currentState.EmptyIndex, _currentState.EmptyIndex + 1);
                //    _currentState.PushMove('R');
                return true;
            }
            else
            {
                /* Cannot swap Right */
                return false;
            }
        }

        public void Draw()
        {
            _currentState.Draw();
        }

        public bool VerifyBoard()
        {
            for (int i = 0; i != _currentState.boardsize / 3; ++i)
            { 
                if(_currentState.GetChip(i) != _currentState.GetChip(i + 2 *(_currentState.boardsize / 3)))
                    return false;
            }
            return true;
        }
        public bool VerifyBoard(string possibleBoard)
        {

            string data = possibleBoard.Replace(" ", string.Empty);
            int boardsize = data.Length;
            for (int i = 0; i != boardsize / 3; ++i)
            {
                if (data[i] != data[i + 2 * boardsize / 3])
                {
                    return false;
                }
            }
            return true;
        }
    }
}