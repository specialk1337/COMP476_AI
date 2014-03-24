using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace COMP472_Color_Puzzle
{
    class GameIO
    {
        public List<string> inputList { get; private set; }
        public bool BenchmarkMode { get; private set; }
        private bool AIplayer;
        
        public GameIO()
        {
            inputList = new List<string>();
            Console.WriteLine("Would you like to enter the input yourself, read it from a file, or use benchmarking?");
            string answer = string.Empty;
            bool done = false;
            do
            {
                Console.Write("Please enter 'f' for file, 'k' for keyboard, or 'b' for benchmarking: ");
                answer = Console.ReadLine();
                done = (answer != "f") || (answer != "k");
                if (!done)
                {
                    Console.WriteLine("Oops! Try again.");
                }
            } while (!done);

            switch (answer)
            {
                case "f":
                    do
                    {
                        ReadInputFromFile();
                    } while (inputList.Count < 1);
                    break;
                case "k":
                    do
                    {
                        ReadInputFromKeyboard();
                        Console.WriteLine("Would you like to enter another one? (y/n): ");
                    } while (Console.ReadLine() != "n");
                    break;
                case "b":
                    Benchmark();
                    BenchmarkMode = true;
                    AIplayer = true;
                    return;
            }

            Console.WriteLine("Great! now would you like to play the game, or see how fast the Computer can solve the puzzle? ");
            Console.Write("Press 'a' for AI or 'p' for Player: ");

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

        public bool AIMode()
        {
            return AIplayer;
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
            Console.WriteLine("Make sure to include 1 'e':");
            string inputString;
            do
            {
                inputString = Console.ReadLine();
                if (ValidateInput(ref inputString))
                {
                    Console.WriteLine("Added successfully.");
                    inputList.Add(inputString);
                }

            }
            while (inputList.Count < 1);
        }

        //public string ChooseInitialBoard()
        //{
        //    string returnString = string.Empty;
        //    int index = 0;
        //    int choice = -1;
        //    string answer = string.Empty;
        //    bool done = false;

        //    if (inputList.Count > 0)
        //    {
        //        Console.WriteLine("Which initial configuration would you like to start with?");
        //        foreach (string sBoardConfig in inputList)
        //        {
        //            Console.WriteLine("[" + (index++) + "] : " + sBoardConfig);
        //        }
        //        do
        //        {
        //            Console.Write("Please make your choice: ");
        //            answer = Console.ReadLine();
        //            choice = int.Parse(answer);
        //            if (choice >= 0 && choice < index)
        //            {
        //                done = true;
        //            }
        //            else
        //            {
        //                Console.WriteLine("Oops! Try again");
        //            }
        //        } while (!done);
        //        returnString = inputList[choice];
        //    }
        //    else
        //    {
        //        Console.WriteLine("No initial board configurations are available.");
        //        ReadInputFromKeyboard();
        //        returnString = inputList[0];
        //    }
        //    return returnString;
        //}

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
                            formattedString.Append(inputChar + " ");
                            break;

                        case 'e':
                            if (!eCharFound)
                            {
                                eCharFound = true;
                                IncrementCharCount(Chars, inputChar);
                                charCount++;
                                formattedString.Append(inputChar + " ");
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
                boardToTest = formattedString.ToString().TrimEnd();
                return true;
            }
            else if (!eCharFound)
            {
                Console.WriteLine("No 'e' was entered.");
                return false;
            }
            else
            {
                Console.WriteLine("Level configuration is invalid. Must conform to 1 of the following:");
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
            StringBuilder sbNewBoard = new StringBuilder();
            Random rand = new Random();

            int i;
            for (i = 0; i < 50; i++) // 50 in 10 sec
            {
                inputList.Add(generateRandomBoard(rand, ref sbNewBoard, 1));
            }

            for (i = 0; i < 50; i++) // 50 in 12 sec
            {
                inputList.Add(generateRandomBoard(rand, ref sbNewBoard, 2));
            }

            for (i = 0; i < 30; i++) // 30 in 30 sec
            {
                inputList.Add(generateRandomBoard(rand, ref sbNewBoard, 3));
            }

            for (i = 0; i < 10; i++) // 10 in 20 sec
            {
                inputList.Add(generateRandomBoard(rand, ref sbNewBoard, 4));

            }
        }

        private static string generateRandomBoard(Random rand, ref StringBuilder sbNewBoard, int level)
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
    }
}