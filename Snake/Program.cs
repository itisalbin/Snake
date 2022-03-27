var game = new Game();

game.Start();

while (!game.Quit)
{
    // listen to key presses
    if (Console.KeyAvailable)
    {
        var input = Console.ReadKey(true);

        switch (input.Key)
        {
            // send key presses to the game if it's not paused
            case ConsoleKey.UpArrow:
            case ConsoleKey.DownArrow:
            case ConsoleKey.LeftArrow:
            case ConsoleKey.RightArrow:
                if (!game.Paused)
                    game.Input(input.Key);
                break;

            case ConsoleKey.P:
                if (game.Paused)
                    game.Resume();
                else
                    game.Pause();
                break;
            case ConsoleKey.Spacebar:
                if (game.GameOver) {
                    game.Start();
                }
                break;
            case ConsoleKey.Escape:
                game.Stop();
                return;
        }
    }
}