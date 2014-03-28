using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Input;


namespace COMP472_Color_Puzzle
{
    class PlayerView
    {
        private GameCommand _command;
        
        public PlayerView(GameCommand command)
        {
            _command = command;
        }

        public bool getKeyPress()
        {
            ConsoleKeyInfo keyPress = Console.ReadKey();

            switch (keyPress.Key)
            {
                case(ConsoleKey.UpArrow):
                    _command.MoveUp();
                    break;
                case(ConsoleKey.DownArrow):
                    _command.MoveDown();
                    break;
                case(ConsoleKey.LeftArrow):
                    _command.MoveLeft();
                    break;
                case(ConsoleKey.RightArrow):
                    _command.MoveRight();
                    break;
                default:
                    return false;
            }
            return true;
        }

        public void play()
        {
            GameState state = _command.getState();
            state.ActualMove = true;
            _command.Draw();
            
            while (!_command.VerifyBoard(state.ToString()))
            {
                if (getKeyPress())
                    _command.Draw();
            }

            _command.Draw();
            Console.WriteLine();
            Console.WriteLine("You won! it took you {0} moves", _command.getMoveCount());

            Console.ReadKey();
        }
    }
}