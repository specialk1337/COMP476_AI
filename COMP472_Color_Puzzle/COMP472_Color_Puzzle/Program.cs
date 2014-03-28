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
            StringBuilder output = new StringBuilder();
            StringBuilder unsolvedBoards = null;
            StringBuilder stats = null;
            StringBuilder trace = IO.trace ? new StringBuilder() : null;
            Stopwatch sw = new Stopwatch() ;

            string solution = string.Empty;
            string outputFile = string.Empty;
            long totalMoves = 0;
            long totalMs = 0;
            long ms = 0;

            long levelTotalMs = 0;
            long levelTotalMoves = 0;
            int solvedPuzzlesCounter = 0;

            if (IO.BenchmarkMode)
            {
                stats = new StringBuilder();

                // a whole lot of code to take the number of the last bench file and create a new one that is 1 number more
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

            for (int i = 0; i < IO.inputList.Count; i++ )
            {
                GameState state = new GameState(IO.inputList[i]);
                GameCommand command = new GameCommand(state);

                if (IO.AIMode())
                {
                    AIView AIview = new AIView(command);

                    if (IO.BenchmarkMode)
                    {
                        output.AppendLine("\r\n" + IO.inputList[i]);
                        output.AppendLine("===============");
                    }

                    sw.Reset();
                    sw.Start();
                    solution = AIview.play();
                    sw.Stop();
                    ms = sw.ElapsedMilliseconds;

                    if (string.IsNullOrEmpty(solution))
                    {
                        if (unsolvedBoards == null)
                        {
                            unsolvedBoards = new StringBuilder();
                        }

                        unsolvedBoards.AppendLine(IO.inputList[i]);

                        if (IO.BenchmarkMode)
                        {
                            output.AppendLine("=== Unable to solve ===");
                        }
                    }
                    else
                    {
                        solvedPuzzlesCounter++;
                    }
                    
                    if (IO.BenchmarkMode)
                    {
                        output.AppendLine(solution);
                        output.AppendLine(solution.Length.ToString() + " moves");
                    }
                    else
                    {
                        command.getState().ActualMove = true;
                        output.AppendLine(IO.Draw(command, solution));
                    }

                    if (IO.trace)
                    {
                        command.getState().ActualMove = true;
                        IO.Draw(command, solution);
                        trace.Append(IO.GetTrace());
                    }

                    output.AppendLine(ms.ToString() + "ms");
                    totalMoves += solution.Length;
                    totalMs += ms;

                    if (IO.BenchmarkMode)
                    {
                        levelTotalMoves += solution.Length;
                        levelTotalMs += ms;

                        if (i == 49)
                        {
                            stats.AppendLine("===== Solved " + solvedPuzzlesCounter + " / 50 Level 1 puzzles =====");
                            stats.AppendLine("Total level 1 moves: " + levelTotalMoves);
                            stats.AppendLine("Total level 1 ms: " + levelTotalMs);
                            stats.AppendLine("Average moves per puzzle: " + levelTotalMoves / 50.0);
                            stats.AppendLine("Average time per puzzle: " + levelTotalMs / 50.0 + "ms\r\n");
                            solvedPuzzlesCounter = 0;
                            levelTotalMoves = 0;
                            levelTotalMs = 0;
                        }
                        else if (i == 99)
                        {
                            stats.AppendLine("===== Solved " + solvedPuzzlesCounter + " / 50 Level 2 puzzles =====");
                            stats.AppendLine("Total level 2 moves: " + levelTotalMoves);
                            stats.AppendLine("Total level 2 ms: " + levelTotalMs);
                            stats.AppendLine("Average moves per puzzle: " + levelTotalMoves / 50.0);
                            stats.AppendLine("Average time per puzzle: " + levelTotalMs / 50.0 + "ms\r\n");
                            solvedPuzzlesCounter = 0;
                            levelTotalMoves = 0;
                            levelTotalMs = 0;
                        }
                        else if (i == 129)
                        {
                            stats.AppendLine("===== Solved " + solvedPuzzlesCounter + " / 30 Level 3 puzzles =====");
                            stats.AppendLine("Total level 3 moves: " + levelTotalMoves);
                            stats.AppendLine("Total level 3 ms: " + levelTotalMs);
                            stats.AppendLine("Average moves per puzzle: " + levelTotalMoves / 30.0);
                            stats.AppendLine("Average time per puzzle: " + levelTotalMs / 30.0 + "ms\r\n");
                            solvedPuzzlesCounter = 0;
                            levelTotalMoves = 0;
                            levelTotalMs = 0;
                        }
                        else if (i == 139)
                        {
                            stats.AppendLine("===== Solved " + solvedPuzzlesCounter + " / 10 Level 4 puzzles =====");
                            stats.AppendLine("Total level 4 moves: " + levelTotalMoves);
                            stats.AppendLine("Total level 4 ms: " + levelTotalMs);
                            stats.AppendLine("Average moves per puzzle: " + levelTotalMoves / 10.0);
                            stats.AppendLine("Average time per puzzle: " + levelTotalMs / 10.0 + "ms\r\n");
                            stats.AppendLine("===== Overall stats: =====");
                            stats.AppendLine("Total moves: " + totalMoves);
                            stats.AppendLine("Total ms: " + totalMs);
                            stats.AppendLine();
                        }
                    }
                    
                    state = null;
                    command = null;
                    AIview = null;
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
                if (!IO.BenchmarkMode)
                {
                    output.AppendLine(totalMoves.ToString());
                    output.AppendLine(totalMs.ToString() + "ms");
                }
                else
                {
                    File.AppendAllText(outputFile, stats.ToString());
                }

                File.AppendAllText(outputFile, output.ToString());

                if (unsolvedBoards != null)
                {
                    File.AppendAllText("unsolved.txt", unsolvedBoards.ToString());
                }

                if (IO.trace)
                {
                    if (File.Exists("game_trace.txt"))
                    {
                        File.Delete("game_trace.txt");
                    }
                    File.AppendAllText("game_trace.txt", trace.ToString());
                }
            }
            else
            {
                Console.WriteLine("Thanks for playing!");
                Console.ReadKey();
            }
        }
    }
}