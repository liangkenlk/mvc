
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System; 
using System.Collections.Generic;
using System.Web;

namespace TY.Core
{
    public class SocketServer
    {
        private static byte[] result = new byte[1024];
        private static int myProt = 8885;   //端口  
        public static Socket serverSocket;
        public static string msg = "";
        public  delegate void MsgHandler();
        public static event MsgHandler OnMsgChange;
        public static System.Collections.Generic.Dictionary<string, Socket> SocketList = new Dictionary<string, Socket>();
        static bool isOnline = true;
        public static void start()
        {
           
            //服务器IP地址  
            IPAddress ip = IPAddress.Any;
          
                try
                {
                    serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);

                    serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口  
                    serverSocket.Listen(10);
                }
                catch (Exception ex) { msg = ex.Message; OnMsgChange(); return; }
            //设定最多10个排队连接请求  
            msg  = string.Format("启动监听{0}成功\n", serverSocket.LocalEndPoint.ToString());
            OnMsgChange();
            //通过Clientsoket发送数据  
            Thread myThread = new Thread(ListenClientConnect);

            myThread.Start();
            //serverSocket.BeginAccept(acceptCallBack, serverSocket);
            //Console.ReadLine();
        }

        private static void acceptCallBack(IAsyncResult ar)
        {
            throw new NotImplementedException();
        }

        public static void Close()
        {
            isOnline = false;
            
            //serverSocket.Shutdown(SocketShutdown.Both);
            serverSocket.Close();
            //serverSocket.Dispose();
            serverSocket = null;

        }
        /// <summary>  
        /// 监听客户端连接  
        /// </summary>  
        private static void ListenClientConnect()
        {


            while (true)
            {
                Socket clientSocket=null; 

                try
                {

                    clientSocket = serverSocket.Accept();
                    clientSocket.Send(Encoding.UTF8.GetBytes("请传token"));
                    //Thread checkt = new Thread(checkOnline);
                    //checkt.Start(clientSocket);

                    msg = "有人连进来\n";
                    OnMsgChange();
                    Thread receiveThread = new Thread(ReceiveMessage);

                    receiveThread.Start(clientSocket);
                }
                catch (Exception ex)
                {
                    //Log.Error("socket", ex.Message);
                    msg = ex.Message;
                    OnMsgChange();
                    if (clientSocket != null)
                    {
                       // clientSocket.Shutdown(SocketShutdown.Both); 
                        clientSocket.Close();
                    }
                    return;
                }
            }
        }

        static void checkOnline(object ob)
        {
            Socket clientSocket = (Socket)ob;
            try
            {
                while (true)
                {
                    clientSocket.Send(Encoding.UTF8.GetBytes("1"));
                    
                    Thread.Sleep(1000 * 5);
                }
            }
            catch
            {
                
                clientSocket.Close();
               
                //登录列表中清楚
                foreach (var item in SocketList)
                {
                    if (item.Value==clientSocket)
                        SocketList.Remove(item.Key);
                }
            }
        }

        /// <summary>  
        /// 接收消息  
        /// </summary>  
        /// <param name="clientSocket"></param>  
        private static void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;

            //经典代码,再也不用写什么心跳包了
            byte[] inValue = new byte[] { 1, 0, 0, 0, 0x20, 0x4e, 0, 0, 0xd0, 0x07, 0, 0 }; //True, 20 秒, 2 秒
            myClientSocket.IOControl(IOControlCode.KeepAliveValues, inValue, null);
            while (true)
            {
                try
                {
     
                    //通过clientSocket接收数据  
                    int receiveNumber = myClientSocket.Receive(result);
                    if (receiveNumber > 0)
                    {
                        msg = string.Format("接收客户端{0}消息{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.UTF8.GetString(result, 0, receiveNumber));
                        if (Encoding.UTF8.GetString(result, 0, receiveNumber).Split(':')[0].ToLower() == "token")
                        {
                            string token =  HttpUtility.UrlDecode(Encoding.UTF8.GetString(result, 0, receiveNumber).Split(':')[1].Replace("\n", ""));
                            if (UserAuth.IsLogined(token))
                            {
                                string userid = UserAuth.GetTokenValue(0, token);
                                //if (SocketList.ContainsKey(userid) && SocketList[userid] != null)
                                    //SocketList[userid].Close();
                                SocketList[userid] = myClientSocket;
                                myClientSocket.Send(Encoding.UTF8.GetBytes("token正确"));
                            }
                        }
                        OnMsgChange();
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                    //myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();

                    return;
                    //登录列表中清楚
      
                }
            }
        }

    
    }
}