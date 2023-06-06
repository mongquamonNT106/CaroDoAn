using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCaro_Chat
{
    [Serializable]
    public class SocketData
    {
        private int command;
        private Point point;
        private string message;

        public int Command { get => command; set => command = value; }
        public Point Point { get => point; set => point = value; }
        public string Message { get => message; set => message = value; }


        public SocketData(int command, Point point, string message) 
        {
            this.Command = command;
            this.Point = point;
            this.Message = message;
        }

    }
    public enum SendCommand
    {
        SEND_LOCATION,
        SEND_MESSAGE,
        SEND_NEW_GAME,
        SEND_END_GAME,
        SEND_TIME_OUT,
        SEND_UNDO,
        SEND_REDO,
        QUIT
    }
}
