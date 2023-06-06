using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameCaro_Chat
{
    public class BoardManager
    {
        #region properties
        private Panel ChessBoard;
        private List<Player> players;
        private int currentPlayer;
        private TextBox playerName;
        private PictureBox avatar;
        private List<List<Button>> matrix;
        private Stack<PlayInfo> UndoStack;
        private Stack<PlayInfo> RedoStack;
        private int gameMode = 0;
        private bool vsAI = false;
        public int GameMode
        {
            get { return gameMode; }
            set { gameMode = value; }
        }
        public Panel ChessBoard1 { get => ChessBoard; set => ChessBoard = value; }
        public List<Player> Players { get => players; set => players = value; }
        public int CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
        public TextBox PlayerName { get => playerName; set => playerName = value; }
        public PictureBox Avatar { get => avatar; set => avatar = value; }
        public List<List<Button>> Matrix { get => matrix; set => matrix = value; }
        public Stack<PlayInfo> UndoStack1 { get => UndoStack; set => UndoStack = value; }
        public Stack<PlayInfo> RedoStack1 { get => RedoStack; set => RedoStack = value; }
        
        private event EventHandler<ButtonClickEvent> buttonCicked;
        public event EventHandler<ButtonClickEvent> ButtonClicked
        {
            add
            {
                buttonCicked += value;
            }
            remove
            {
                buttonCicked -= value;
            }
        }

        
        private event EventHandler gameOver;
        public event EventHandler GameOver
        {
            add
            {
                gameOver += value;
            }
            remove
            {
                gameOver -= value;
            }
        }

        #endregion
        #region Initialize
        public BoardManager(Panel ChessBoard, TextBox PlayerName, PictureBox Avatar)
        {
            this.ChessBoard = ChessBoard;
            this.Avatar =   Avatar;
            this.playerName = PlayerName;   
            this.Players = new List<Player>()
            {
                new Player("Player 1",Image.FromFile(Application.StartupPath +"\\Resources\\Player1.png"),
                                      Image.FromFile(Application.StartupPath + "\\Resources\\X.png")),
                new Player("Player 2",Image.FromFile(Application.StartupPath +"\\Resources\\Player2.png"),
                                      Image.FromFile(Application.StartupPath + "\\Resources\\O.png"))
            };
            CurrentPlayer = 0;
            ChangePlayer();
        }
        
        #endregion
        #region Methods
        public void DrawChessBoard()
        {   
            ChessBoard.Controls.Clear(); 


            Matrix = new List<List<Button>>(); 
            UndoStack = new Stack<PlayInfo>();
            RedoStack = new Stack<PlayInfo>(); 
            this.CurrentPlayer = 0;
            ChangePlayer();
            Button oldbtn = new Button()
            {
                Width = 0,
                Location = new Point(0, 0)
            };
            for (int i = 0; i < Cons.CHESS_BOARD_WIDTH; i++)
            {
                for (int j = 0; j < Cons.CHESS_BOARD_HEIGHT; j++)
                {
                    Matrix.Add(new List<Button>());
                    Button btn = new Button()
                    {
                        Width = Cons.CHESS_WIDTH,
                        Height = Cons.CHESS_HEIGHT,
                        Location = new Point(oldbtn.Location.X + oldbtn.Width, oldbtn.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString()
                    };
                    btn.Click += Btn_Click;
                    ChessBoard.Controls.Add(btn);
                    Matrix[i].Add(btn);
                    oldbtn = btn;
                }
                oldbtn.Location = new Point(0, oldbtn.Location.Y + Cons.CHESS_HEIGHT);
                oldbtn.Width = 0;
                oldbtn.Height = 0;
            }
        }

        private void Btn_Click(object? sender, EventArgs e)
        {
           Button btn = sender as Button;
            if (btn.BackgroundImage != null)
                return;
            btn.BackgroundImage = Players[CurrentPlayer].Mark;
            
            UndoStack.Push(new PlayInfo(GetLocation(btn), CurrentPlayer, btn.BackgroundImage));
            
            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
            ChangePlayer();
            if (buttonCicked != null)
                buttonCicked(this, new ButtonClickEvent(GetLocation(btn)));
            if (isEndGame(btn))
            {
                ChessBoard.Enabled = false;
                EndGame();  
            }
        }

        #region undo+redo
        public bool Undo()
        {
            if (UndoStack.Count <= 1)
                return false;
            PlayInfo OldBtn = UndoStack.Peek();
            CurrentPlayer = OldBtn.CurrentPlayer == 1 ? 0 : 1;
            bool undoValid1 = UndoBack();
            bool undoValid2 = UndoBack();

            return undoValid1 && undoValid2;

        }
        private bool UndoBack()
        {
            if(UndoStack.Count< 1)
                return false;
            PlayInfo OldBtn = UndoStack.Pop();
            RedoStack.Push(OldBtn);

            Button btn = Matrix[OldBtn.Point.Y][OldBtn.Point.X];
            btn.BackgroundImage = null;

            if (UndoStack.Count < 1)
                currentPlayer = 0;
            else
                OldBtn = UndoStack.Peek();
            ChangePlayer();
            return true;
        }
        public bool Redo()
        {
            if(RedoStack.Count<1)
                return false;
            PlayInfo OldBtn = RedoStack.Peek();
            CurrentPlayer = OldBtn.CurrentPlayer;
            bool redoValid1 = RedoBack();
            bool redoValid2 = RedoBack();

            return redoValid1 && redoValid2;

        }
        private bool RedoBack()
        {
            if(RedoStack.Count < 1)
                return false;

            PlayInfo OldBtn = RedoStack.Pop();
            UndoStack.Push(OldBtn);

            Button btn = Matrix[OldBtn.Point.Y][OldBtn.Point.X];
            btn.BackgroundImage = OldBtn.Mark;

            if (RedoStack.Count < 1)
            {
                CurrentPlayer = OldBtn.CurrentPlayer == 1 ? 0 : 1;
            }
            else
            {
                OldBtn = RedoStack.Peek();
            }

            ChangePlayer();

            return true;    
        }

        #endregion
        public void OtherPlayerLocation(Point point)
        {
            Button btn = Matrix[point.Y][point.X];

            if (btn.BackgroundImage != null)
                return;
            ChessBoard.Enabled = true;
            btn.BackgroundImage = Players[CurrentPlayer].Mark;
            UndoStack.Push(new PlayInfo(GetLocation(btn), CurrentPlayer,btn.BackgroundImage));
            RedoStack.Clear();
            
            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
            ChangePlayer();

            if(isEndGame(btn))
                EndGame();
        }
        private void EndGame()
        {
            if (gameOver !=null)
                gameOver(this,new EventArgs());
        }

        private bool isEndGame(Button btn)
        {
            return checkHorizontal(btn) || checkVertical(btn) || checkPrimary(btn) || checkSub(btn);
        }
        private bool checkSub(object btn)
        {
            Button button = btn as Button;
            int x = Convert.ToInt32(button.Tag);
            int y = matrix[x].IndexOf(button);
            int countUp = 0;
            for (int i = 0; i <= Cons.CHESS_BOARD_WIDTH - y; i++)
            {
                if (x - i < 0 || y + i >= Cons.CHESS_BOARD_WIDTH)
                    break;
                if (matrix[x - i][y + i].BackgroundImage == button.BackgroundImage)
                {
                    countUp++;
                }
                else
                    break;
            }
            int countDown = 0;
            for (int i = 1; i <= y; i++)
            {
                if (x + i >= Cons.CHESS_BOARD_HEIGHT || y - i < 0)
                    break;
                if (matrix[x + i][y - i].BackgroundImage == button.BackgroundImage)
                {
                    countDown++;
                }
                else
                    break;
            }
            return countUp + countDown >= 5 ? true : false;
        }
        private bool checkPrimary(object btn)
        {
            Button button = btn as Button;
            int x = Convert.ToInt32(button.Tag);
            int y = matrix[x].IndexOf(button);
            int countUp = 0;
            for (int i = 0; i <= y; i++)
            {
                if (x - i < 0 || y - i < 0)
                    break;
                if (matrix[x - i][y - i].BackgroundImage == button.BackgroundImage)
                {
                    countUp++;
                }
                else
                    break;
            }
            int countDown = 0;
            for (int i = 1; i <= Cons.CHESS_BOARD_WIDTH - y; i++)
            {
                if (x + i >= Cons.CHESS_BOARD_HEIGHT || y + i >= Cons.CHESS_BOARD_WIDTH)
                    break;
                if (matrix[x + i][y + i].BackgroundImage == button.BackgroundImage)
                {
                    countDown++;
                }
                else
                    break;
            }
            return countUp + countDown >= 5 ? true : false;
        }
        private bool checkVertical(object btn)
        {
            Button button = btn as Button;
            int x = Convert.ToInt32(button.Tag);
            int y = matrix[x].IndexOf(button);
            int countLeft = 0;
            
            for (int i = x; i >= 0; i--)
            {

                if (matrix[i][y].BackgroundImage == button.BackgroundImage)
                {
                    countLeft++;
                }
                else
                    break;
            }
            int countRight = 0;
            for (int i = x + 1; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                if (matrix[i][y].BackgroundImage == button.BackgroundImage)
                {
                    countRight++;
                }
                else
                    break;
            }
            return countLeft + countRight >= 5 ? true : false;
        }
        private bool checkHorizontal(object btn)
        {
            Button button = btn as Button;
            int x = Convert.ToInt32(button.Tag);
            int y = matrix[x].IndexOf(button);
            int countLeft = 0;
            for (int i = y; i >= 0; i--)
            {
                if (matrix[x][i].BackgroundImage == button.BackgroundImage)
                {
                    countLeft++;
                }
                else
                    break;
            }
            int countRight = 0;
            for (int i = y + 1; i < Cons.CHESS_BOARD_WIDTH; i++)
            {
                if (matrix[x][i].BackgroundImage == button.BackgroundImage)
                {
                    countRight++;
                }
                else
                    break;
            }
            return countLeft + countRight >= 5 ? true : false;
        }
        private Point GetLocation(Button btn)
        {
            int vertical = Convert.ToInt32(btn.Tag);
            int horizontal = Matrix[vertical].IndexOf(btn);

            Point location = new Point(horizontal, vertical);
            return location;
            
        }
        private void ChangePlayer()
        {
            PlayerName.Text = Players[CurrentPlayer].Name;
            Avatar.Image = Players[CurrentPlayer].Avatar;
        }

        #endregion
    }
}
