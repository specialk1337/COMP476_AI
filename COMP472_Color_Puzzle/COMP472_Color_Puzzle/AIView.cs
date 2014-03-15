using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COMP472_Color_Puzzle
{
    class AIView
    {
        private GameCommand _command;
        private GameState _state;

        public AIView(GameCommand command)
        {
            _command = command;
            _state = _command.getState();
        }
        public void play()
        {
            String[] testString = new String[] {
            "rbrbre",
            "rbwrebrbr",
            "rbebrr",
            };

            for (int i = 0; i != testString.Count(); ++i)
            {
                float boardScore = score(testString[i]);
            }

        }

        private float score(String Board_Info)
        {
            float MaxScore = 10000;
            float currentScore = 0;

            string data = Board_Info.Replace(" ", string.Empty);
            int boardsize = data.Length;

            if (boardsize % 3 != 0)
            {
                Console.WriteLine("ERROR DETECTED Score requested on bad format board");
                return 0;
            }

            for (int i = 0; i != boardsize/3; ++i)
            {
                if (boardsize == 3)
                {
                    currentScore += (data[0] == data[2]) ? 2000 : 0;
                    currentScore += (data[0] == data[1] && data[2] == 'e') ? 1500 : 0;
                    currentScore += (data[1] == data[2] && data[0] == 'e') ? 1500 : 0;
                    return currentScore;
                    
                }

                /* Score the column*/
                if (data[i] == data[i + 2 * (boardsize / 3)])
                {
                    currentScore += 2000;
                }

                if (data[i] == data[i + 1 * (boardsize / 3)])
                {
                    currentScore += 5;
                    if (data[i + 2 * (boardsize / 3)] == 'e')
                    {
                        currentScore += 1500;
                    }
                }
                if (data[i + 2 * (boardsize / 3)] == data[i + 1 * (boardsize / 3)])
                {
                    currentScore+=5;
                    if(data[i] == 'e')
                    {
                        currentScore +=1500;
                    }
                }

                /*End Column*/

                if (i == 0)
                {
                    //On the left side
                    if (data[i] == data[i + 2 * (boardsize / 3) +1] && data[i + 2 * (boardsize / 3)] =='e' )
                    {
                        currentScore+=1500;
                    }
                    else if ( data[i +1] == data[i + 2 * (boardsize / 3)] && data[i] =='e')
                    {
                        currentScore+=1500;
                    }
                }

                else if (i == boardsize / 3)
                {
                    //on the right side
                    if (data[i] == data[i + 2 * (boardsize / 3) -1] && data[i + 2 * (boardsize / 3)] =='e' )
                    {
                        currentScore+=1500;
                    }
                    if ( data[i -1] == data[i + 2 * (boardsize / 3)] && data[i] =='e')
                    {
                        currentScore+=1500;
                    }
                }

                else
                {
                    if (data[i] == data[i + 2 * (boardsize / 3) -1] && data[i + 2 * (boardsize / 3)] =='e' )
                    {
                        currentScore+=1500;
                    }
                    if ( data[i +1] == data[i + 2 * (boardsize / 3)] && data[i] =='e')
                    {
                        currentScore+=1500;
                    }

                                        //on the right side
                    if (data[i] == data[i + 2 * (boardsize / 3) -1] && data[i + 2 * (boardsize / 3)] =='e' )
                    {
                        currentScore+=1500;
                    }
                    else if ( data[i -1] == data[i + 2 * (boardsize / 3)] && data[i] =='e')
                    {
                        currentScore+=1500;
                    }
                }

            }

            return currentScore;
        }

    }
}
