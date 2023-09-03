// See https://aka.ms/new-console-template for more information

using GameServer;
using System.Threading;

namespace GameServer
{
    class Program
    {
        private static bool isRunning = false;

        static void Main(string[] args)
        {
            Console.Title = "Server";
            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start(8, 5500);
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread started. running at { Constants.TICKS_PER_SEC } ticks/sec");
            DateTime nextLoop = DateTime.Now;

            while (isRunning)
            {
                while(nextLoop < DateTime.Now)
                {
                    GameLogic.Update();

                    nextLoop = nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if(nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(nextLoop - DateTime.Now);
                    }
                }
            }
        }

        public static void setRunning(bool status)
        {
            isRunning = status;
        }
    }
}

