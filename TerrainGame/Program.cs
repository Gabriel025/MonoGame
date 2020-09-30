using System;

namespace TerrainGame
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (Main game = new Main())
                game.Run();
        }
    }
#endif
}
