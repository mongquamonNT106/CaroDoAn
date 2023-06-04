using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCaro_Chat
{
    public class PlayInfo
    {
        private Point point;
        private int currentPlayer;
        private Image mark;

        public Point Point { get => point; set => point = value; }
        public int CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
        public Image Mark { get => mark; set => mark = value; }
        public PlayInfo(Point point, int currentPlayer, Image mark)
        {
            this.Point = point;
            this.CurrentPlayer = currentPlayer;
            this.Mark = mark;
        }
                
    }
}
