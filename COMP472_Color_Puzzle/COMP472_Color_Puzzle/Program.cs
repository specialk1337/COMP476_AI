using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace COMP472_Color_Puzzle
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            GameIO IO = new GameIO();
            string outputFile = string.Empty;

            if (IO.BenchmarkMode)
            {
                // a whole lot of code to take the number of the last bench file and create a new one
                string[] benchFiles = Directory.GetFiles(Directory.GetCurrentDirectory());
                for (int i = 0; i < benchFiles.Length - 1; i++)
                {
                    if ((Path.GetFileName(benchFiles[i]).StartsWith("bench")) && 
                        (!Path.GetFileName(benchFiles[i + 1]).StartsWith("bench")))
                    {
                        Match  nextBenchFile = Regex.Match(Path.GetFileName(benchFiles[i]), @"\d+");
                        int nextBenchFileNumber = int.Parse(nextBenchFile.Groups[0].ToString());
                        outputFile = "bench" + ++nextBenchFileNumber + ".txt";
                        break;
                    }
                }
                
                benchFiles = null;
                if (string.IsNullOrEmpty(outputFile))
                {
                    outputFile = "bench1.txt";
                }
            }
            else
            {
                outputFile = "output.txt";
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }
            }

            StringBuilder output = new StringBuilder();
            StringBuilder unsolvedBoards = new StringBuilder();
            long totalMoves = 0;
            long totalMs = 0;
            int solvedPuzzlesCounter = 0;

            for (int i = 0; i < IO.inputList.Count; i++ )
            {
                GameState state = new GameState(IO.inputList[i]);
                GameCommand command = new GameCommand(state);

                if (IO.AIMode())
                {
                    AIView AIview = new AIView(command);

                    Stopwatch sw = Stopwatch.StartNew();
                    try
                    {
                        AIview.play();
                        ++solvedPuzzlesCounter;
                    }
                    catch (Exception e)
                    {
                        unsolvedBoards.AppendLine(IO.inputList[i]);
                    }
                    
                    sw.Stop();

                    output.AppendLine(state.GetMoveHistory());
                    output.AppendLine(sw.ElapsedMilliseconds.ToString() + "ms");
                    totalMoves += state.OptimalCounter;
                    totalMs += sw.ElapsedMilliseconds;

                    if (IO.BenchmarkMode)
                    {
                        if (i == 49)
                        {
                            output.AppendLine("===== Solved " + solvedPuzzlesCounter + " / 50 Level 1 puzzles =====");
                            solvedPuzzlesCounter = 0;
                        }
                        else if (i == 99)
                        {
                            output.AppendLine("===== Solved " + solvedPuzzlesCounter + " / 50 Level 2 puzzles =====");
                            solvedPuzzlesCounter = 0;
                        }
                        else if (i == 129)
                        {
                            output.AppendLine("===== Solved " + solvedPuzzlesCounter + " / 30 Level 3 puzzles =====");
                            solvedPuzzlesCounter = 0;
                        }
                        else if (i == 139)
                        {
                            output.AppendLine("===== Solved " + solvedPuzzlesCounter + " / 10 Level 4 puzzles =====");
                        }
                    }
                }
                else
                {
                    PlayerView view = new PlayerView(command);
                    view.play();

                    if (++solvedPuzzlesCounter < IO.inputList.Count)
                    {
                        Console.Write("Load next board? ");
                        bool done = false;
                        string answer;
                        do
                        {
                            answer = Console.ReadLine();
                            done = (answer != "y") || (answer != "n");
                            if (!done)
                            {
                                Console.WriteLine("Oops! Try again.");
                            }
                        } while (!done);

                        if (answer == "n")
                            break;
                    }
                }
            }

            if (IO.AIMode())
            {
                output.AppendLine(totalMoves.ToString());
                output.AppendLine(totalMs.ToString() + "ms");
                File.AppendAllText(outputFile, output.ToString());
                File.AppendAllText("unsolved.txt", unsolvedBoards.ToString());
            }
            else
            {
                Console.WriteLine("Thanks for playing!");
                Console.ReadKey();
            }
        }
    }
}