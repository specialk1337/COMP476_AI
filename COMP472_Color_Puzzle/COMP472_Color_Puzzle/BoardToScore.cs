using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COMP472_Color_Puzzle
{
    class BoardToScore : IComparable<BoardToScore>
    {
        public string boardConfig { get; set; }
        public int value { get; set; }
        public string moves { get; set; }

        public BoardToScore(string boardConfig, int value, string moves)
        {
            this.boardConfig = boardConfig;
            this.value = value;
            this.moves = moves;
        }

        public int CompareTo(BoardToScore other)
        {
            if (this.value < other.value)
            {
                return -1;
            }
            else if (this.value > other.value)
            {
                return 1;
            }
            else
                if (this.moves.Length > other.moves.Length)
                {
                    return -1;
                }
                else if (this.moves.Length < other.moves.Length)
                    return 1;

                return 0;
        }
    }
}
