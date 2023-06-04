using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GameCaro_Chat
{
    public class SocketManager
    {
        #region Server
        Socket server;
        public void HostServer()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IP), PORT);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            server.Bind(iep);
            server.Listen(15);

            Thread AcceptClient = new Thread(() => 
            { 
                try 
                { 
                    client = server.Accept(); 
                } 
                catch 
                { } });
            AcceptClient.IsBackground = true; 
            AcceptClient.Start();
        }
        #endregion
        #region Client
        Socket client;
        public bool ConnectServer()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IP), PORT);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.Connect(iep);
                return true;
            }
            catch
            {
                return false;
            }  
        }
        #endregion
        #region Multi
        public string IP = "127.0.0.1";
        public int PORT = 9999;
        public bool IsServer = true;
        public const int Length = 1024;

        private bool SendData(Socket target, byte[] data)
        {
            return target.Send(data) == 1;
        }

        private bool ReceiveData(Socket target, byte[] data)
        {
            return target.Receive(data) == 1;
        }

        public bool Send(object data)
        {
            byte[] sendedData = SerializeData(data);
            return SendData(client, sendedData);
        }

        public object Receive()
        {
            byte[] receivedData = new byte[Length]; 
            bool IsOk = ReceiveData(client, receivedData);
            return DeserializeData(receivedData);
        }

        
        
        public byte[] SerializeData(Object obj)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf1 = new BinaryFormatter();
            bf1.Serialize(ms, obj);
            return ms.ToArray();
        }

        
       
        public object DeserializeData(byte[] theByteArray)
        {
            MemoryStream ms = new MemoryStream(theByteArray);
            BinaryFormatter bf1 = new BinaryFormatter();
            ms.Position = 0;
            return bf1.Deserialize(ms);
        }

        

        public string GetLocalIPv4(NetworkInterfaceType _type)
        {
            string output = "";

            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            output = ip.Address.ToString();
            return output;
        }

        public void CloseConnect()
        {
            try
            {
                server.Close();
                client.Close();
            }
            catch { }

        }
        #endregion

    }
}
