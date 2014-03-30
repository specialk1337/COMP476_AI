using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace COMP472_Color_Puzzle
{
    class GameIO
    {
        public List<string> inputList   { get; private set; }
        public bool BenchmarkMode       { get; private set; }
        public bool AIplayer            { get; private set; }
        public bool Trace               { get; private set; }
        public bool Report              { get; private set; }

        private StringBuilder sbNewBoard;
        private StringBuilder sbMoveTracer;
        private Random rand;

        public GameIO()
        {
            inputList = new List<string>();
            Console.WriteLine("Would you like to enter the input manually, read it from a file, or use benchmarking?");
            string answer = string.Empty;
            bool done = false;
            do
            {
                Console.WriteLine("Press 'f' for file, 'k' for keyboard, or 'b' for benchmarking.");
                Console.WriteLine("Options:");
                Console.WriteLine("-'t'\t traces every move (works in AI only): ");
                Console.WriteLine("-'r'\t generates a statistics (enable by default in benchmarking)");
                answer = Console.ReadLine();
                done = (answer.StartsWith("f")) || (answer.StartsWith("k")) || (answer.StartsWith("b"));
                if (!done)
                {
                    Console.WriteLine("Oops! Try again.");
                }
            } while (!done);

            Trace = answer.Contains('t');
            sbMoveTracer = Trace ? new StringBuilder() : null;
            Report = answer.Contains('r');
            
            switch (answer[0])
            {
                case 'f':
                    do
                    {
                        ReadInputFromFile();
                    } while (inputList.Count < 1);
                    break;
                case 'k':
                    sbNewBoard = new StringBuilder();
                    rand = new Random();
                    do
                    {
                        ReadInputFromKeyboard();
                        Console.WriteLine("Would you like to enter more boards? (y/n): ");
                    } while (Console.ReadLine() != "n");
                    break;
                case 'b':
                    sbNewBoard = new StringBuilder();
                    rand = new Random();
                    Benchmark();
                    BenchmarkMode = true;
                    AIplayer = true;
                    Report = true;
                    return;
            }

            Console.Write("Who plays, you or the AI? Press 'a' for AI or 'p' for Player: ");

            do
            {
                answer = Console.ReadLine();
                done = (answer != "a") || (answer != "p");
                if (!done)
                {
                    Console.WriteLine("Oops! Try again.");
                }
            } while (!done);

            AIplayer = (answer == "a") ? true : false;
        }

        [STAThread]
        private void ReadInputFromFile()
        {
            string FilePath = GetPath();
            string[] file = File.ReadAllLines(FilePath, Encoding.UTF8);
            string boardToTest;

            foreach (string startingBoard in file)
            {
                if (!string.IsNullOrEmpty(startingBoard))
                {
                    boardToTest = startingBoard;
                    if (ValidateInput(ref boardToTest))
                    {
                        inputList.Add(boardToTest);
                    }
                    else
                    {
                        Console.WriteLine("Skipping " + boardToTest);
                    }
                }
            }
        }

        private void ReadInputFromKeyboard()
        {
            Console.WriteLine("\nPlease enter your inital board configuration, all in one line.");
            Console.WriteLine("Make sure to include one 'e'.");
            Console.WriteLine("Otherwise, you may type '1', '2', '3', '4' to generate a random level.");
            string inputString;
            do
            {
                inputString = Console.ReadLine();
                if (inputString.Contains('1') || inputString.Contains('2') ||
                    inputString.Contains('3') || inputString.Contains('4'))
                {
                    if (Regex.IsMatch(inputString, @"\d+x\d+"))
                    {
                    int level = 0;
                    int times = 0;
                    
                        for (Match m = Regex.Match(inputString, @"(\d+)x(\d+)"); m.Success; m = m.NextMatch())
                        {
                            level = int.Parse(m.Groups[1].ToString());
                            times = int.Parse(m.Groups[2].ToString());
                            for (int i = 0; i < times; i++)
                                {
                                inputList.Add(generateRandomBoard(level));
                                }
                        }
                    }
                    else
                    {
                        foreach (char level in inputString)
                        {
                            switch (level)
                            {
                                case '1':
                                    inputList.Add(generateRandomBoard(1));
                                    break;
                                case '2':
                                    inputList.Add(generateRandomBoard(2));
                                    break;
                                case '3':
                                    inputList.Add(generateRandomBoard(3));
                                    break;
                                case '4':
                                    inputList.Add(generateRandomBoard(4));
                                    break;
                                default:
                                    continue;
                            }
                        }
                    }
                }
                else
                {
                    if (ValidateInput(ref inputString))
                    {
                    inputList.Add(inputString);
                    }
                }
            }
            while (inputList.Count < 1);
            Console.WriteLine("Added successfully.");
        }

        [STAThread]
        public static string GetPath()
        {
            string filePath;
                        
            OpenFileDialog browseFile = new OpenFileDialog();
            browseFile.Filter = "Text File (*.txt) | *.txt";
            browseFile.Title = "Browse Sample Input";
            if (browseFile.ShowDialog() == DialogResult.Cancel)
                return "null";
            try
            {
                filePath = browseFile.FileName;
            }
            catch (Exception)
            {
                MessageBox.Show("Error opening file", "File Error",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                filePath = "null";
            }
            return @filePath;
        }

        private static bool ValidateInput(ref string boardToTest)
        {
            Dictionary<char, int> Chars = new Dictionary<char,int>();

            bool eCharFound = false;
            bool isValidLevel = false;
            int charCount = 0;
            StringBuilder formattedString = new StringBuilder();

            foreach (char inputChar in boardToTest)
            {
                if (char.IsLetter(inputChar))
                {
                    switch (inputChar)
                    {
                        case 'r': case 'b': case 'w': case 'y': case 'g': case 'p':
                            IncrementCharCount(Chars, inputChar);
                            charCount++;
                            formattedString.Append(inputChar);
                            break;

                        case 'e':
                            if (!eCharFound)
                            {
                                eCharFound = true;
                                IncrementCharCount(Chars, inputChar);
                                charCount++;
                                formattedString.Append(inputChar);
                            }
                            else
                            {
                                Console.WriteLine("Input string contains more than 1 'e'.");
                                return false;
                            }
                            break;

                        default:
                            Console.WriteLine(" '" + inputChar + "' is not a valid character.");
                            Console.WriteLine("Please use 'r', 'b', 'w', 'y', 'g', 'p', and 'e' only.");
                            return false;
                    }
                }
            }

            if (charCount != 15)
            {
                Console.WriteLine("Incorrect number of characters entered.");
                Console.WriteLine("A board must contain 15 characters, and you've entered " + charCount);
                return false;
             }

            if (Chars.ContainsKey('r') && Chars.ContainsKey('b') && Chars.ContainsKey('w'))
            {
                if (Chars['r'] == 6)
                {
                    // Level 1: 6 red + 6 blue + 2 white
                    if (Chars['b'] == 6 && Chars['w'] == 2)
                    {
                        isValidLevel = true;
                    }
                    // Level 2: 6 red + 4 blue + 2 white + 2 yellow
                    else if (Chars.ContainsKey('y') && Chars['b'] == 4 && Chars['w'] == 2 && Chars['y'] == 2)
                    {
                        isValidLevel = true;
                    }
                }
                else if (Chars['r'] == 4 && Chars.ContainsKey('y') && Chars.ContainsKey('g'))
                {
                    // Level 3: 4 red + 4 blue + 2 white + 2 yellow + 2 green
                    if (Chars['b'] == 4 && Chars['w'] == 2 && Chars['y'] == 2 && Chars['g'] == 2)
                    {
                        isValidLevel = true;
                    }
                    // Level 4: 4 red + 2 blue + 2 white + 2 yellow + 2 green + 2 pink
                    else if (Chars.ContainsKey('p') && Chars['b'] == 2 && Chars['w'] == 2 &&
                             Chars['y'] == 2 && Chars['g'] == 2 && Chars['p'] == 2)
                    {
                        isValidLevel = true;
                    }
                }
            }

            if (isValidLevel && eCharFound)
            {
                boardToTest = formattedString.ToString();
                return true;
            }
            else if (!eCharFound)
            {
                Console.WriteLine("No 'e' was entered.");
                return false;
            }
            else
            {
                Console.WriteLine("Level configuration: {0} is invalid. Must conform to 1 of the following:", boardToTest);
                Console.WriteLine("Level 1: 6 red + 6 blue + 2 white OR");
                Console.WriteLine("Level 2: 6 red + 4 blue + 2 white + 2 yellow OR");
                Console.WriteLine("Level 3: 4 red + 4 blue + 2 white + 2 yellow + 2 green OR");
                Console.WriteLine("Level 4: 4 red + 2 blue + 2 white + 2 yellow + 2 green + 2 pink OR");
                Console.WriteLine("--- and of course, include 1 'e' ---");
                return false;
            }
        }

        private static void IncrementCharCount(Dictionary<char, int> charCount, char charToAdd)
        {
            if (charCount.ContainsKey(charToAdd))
            {
                charCount[charToAdd]++;
            }
            else
            {
                charCount.Add(charToAdd, 1);
            }
        }

        private void Benchmark()
        {
            int i;
            for (i = 0; i < 50; i++) // 50 in 10 sec
            {
                inputList.Add(generateRandomBoard(1));
            }

            for (i = 0; i < 50; i++) // 50 in 12 sec
            {
                inputList.Add(generateRandomBoard(2));
            }

            for (i = 0; i < 30; i++) // 30 in 30 sec
            {
                inputList.Add(generateRandomBoard(3));
            }

            for (i = 0; i < 10; i++) // 10 in 20 sec
            {
                inputList.Add(generateRandomBoard(4));

            }
        }

        private string generateRandomBoard(int level)
        {
            sbNewBoard.Clear();
            char[] newBoard = null;
            switch (level)
            {
                case 1:
                    newBoard = new char[] { 'e', 'r', 'r', 'r', 'r', 'r', 'r', 'b', 'b', 'b', 'b', 'b', 'b', 'w', 'w' };
                    break;
                case 2:
                    newBoard = new char[] { 'e', 'r', 'r', 'r', 'r', 'r', 'r', 'b', 'b', 'b', 'b', 'w', 'w', 'y', 'y' };
                    break;
                case 3:
                    newBoard = new char[] { 'e', 'r', 'r', 'r', 'r', 'b', 'b', 'b', 'b', 'w', 'w', 'y', 'y', 'g', 'g' };
                    break;
                case 4:
                    newBoard = new char[] { 'e', 'r', 'r', 'r', 'r', 'b', 'b', 'w', 'w', 'y', 'y', 'g', 'g', 'p', 'p' };
                    break;
            }

            int r;
            char temp;
            for (int i = 0; i < newBoard.Length; i++)
            {
                r = rand.Next(0, i);
                temp = newBoard[i];
                newBoard[i] = newBoard[r];
                newBoard[r] = temp;
            }

            foreach (char letter in newBoard)
            {
                sbNewBoard.Append(letter);
            }
            return sbNewBoard.ToString();
        }

        public string Draw(GameCommand _command, string moves, int gameCounter)
        {
            GameState _state = _command.getState();
            if (Trace)
            {
                sbMoveTracer.Clear();
                sbMoveTracer.AppendLine("\r\n==================================================\r\n");
                sbMoveTracer.AppendLine("Game " + (gameCounter + 1));
                _command.Draw(ref sbMoveTracer);
            }
            
            for (int i = 0 ; i < moves.Length; i++)
            {
                switch (moves[i])
                {
                    case 'u':
                        _command.MoveUp();
                        break;
                    case 'd':
                        _command.MoveDown();
                        break;
                    case 'l':
                        _command.MoveLeft();
                        break;
                    case 'r':
                        _command.MoveRight();
                        break;
                }

                if (Trace)
                {
                    _command.Draw(ref sbMoveTracer);
                    for (int j = 0; j <= i; j++)
                        sbMoveTracer.Append(moves[j]);
                    sbMoveTracer.AppendLine();
                }
            }

            if (Trace)
                sbMoveTracer.AppendLine(moves.Length.ToString() + " moves");

            return _command.getState().GetMoveHistory();
        }

        public string GetTrace()
        {
            return (Trace) ? sbMoveTracer.ToString() : string.Empty;
        }
    }
}