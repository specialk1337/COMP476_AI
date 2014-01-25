using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace COMP472_Color_Puzzle
{
    class Program
    {
        static void Main(string[] args)
        {
            GameState state = new GameState();
            GameCommand command = new GameCommand(state);
            PlayerView view = new PlayerView(command);

            view.play();
        }
    }
}
