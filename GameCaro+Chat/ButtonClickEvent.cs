using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCaro_Chat
{
    public class ButtonClickEvent
    {
        private Point clickedPoint;

        public Point ClickedPoint { get => clickedPoint; set => clickedPoint = value; }
        
        public ButtonClickEvent(Point clickedPoint) {
            this.ClickedPoint = clickedPoint;
        }

    }
}
