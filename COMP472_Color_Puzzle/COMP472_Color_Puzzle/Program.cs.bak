using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace COMP472_Color_Puzzle
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            GameState state = new GameState();
            GameCommand command = new GameCommand(state);
            //PlayerView view = new PlayerView(command);
            AIView view = new AIView(command);

            view.play();

            //Hardcoded
            TextWriter output = File.CreateText(@"C:\output.txt");
            output.WriteLine(state.Moves.ToString());
            output.Close();
        }
    }
}
