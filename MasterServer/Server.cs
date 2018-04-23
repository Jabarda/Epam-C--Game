using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;

namespace WindowsFormsApplication1
{
    static class Server
    {
        /// <summary>
        /// Главная точка входа для приложения.
        public static bool Stop = false;
        public static int portstart = 10;
        public static int portcount = 10;
        public static int[] serverPop = new int[100]; 
        /// </summary>
        [STAThread]

        static void Main()
        {
            
            RegisterNewServer();//Раздает порты новым серверам
            TransferNewUser();//Переводит пользователей по серверам
            while (!Stop) ;
        }

        static void RegisterNewServer()
        {
            Console.WriteLine("Auth");
            ThreadPool.QueueUserWorkItem(state =>
            {
                using (var context = new ZContext())
                using (var responder = new ZSocket(context, ZSocketType.REP))
                {
                    responder.Bind("tcp://*:6000");
                    while (!Stop)
                    {
                        Console.WriteLine("Waiting");
                        using (ZFrame request = responder.ReceiveFrame())
                        {

                            Console.WriteLine("New Server!");
                            string buf = request.ReadString();
                            if (buf == "New" && portcount < 100)
                            {
                                responder.Send(new ZFrame(portcount.ToString()));
                                serverPop[portcount] = 0;
                                portcount++;
                            }

                        }
                    }
                }
            });
        }
        static void TransferNewUser()
        {
            Console.WriteLine("TransferNewUser");
            ThreadPool.QueueUserWorkItem(state =>
            {
                using (var context = new ZContext())
                using (var responder = new ZSocket(context, ZSocketType.REP))
                {
                    responder.Bind("tcp://*:5000");
                    while (!Stop)
                    {
                        Console.WriteLine("Waiting");
                        using (ZFrame request = responder.ReceiveFrame())
                        {
                            string buf = request.ReadString();
                            if (buf == "New" && portcount >10)
                            {
                                Console.WriteLine("New User!");
                                int min = portstart;
                                for (int i=portstart; i<portcount; i++)
                                {
                                    if (serverPop[i] < serverPop[min]) min = i;
                                }
                                Console.WriteLine(min.ToString());
                                responder.Send(new ZFrame(min.ToString()));
                                serverPop[min]++;

                            }
                            else
                                responder.Send(new ZFrame("error"));

                        }
                    }
                }
            });
        }



    }
}
