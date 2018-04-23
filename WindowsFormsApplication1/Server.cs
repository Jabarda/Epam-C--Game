using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeroMQ;

namespace WindowsFormsApplication1
{
    static class Server
    {
        /// <summary>
        /// Главная точка входа для приложения.
        public static bool Stop=false;
        public static List<String> Player;
        public static ServerWindow instance;
        public static String port;
        /// </summary>
        [STAThread]
        
        static void Main()
        {
            Player = new List<string>();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Console.WriteLine("1");
            instance = new ServerWindow();
            Console.WriteLine("2");
            GetPort();
            Auth();//Поток авторизации
            Updater();//Обновляет координаты для всех клиентов
            MovementReciver();//Получает передвижения игроков
            Application.Run(instance);
            
        }

        static void GetPort()
        {
            string ServerAdress = "tcp://127.0.0.1:6000";
            using (var context = new ZContext())
            using (var requester = new ZSocket(context, ZSocketType.REQ))
            {
                requester.Connect(ServerAdress);
                requester.Send(new ZFrame("New"));
                using (ZFrame reply = requester.ReceiveFrame())
                {
                    String answer = reply.ReadString();
                    if (answer != "Error")
                    {
                        Console.WriteLine("Accept, new port = "+ answer);
                        port = answer;
                        MessageBox.Show("Port for this server - "+ port, "New port!",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information);
                        
                    }
                    else
                    {
                        Console.WriteLine("error");
                        MessageBox.Show("Error (auth)", "error",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
                    }
                }
            }
        }

        static void Auth()
        {
            Console.WriteLine("Auth");
            ThreadPool.QueueUserWorkItem(state =>
            {
                using (var context = new ZContext())
                using (var responder = new ZSocket(context, ZSocketType.REP))
                {
                    responder.Bind("tcp://*:5"+port+"5");
                    while (!Stop)
                    {
                        Console.WriteLine("Waiting");
                        using (ZFrame request = responder.ReceiveFrame())
                        {
                            string buf = request.ReadString();
                            
                            if (CreateNewPlayer(buf))
                                responder.Send(new ZFrame("Accepted"));
                                
                            else
                                responder.Send(new ZFrame("Error"));
                            
                        }
                    }
                }
            });
        }

        static bool CreateNewPlayer(String Name)
        {
            Console.WriteLine("Create "+Name);
            if (!Player.Contains(Name))
            {
                instance.Invoke(new Action(() => instance.AddLabel(Name)));
                Player.Add(Name);
                return true;
            }
            return false;
        }

        static void MovementReciver()//using 5556 port
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                using (var context = new ZContext())
                using (var responder = new ZSocket(context, ZSocketType.REP))
                {
                    responder.Bind("tcp://*:5" + port + "6");
                    while (!Stop)
                    {
                        using (ZFrame request = responder.ReceiveFrame())
                        {
                            string buf = request.ReadString();
                            Label PlayerComp = instance.Controls.Find(buf.Split(';')[0], true).FirstOrDefault() as Label;
                            switch (buf.Split(';')[1])
                            {
                                case "Left":
                                    instance.Invoke(new Action(()=>PlayerComp.Left--));
                                    break;
                                case "Right":
                                    instance.Invoke(new Action(() => PlayerComp.Left++));
                                    break;
                                case "Up":
                                    instance.Invoke(new Action(() => PlayerComp.Top--));
                                    break;
                                case "Down":
                                    instance.Invoke(new Action(() => PlayerComp.Top++));
                                    break;
                            }
                            responder.Send(new ZFrame("Accepted"));

                        }
                    }
                }
            });
        }

        static void Updater()//using 5557 port, updating the state of map
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                using (var context = new ZContext())
                using (var publisher = new ZSocket(context, ZSocketType.PUB))
                {
                    string address = "tcp://*:5" + port + "7";
                    publisher.Bind(address);
                    while (true)
                    {
                        List<String> ListBuf = Player;
                        foreach (String Pl in ListBuf.ToArray())
                        {
                            //Console.WriteLine("Sending " + Pl);
                            Label PlayerComp = instance.Controls.Find(Pl, true).FirstOrDefault() as Label;
                            publisher.Send(new ZFrame(PlayerComp.Name +";"+ PlayerComp.BackColor.ToArgb() + ";"+ PlayerComp.Location.X + ";" + PlayerComp.Location.Y+";"));
                        }
                        Thread.Sleep(100);
                    }
                }
            });
        }
    }
}
