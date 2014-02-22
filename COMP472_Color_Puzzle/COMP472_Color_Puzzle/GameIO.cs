﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace COMP472_Color_Puzzle
{
    class GameIO
    {
        private static List<string> inputList;

        public GameIO()
        {
            inputList = new List<string>();
            Console.WriteLine("Would you like to enter the input yourself, or read it from a file?");
            string answer = string.Empty;
            bool done = false;
            do
            {
                Console.Write("Please enter \'f\' for file and \'k\' for keyboard: ");
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
                    ReadInputFromFile();
                    break;
                case "k":
                    do
                    {
                    ReadInputFromKeyboard();
                    Console.WriteLine("Would you like to enter another one? (y/n): ");
                    } while (Console.ReadLine() != "n");
                    break;
            }
        }

        private void ReadInputFromFile()
        {
            Console.WriteLine("Reading from default file");
            string[] file = File.ReadAllLines(GetPath(), Encoding.UTF8); // hardcoded for now

            foreach (string startingBoard in file)
            {
                if (!string.IsNullOrEmpty(startingBoard))
                {
                    inputList.Add(startingBoard);
                }
            }
        }

        private void ReadInputFromKeyboard()
        {
            // they said that we do not need to validate input. I'll add it just in case for next build
            Console.WriteLine("\nPlease enter your inital board configuration, all in one line:");
            string inputString = Console.ReadLine();
            inputList.Add(inputString);
        } 

        public string ChooseInitialBoard()
        {
            string returnString = string.Empty;
            int index = 0;
            int choice = -1;
            string answer = string.Empty;
            bool done = false;
            
            if (inputList.Count > 0)
            {
                Console.WriteLine("Which initial configuration would you like to start with?");
                foreach (string sBoardConfig in inputList)
                {
                    Console.WriteLine("[" + (index++) + "] : " + sBoardConfig);
                }
                do
                {
                    Console.Write("Please make your choice: ");
                    answer = Console.ReadLine();
                    choice = int.Parse(answer);
                    if (choice >= 0 && choice < index)
                    {
                        done = true;
                    }
                    else
                    {
                        Console.WriteLine("Oops! Try again");
                    }
                } while (!done);
                returnString = inputList[choice];
            }
            else
            {
                Console.WriteLine("Oops! No initial board configurations are available.");
                ReadInputFromKeyboard();
                returnString = inputList[0];
            }

            return returnString;
        }

        // Hard-coded for now to my computer. Do you know how to make stuff relative?
        public static string GetPath()
        {
            return @"C:\Documents and Settings\dima\My Documents\Visual Studio 2010\Projects\AI\COMP476_AI\COMP472_Color_Puzzle\COMP472_Color_Puzzle\IoFiles\sample-input1.txt";
        }

    }
}
