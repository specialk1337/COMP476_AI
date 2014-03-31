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
            StringBuilder sbStats = IO.Report ? new StringBuilder() : null;
            StringBuilder sbMoveTracer = IO.Trace ? new StringBuilder() : null;
            Stopwatch sw = new Stopwatch();
            LevelStatsStruct[] levelStats = null;
            
            if (IO.Report)
                {
                levelStats = new LevelStatsStruct[4];
                for (int i = 0; i < 4; i++)
                levelStats[i].level = i + 1;
                }
            
            string solution = string.Empty;
            string outputFile = string.Empty;

            long totalMoves = 0;
            int moves = 0;
            long totalMs = 0;
            long ms = 0;
            int level = 0;

            if (IO.BenchmarkMode)
                {
                PrepBench(ref outputFile, ref sbStats);
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

                if (IO.AIplayer)
                {
                    AIView AIview = new AIView(command);
                    level = GetLevelMinus1(IO.inputList[i]);

                    if (IO.BenchmarkMode)
                    {
                        output.AppendLine("\r\n" + IO.inputList[i]);
                        output.AppendLine("===============");
                        output.AppendFormat("Level {0}\r\n\r\n", level + 1);
                    }

                    if (IO.Report)
                        {
                        levelStats[level].totalNbBoards++;
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
                        moves = (string.IsNullOrEmpty(solution) || solution == " ") ? 0 : solution.Length;

                        if (IO.Report)
                            {
                            levelStats[level].totalNbMs += ms;
                            levelStats[level].totalNbMoves += moves;
                            levelStats[level].solvedNbBoards++;
                            }
                    
                        if (IO.BenchmarkMode)
                        {
                            output.AppendLine(solution);
                            output.AppendLine("Moves" + moves);
                        }
                        else
                        {
                            state.ActualMove = true;
                            if (string.IsNullOrEmpty(solution) || solution == " ")
                            {
                                output.AppendLine();
                            }
                            else
                            {
                                output.AppendLine(IO.Draw(command, solution, i));
                            }
                            state.ActualMove = false;
                        }

                        if (IO.Trace)
                        {
                            state.BuildBoard(IO.inputList[i]);
                            state.ClearMoveHistroy();
                            state.ActualMove = true;
                            IO.Draw(command, solution, i);
                            sbMoveTracer.Append(IO.GetTrace());
                            state.ActualMove = true;
                        }
                    }

                    output.AppendLine(ms.ToString() + "ms");
                    totalMoves += moves;
                    totalMs += ms;

                    state = null;
                    command = null;
                    AIview = null;
                }
                else
                {
                    PlayerView view = new PlayerView(command);
                    view.play();

                    if (i < IO.inputList.Count)
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

            if (IO.AIplayer)
            {
                if (IO.Report)
                    {
                    for (int i = 0; i < 4; i++)
                        {
                        if (levelStats[i].totalNbBoards > 0)
                            {
                            levelStats[i].averageNbMoves = (double)levelStats[i].totalNbMoves / levelStats[i].totalNbBoards;
                            levelStats[i].averageNbMs = (double)levelStats[i].totalNbMs / levelStats[i].totalNbBoards;

                            sbStats.AppendFormat("===== Solved {0} / {1} Level {2} boards =====\r\n",
                                                 levelStats[i].solvedNbBoards, levelStats[i].totalNbBoards, levelStats[i].level);
                            sbStats.AppendLine("Total moves: " + levelStats[i].totalNbMoves);
                            sbStats.AppendLine("Total time: " + levelStats[i].totalNbMs + "ms");
                            sbStats.AppendLine("Average moves per puzzle: " + levelStats[i].averageNbMoves);
                            sbStats.AppendLine("Average time per puzzle: " + levelStats[i].averageNbMs + "ms\r\n");
                            }
                        }
                    }

                if (!IO.BenchmarkMode)
                {
                    output.AppendLine(totalMoves.ToString());
                    output.AppendLine(totalMs.ToString() + "ms");
                    if (IO.Report)
                    {
                        File.AppendAllText("report.txt", sbStats.ToString());
                    }

                }

                else
                {
                    File.AppendAllText(outputFile, sbStats.ToString());
                }

                File.AppendAllText(outputFile, output.ToString());

                
                if (unsolvedBoards != null)
                {
                    File.AppendAllText("unsolved.txt", unsolvedBoards.ToString());
                    Console.WriteLine("Unable to solve 1 or more puzzles. See unsolved.txt");
                    Console.ReadKey();
                }

                if (AIView.UsedIgnoredList)
                    {
                    output.Clear();
                    foreach (string ignoredBoard in AIView.ignoredBoards)
                        {
                        output.AppendLine(ignoredBoard);
                        }

                    File.AppendAllText("ignored.txt", output.ToString());
                    }

                if (IO.Trace)
                {
                    if (File.Exists("game_trace.txt"))
                    {
                        File.Delete("game_trace.txt");
                    }
                    File.AppendAllText("game_trace.txt", sbMoveTracer.ToString());
                }
            }
            else
            {
                Console.WriteLine("Thanks for playing!");
                Console.ReadKey();
            }
        }

        private static void PrepBench(ref string outputFile, ref StringBuilder stats)
        {
            int largestBenchSoFar = 0;
            Match nextBenchFile;
            string[] benchFiles = Directory.GetFiles(Directory.GetCurrentDirectory());
            for (int i = 0; i < benchFiles.Length; i++)
            {
                if (Path.GetFileName(benchFiles[i]).Contains("bench"))
                {
                    nextBenchFile = Regex.Match(Path.GetFileName(benchFiles[i]), @"\d+");

                    if (int.Parse(nextBenchFile.Groups[0].ToString()) > largestBenchSoFar)
                    {
                        largestBenchSoFar = int.Parse(nextBenchFile.Groups[0].ToString());
                    }
                }
            }

            if (largestBenchSoFar > 0)
            {
                outputFile = "bench" + (++largestBenchSoFar) + ".txt";
            }
            else
            {
                outputFile = "bench1.txt";
            }
        }

        private static int GetLevelMinus1(string board)
        {
            int level;
            if (board.Contains('p'))
                level = 3;
            else if (board.Contains('g'))
                level = 2;
            else if (board.Contains('y'))
                level = 1;
            else
                level = 0;
            return level;
        }
    }

    struct LevelStatsStruct
    {
        public int level { get; set; }
        public int solvedNbBoards { get; set; }
        public int totalNbBoards { get; set; }
        public int totalNbMoves { get; set; }
        public long totalNbMs { get; set; }
        public double averageNbMoves { get; set; }
        public double averageNbMs { get; set; }
    }
}
