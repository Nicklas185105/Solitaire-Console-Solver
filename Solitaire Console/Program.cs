using System;

namespace Solitaire_Console
{
    class Program
    {
        public static bool Restart = true;

        static void Main(string[] args) => new Program().Start(args);

        public static int Wins = 0;
        public static int Lost = 0;

        void Start(string[] args)
        {
            while (Restart)
            {
                if (Wins + Lost >= 500)
                    break;

                System.Threading.Thread.Sleep(1000);

                Restart = false;
                new Solitaire();
            }

            Console.Clear();
            Console.WriteLine("Wins: " + Wins);
            Console.WriteLine("Lost: " + Lost);
            Console.ReadKey();
        }
    }
}
