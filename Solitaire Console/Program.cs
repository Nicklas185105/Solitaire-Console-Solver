namespace Solitaire_Console
{
    class Program
    {
        public static bool Restart = true;

        static void Main(string[] args) => new Program().Start(args);

        void Start(string[] args)
        {
            while (Restart)
            {
                Restart = false;
                new Solitaire();
            }
        }
    }
}
