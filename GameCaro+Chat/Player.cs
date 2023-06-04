using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCaro_Chat
{
    public class Player
    {
        private string name;
        private Image mark;
        private Image avatar;

        public string Name { get => name; set => name = value; }
        public Image Mark { get => mark; set => mark = value; }
        public Image Avatar { get => avatar; set => avatar = value; }

        public Player(string name, Image avatar, Image mark) 
        {
            this.Name = name;
            this.Mark = mark;
            this.Avatar = avatar;
        }
    }
}
