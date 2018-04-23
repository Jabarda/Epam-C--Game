using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ZeroMQ;

namespace Client
{
    static class Client
    {
        /// <summary>
        /// Главная точка входа для приложения.
        public static InputNameWindow inpWindow;
        public static GameplayWindow gameWindow;
        public static String PlayerName;
        public static String Port;
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            inpWindow = new InputNameWindow();
            Application.Run(inpWindow);
            gameWindow = new GameplayWindow();
            Updater();
            Application.Run(gameWindow);
        }

        public static void Auth(string Name)
        {
            GetServerPort();
            string ServerAdress = "tcp://127.0.0.1:5"+ Port +"5";
            using (var context = new ZContext())
            using (var requester = new ZSocket(context, ZSocketType.REQ))
            {
                requester.Connect(ServerAdress);
                requester.Send(new ZFrame(Name));
                using (ZFrame reply = requester.ReceiveFrame())
                {
                    if (reply.ReadString() == "Accepted")
                    {
                        Console.WriteLine("Accept");
                        PlayerName = Name;
                        inpWindow.Close();
                    }
                    else
                    {
                        Console.WriteLine("error");
                        MessageBox.Show("Error (maybe name is already taken)", "error",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
                    }
                }
            }
        }

        public static void GetServerPort()
        {
            string ServerAdress = "tcp://127.0.0.1:5000";
            using (var context = new ZContext())
            using (var requester = new ZSocket(context, ZSocketType.REQ))
            {
                requester.Connect(ServerAdress);
                requester.Send(new ZFrame("New"));
                using (ZFrame reply = requester.ReceiveFrame())
                {
                    String Answer = reply.ReadString();
                    if (Answer != "Error")
                    {
                        Console.WriteLine("Accept");
                        Port = Answer;
                        MessageBox.Show("Connecting to "+Port+"...", "Connecting...",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information);
                    }
                    else
                    {
                        Console.WriteLine("error");
                        MessageBox.Show("Error (Get New Port)", "error",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
                    }
                }
            }
        }


        public static void Move(string Direction)
        {
            string ServerAdress = "tcp://127.0.0.1:5" + Port + "6";
            using (var context = new ZContext())
            using (var requester = new ZSocket(context, ZSocketType.REQ))
            {
                requester.Connect(ServerAdress);
                requester.Send(new ZFrame(PlayerName +";"+Direction));
                using (ZFrame reply = requester.ReceiveFrame())
                {
                    if (reply.ReadString() == "Accepted")
                    {
                        Console.WriteLine("Accepted");
                    }
                    else
                    {
                        Console.WriteLine("error");
                        MessageBox.Show("Error (maybe name is already taken)", "error",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
                    }
                }
            }
        }

        public static void Updater()
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                using (var context = new ZContext())
                using (var subscriber = new ZSocket(context, ZSocketType.SUB))
                {
                    string connect_to = "tcp://127.0.0.1:5" + Port + "7";
                    subscriber.Connect(connect_to);
                    subscriber.SubscribeAll();
                    while (true)
                    {
                        using (var replyFrame = subscriber.ReceiveFrame())
                        {
                            string reply = replyFrame.ReadString();
                            gameWindow.Invoke(new Action(() =>
                            {
                                Label PlayerComp = gameWindow.Controls.Find(reply.Split(';')[0], true).FirstOrDefault() as Label;
                                if (PlayerComp != null) gameWindow.Controls.Remove(PlayerComp);

                            }));
                            gameWindow.Invoke(new Action(()=>gameWindow.AddLabel(reply.Split(';')[0], reply.Split(';')[1], reply.Split(';')[2], reply.Split(';')[3])));
                        }
                    }
                }
            });
        }

    }
}
