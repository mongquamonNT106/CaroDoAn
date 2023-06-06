using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCaro_Chat
{
    public partial class GameCaro : Form
    {
        #region properties
        BoardManager ChessBoard;
        string winner_timeout;
        string winner;
        SocketManager socket;
        int undolimit;
        int redolimit;
        #endregion
        public GameCaro()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            ChessBoard = new BoardManager(pnlChessBoard,txbName, pbAvatar);
            ChessBoard.GameOver += ChessBoard_GameOver;
            ChessBoard.ButtonClicked += ChessBoard_ButtonClicked;
            pbTimer.Step = Cons.TIMER_STEP;
            pbTimer.Maximum = Cons.TIMER_sum;
            pbTimer.Value = 0;
            Cooldown.Interval = Cons.TIME_INTERVAL;
            
            socket = new SocketManager();
            
            txb_Chat.Text = "";
            NewGame();
        }

        private void ChessBoard_ButtonClicked(object? sender, ButtonClickEvent e)
        {
            Cooldown.Start();
            pbTimer.Value = 0;
            
            undolimit = 1;
            
            if (ChessBoard.GameMode == 1)
            {
                try
                {
                    pnlChessBoard.Enabled = false;
                    socket.Send(new SocketData((int)SendCommand.SEND_LOCATION, e.ClickedPoint,""));

                    undoToolStripMenuItem.Enabled = false;
                    redoToolStripMenuItem.Enabled = false;

                    btnUndo.Enabled = false;
                    btnRedo.Enabled = false;

                    Listen();
                }
                catch
                {
                    GameEnd();
                    MessageBox.Show("Hiện tại không có kết nối !!", "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }


        }

        void GameEnd()
        {
            Cooldown.Stop();
            pnlChessBoard.Enabled = false;
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;
            btnUndo.Enabled = false;
            btnRedo.Enabled = false;
            if (ChessBoard.GameMode != 1)
            {
                MessageBox.Show("Trò chơi kết thúc !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            
        }
        private void ChessBoard_GameOver(object? sender, EventArgs e)
        {
            GameEnd();
            if (ChessBoard.GameMode == 1)
            {   
                winner = ChessBoard.Players[ChessBoard.CurrentPlayer == 1 ? 0 : 1].Name;

                socket.Send(new SocketData((int)SendCommand.SEND_END_GAME, new Point(), ""));
            }
        }

        private void Cooldown_Tick(object sender, EventArgs e)
        {
            pbTimer.ForeColor = Color.Red;
            pbTimer.PerformStep();
            if(pbTimer.Value >=pbTimer.Maximum)
            {   
            
                if (ChessBoard.GameMode == 1)
                {
                    
                    socket.Send(new SocketData((int)SendCommand.SEND_TIME_OUT, new Point(), ""));
                }
                GameEnd();
                MessageBox.Show("Hết giờ !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        void NewGame()
        {
            pnlChessBoard.Controls.Clear();
            
            pbTimer.Value = 0;
            pnlChessBoard.Enabled = true;
            ChessBoard.DrawChessBoard();
            
        }
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
            if (ChessBoard.GameMode == 1)
            {
                try
                {
                    socket.Send(new SocketData((int)SendCommand.SEND_NEW_GAME, new Point(),""));
                }
                catch { }
            }

            pnlChessBoard.Enabled = true;
        }
        void Exit()
        {
            if (MessageBox.Show("Bạn có muốn thoát chứ ?", "Thông Báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK) ;
            
        }
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát không", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                e.Cancel = true;
            else
            {
                try
                {
                    socket.Send(new SocketData((int)SendCommand.QUIT, new Point(),""));
                }
                catch { }
            }
        }
        public int undolimits()
        {
            return undolimit;
        }
        
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undolimit == 1)
            {
                ChessBoard.Undo();
                pbTimer.Value = 0;
                if (ChessBoard.GameMode == 1)
                    socket.Send(new SocketData((int)SendCommand.SEND_UNDO, new Point(), ""));
                undolimit = 0;
                redolimit = 1;
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (redolimit == 1)
            {
                ChessBoard.Redo();
                pbTimer.Value = 0;
                if (ChessBoard.GameMode == 1)
                    socket.Send(new SocketData((int)SendCommand.SEND_REDO, new Point(), ""));
                redolimit = 0;
            }
        }
        #region LAN
        private void Form1_Shown(object sender, EventArgs e)
        {
            txbIP.Text = socket.GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            if(string.IsNullOrEmpty(txbIP.Text))
            {
                txbIP.Text = socket.GetLocalIPv4(NetworkInterfaceType.Ethernet);
            }
        }
        private void Listen()
        {
            Thread ListenThread = new Thread(() =>
            {
                try
                {
                    SocketData data = (SocketData)socket.Receive();
                    DataCase(data);
                }
                catch { }
            });

            ListenThread.IsBackground = true;
            ListenThread.Start();
        }

        private void DataCase(SocketData e)
        {
            winner_timeout = ChessBoard.Players[ChessBoard.CurrentPlayer == 1 ? 0 : 1].Name;
            
            switch (e.Command)
            {   
                case (int)SendCommand.SEND_LOCATION:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        ChessBoard.OtherPlayerLocation(e.Point);
                        pnlChessBoard.Enabled = true;

                        pbTimer.Value = 0;
                        Cooldown.Start();

                        undoToolStripMenuItem.Enabled = true;
                        redoToolStripMenuItem.Enabled = true;

                        btnUndo.Enabled = true;
                        btnRedo.Enabled = true;
                    }));
                    break;
                
                case (int)SendCommand.SEND_NEW_GAME:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        NewGame();
                        pnlChessBoard.Enabled = false;
                    }));
                    break;
                case (int)SendCommand.SEND_UNDO:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        pbTimer.Value = 0;
                        ChessBoard.Undo();
                        MessageBox.Show( "Đối thủ vừa sử dụng chức năng Undo", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                    break;
                case (int)SendCommand.SEND_REDO:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        pbTimer.Value = 0;
                        ChessBoard.Redo();
                        MessageBox.Show("Đối thủ vừa sử dụng chức năng Redo", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                    break;
                case (int)SendCommand.SEND_END_GAME:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        GameEnd();
                        MessageBox.Show(winner + " đã chiến thắng  !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                    break;
               
                case (int)SendCommand.QUIT:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        Cooldown.Stop();
                        GameEnd();

                        ChessBoard.GameMode = 2;
                        socket.CloseConnect();

                        MessageBox.Show("Đối thủ đã rời khỏi phòng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                    break;
                case (int)SendCommand.SEND_TIME_OUT:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        Cooldown.Stop();
                        GameEnd();
                        MessageBox.Show("Thời gian kết thúc, " + winner_timeout + " thắng !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                    break;
                case (int)SendCommand.SEND_MESSAGE:
                    txb_Chat.Text += "Player: " + e.Message + "\n";
                    break;
                default:
                    break;




            }
            Listen();
        }
        #endregion

        private void LAN_Click(object sender, EventArgs e)
        {   
            lANToolStripMenuItem_Click(sender, e);
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            undoToolStripMenuItem_Click(sender, e);
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            redoToolStripMenuItem_Click(sender, e);
        }

        private void lANToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ChessBoard.GameMode = 1;
            NewGame();
            socket.IP = txbIP.Text;
            if (!socket.ConnectServer())
            {
                socket.IsServer = true;
                pnlChessBoard.Enabled = true;
                socket.HostServer();
                MessageBox.Show("Tạo phòng thành công !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                socket.IsServer = false;
                pnlChessBoard.Enabled = false;
               
                Listen();
                MessageBox.Show("Đã kết nối vào phòng chơi !!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }
        }

        private void singleplayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void pCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ChessBoard.GameMode == 1)
            {
                try
                {
                    socket.Send(new SocketData((int)SendCommand.QUIT, new Point(),""));
                }
                catch { }

                socket.CloseConnect();
                MessageBox.Show("Đã ngắt kết nối mạng LAN", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            ChessBoard.GameMode = 2;
            NewGame();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            newGameToolStripMenuItem_Click(sender, e);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            if(ChessBoard.GameMode != 1) 
                return;
            
            txb_Chat.Text += "You: " + txb_Type.Text + "\n";
            socket.Send(new SocketData((int)SendCommand.SEND_MESSAGE,  new Point(), txb_Type.Text));
            txb_Type.Clear();
            Listen();
        }
    }
}