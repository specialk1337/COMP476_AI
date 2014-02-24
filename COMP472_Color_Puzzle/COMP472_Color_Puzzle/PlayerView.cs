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
            //Console.WriteLine("you pressed {0}", keyPress.Key);
        }
        public void play()
        {
            _command.Draw();
            while (!_command.VarifyBoard())
            {
                if (getKeyPress())
                    _command.Draw();
            }
            Console.Clear();
            Console.WriteLine("You won! it took you {0} moves", _command.getMoveCount());
            _command.Draw();
            Console.WriteLine("You won! it took you {0} moves", _command.getMoveCount());

            Console.ReadKey();
            
        }
        public void draw()
        {
            
            //for (int i = 0; i != 3; i++)
            //{
            //    Console.WriteLine("+++++++++++++++++++++++++++++++++");
                
            //}
        }
    }
}
