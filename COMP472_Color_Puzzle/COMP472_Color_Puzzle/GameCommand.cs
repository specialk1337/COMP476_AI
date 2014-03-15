using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COMP472_Color_Puzzle
{
    class GameCommand
    {
        private GameState _currentState;

        public int getMoveCount(){return _currentState.moveCounter;}

        public GameCommand(GameState currentState)
        {
            _currentState = currentState;
        }

        public GameState getState()
        {
            return _currentState;
        }

        public void MoveUp()
        {
            if (_currentState.EmptyIndex / (_currentState.boardsize / 3) != 0)
            {
                _currentState.swap(_currentState.EmptyIndex, _currentState.EmptyIndex - (_currentState.boardsize / 3));
                _currentState.PushMove('U');
            }
            else
            {
                /* Cannot swap up, empty chip is in top row */
            }
        }
        public void MoveDown()
        { 
            if (_currentState.EmptyIndex / (_currentState.boardsize / 3) != 2)
            {
                _currentState.swap(_currentState.EmptyIndex, _currentState.EmptyIndex + (_currentState.boardsize / 3));
                _currentState.PushMove('D');
            }
            else
            {
                /* Cannot swap down, empty chip is in bottom row */
            }
        }
        public void MoveLeft()
        { 
            if (_currentState.EmptyIndex % (_currentState.boardsize / 3) != 0)
            {
                _currentState.swap(_currentState.EmptyIndex, _currentState.EmptyIndex - 1);
                _currentState.PushMove('L');
            }
            else
            {
                /* Cannot swap left */
            }
        }
        public void MoveRight()
        {
            if (_currentState.EmptyIndex % (_currentState.boardsize / 3) != (_currentState.boardsize / 3)-1)
            {
                _currentState.swap(_currentState.EmptyIndex, _currentState.EmptyIndex +1);
                _currentState.PushMove('R');
            }
            else
            {
                /* Cannot swap Right */
            }
        }
        public void Draw()
        {
            _currentState.Draw();
        }
        public bool VarifyBoard()
        {

            for (int i = 0; i != _currentState.boardsize / 3; ++i)
            { 
                if(_currentState.GetChip(i) != _currentState.GetChip(i+ 2*(_currentState.boardsize / 3)))
                    return false;
                
            }
            return true;
        }
    }
}
