using System;

namespace Roguelike
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new RoguelikeGame())
                game.Run();
        }
    }
}
