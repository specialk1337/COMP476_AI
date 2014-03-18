using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
namespace COMP472_Color_Puzzle
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            GameIO IO = new GameIO();

            GameState state = new GameState(IO.ChooseInitialBoard());
            GameCommand command = new GameCommand(state);

            if (IO.AIMode())
            {

                //PlayerView view = new PlayerView(command);
                AIView AIview = new AIView(command);
               // do
                //{
                    Stopwatch sw = Stopwatch.StartNew();
                    AIview.play();
                    //System.Threading.Thread.Sleep(2000);
                    sw.Stop();
                    //state.BuildBoard(IO.nextBoard());

                    FileStream fs = new FileStream("output_d2.txt", FileMode.Create);
                    // First, save the standard output.
                    
                    StreamWriter sw1 = new StreamWriter(fs);
                    Console.SetOut(sw1);
                    command.Draw();
                    Console.Write(sw.ElapsedMilliseconds);
                    Console.WriteLine("ms");
                    sw1.Close();
                //}
               // while (IO.moreBoards());

                //Console.WriteLine("Press any key to view final values");
                ///* Output to display */
                //Console.ReadKey();
                //command.Draw();
                //Console.Write("Elapsed Time (ms): ");
                //Console.WriteLine(sw.ElapsedMilliseconds);


                //Console.WriteLine("Press anykey to write data to file");
                //Console.ReadKey();
                ///* Output to file */
                //FileStream fs = new FileStream("output_d2.txt", FileMode.Create);
                //// First, save the standard output.
                //TextWriter tmp = Console.Out;
                //StreamWriter sw1 = new StreamWriter(fs);
                //Console.SetOut(sw1);
                //command.Draw();
                //Console.Write(sw.ElapsedMilliseconds);
                //Console.WriteLine("ms");
                //sw1.Close();

                TextWriter tmp = Console.Out;
                Console.SetOut(tmp);
            }
            else
            {
                PlayerView view = new PlayerView(command);
                
                view.play();


            }

            //Console.WriteLine("Thank you for playing");
            //Console.ReadKey();
        }
    }
}
