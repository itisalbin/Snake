class Game
{
    ScheduleTimer? _timer;

    public bool Paused { get; private set; }
    public bool GameOver { get; private set; }
    public bool Quit { get; private set; }

    Snake? _snake;
    List<Food>? _food;
    int _score;
    Dir _lastInputDir;

    int _tickRate = 100;

    int _xSize = 5;
    int _ySize = 3;

    int _superModeCounter;
    int _superModeTickLength = 100;
    bool _superFoodExistOnField;

    public void Start()
    {
        Console.Clear();
        _snake = new Snake(6);
        _food = new List<Food>();
        _score = 0;
        _lastInputDir = Dir.Right;
        _superModeCounter = 0;
        GameOver = false;
        _superFoodExistOnField = false;
        DrawFullSnake(_snake);
        AddFoodToFoodList(_snake.GetBody().ToList(), false);
        ScheduleNextTick();
    }

    public void Pause()
    {
        Console.WriteLine("Pause");
        Paused = true;
        _timer!.Pause();
    }

    public void Resume()
    {
        Console.WriteLine("Resume");
        Paused = false;
        _timer!.Resume();
    }

    public void Stop()
    {
        Console.WriteLine("Stop");
        Quit = true;
    }

    public void Input(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.RightArrow:
                _lastInputDir = Dir.Right;
                break;
            case ConsoleKey.LeftArrow:
                _lastInputDir = Dir.Left;
                break;
            case ConsoleKey.UpArrow:
                _lastInputDir = Dir.Up;
                break;
            case ConsoleKey.DownArrow:
                _lastInputDir = Dir.Down;
                break;
        }
    }

    void Tick()
    {
        Vector2Int oldTailTip = _snake.Move(_lastInputDir);

        if (!_snake.SuperMode)
        {
            if (_snake.CheckIfIntersecting())
            {
                SetGameOver();
                return;
            }

            if (CheckIfOutsideBounds(_snake.Head, out _))
            {
                SetGameOver();
                return;
            }
        }
        else
        {
            if (CheckIfOutsideBounds(_snake.Head, out var loopHeadPos))
            {
                _snake.TeleportHead(loopHeadPos);
            }

            _superModeCounter++;
            if (_superModeCounter > _superModeTickLength)
            {
                _snake.SuperMode = false;
                _superModeCounter = 0;
            }
        }

        if (CheckIfEating(_snake.Head, out var isSuperFood))
        {
            _score++;
            _snake.Grow();

            if (!isSuperFood) {
                AddFoodToFoodList(_snake.GetBody().ToList(), false);
                if (!_snake.SuperMode && !_superFoodExistOnField)
                {
                    Random random = new();
                    if (random.Next(0, 100) < 10)
                    {
                        AddFoodToFoodList(_snake.GetBody().ToList(), true);
                        _superFoodExistOnField = true;
                    }
                }
            }
            else
            {
                _superFoodExistOnField = false;
                _snake.SuperMode = true;
            }
        }

        DrawSnake(_snake, oldTailTip);
        DrawFood();
        Console.SetCursorPosition(0, 0);
        ScheduleNextTick();
    }

    bool CheckIfOutsideBounds(Vector2Int head, out Vector2Int loopHeadPos)
    {
        if (head.X < 0)
        {
            loopHeadPos = new Vector2Int((Console.WindowWidth - 1) / _xSize, head.Y);
            return true;
        }
        else if (head.X > (Console.WindowWidth - 1) / _xSize)
        {
            loopHeadPos = new Vector2Int(0, head.Y);
            return true;
        }
        else if (head.Y < 0)
        {
            loopHeadPos = new Vector2Int(head.X, (Console.WindowHeight - 1) / _ySize);
            return true;
        }
        else if (head.Y > (Console.WindowHeight - 1) / _ySize)
        {
            loopHeadPos = new Vector2Int(head.X, 0);
            return true;
        }
        loopHeadPos = new Vector2Int(-1, -1);
        return false;
    }

    void SetGameOver()
    {
        GameOver = true;
        Console.ForegroundColor = ConsoleColor.White;
        var gameOverMessage = "Game Over";
        var scoreMessage = "Score: " + _score.ToString();
        var replayMessage = "Press Spacebar to play again";
        Console.SetCursorPosition(Console.WindowWidth / 2 - gameOverMessage.Length / 2, Console.WindowHeight / 2 - 1);
        Console.WriteLine(gameOverMessage);
        Console.SetCursorPosition(Console.WindowWidth / 2 - scoreMessage.Length / 2, Console.WindowHeight / 2);
        Console.WriteLine(scoreMessage);
        Console.SetCursorPosition(Console.WindowWidth / 2 - replayMessage.Length / 2, Console.WindowHeight / 2 + 1);
        Console.WriteLine(replayMessage);
        Console.SetCursorPosition(0, 0);
    }

    bool CheckIfEating(Vector2Int head, out bool isSuperFood)
    {
        for (int i = _food.Count - 1; i >= 0; i--)
        {
            if (head == _food[i].Position)
            {
                isSuperFood = _food[i].IsSuperFood;
                _food.RemoveAt(i);
                return true;
            }
        }
        isSuperFood = false;
        return false;
    }

    void DrawFood()
    {
        foreach (var food in _food)
        {
            if (!food.IsSuperFood)
                Console.ForegroundColor = ConsoleColor.Red;
            
            for (int x = 0; x < _xSize; x++)
            {
                for (int y = 0; y < _ySize; y++)
                {
                    if (x != 0 && x != _xSize - 1 && y == 1){continue;}
                    if (x == 0 && y == 0) {continue;}
                    if (x == 0 && y == _ySize - 1) {continue;}
                    if (x == _xSize - 1 && y == 0) {continue;}
                    if (x == _xSize - 1 && y == _ySize - 1) {continue; }

                    if (food.IsSuperFood)
                        Console.ForegroundColor = GetRandomColor(false);

                    Console.SetCursorPosition(0, 12);
                    Console.SetCursorPosition(food.Position.X * _xSize + x, food.Position.Y * _ySize + y);
                    Console.Write(food.Symbol);
                }
            }
        }
    }

    void AddFoodToFoodList(List<Vector2Int> invalidPositions, bool isSuperFood)
    {
        var validPos = false;
        Random random = new Random();
        var x = 0;
        var y = 0;
        WhileStart:
        while (!validPos)
        {
            x = random.Next(0, (Console.WindowWidth - 1) / _xSize);
            y = random.Next(0, (Console.WindowHeight - 1) / _ySize);
            foreach (var invalidPosition in invalidPositions)
            {
                if (x == invalidPosition.X && y == invalidPosition.Y)
                {
                    goto WhileStart;//My apologies
                }
            }
            validPos = true;
        }
        _food.Add(new Food(x, y, isSuperFood));
    }

    void DrawFullSnake(Snake snake)
    {
        Console.ForegroundColor = snake.SuperMode ? GetRandomColor(false) : ConsoleColor.White;

        for (int x = 0; x < _xSize; x++)
        {
            for (int y = 0; y < _ySize; y++)
            {

                if (x != 0 && x != _xSize - 1 && y == 1)
                    continue;

                Console.SetCursorPosition(snake.Head.X * _xSize + x, snake.Head.Y * _ySize + y);
                Console.Write(snake.HeadSymbol);
                for (int i = 1; i < snake.Length; i++)
                {
                    Console.SetCursorPosition(snake.GetBody(i).X * _xSize + x, snake.GetBody(i).Y * _ySize + y);
                    Console.Write(snake.BodySymbol);
                    Console.SetCursorPosition(0, 0);
                }
            }
        }
    }

    void DrawSnake(Snake snake, Vector2Int oldTailTip)
    {
        Console.ForegroundColor = snake.SuperMode ? GetRandomColor(false) : ConsoleColor.White;

        for (int x = 0; x < _xSize; x++)
        {
            for (int y = 0; y < _ySize; y++)
            {

                if (x != 0 && x != _xSize - 1 && y == 1)
                    continue;

                if (snake.Head.X * _xSize + x >= Console.WindowWidth)
                    return;

                Console.SetCursorPosition(snake.Head.X * _xSize + x, snake.Head.Y * _ySize + y);
                Console.Write(snake.HeadSymbol);
                Console.SetCursorPosition(snake.GetBody(1).X * _xSize + x, snake.GetBody(1).Y * _ySize + y);
                Console.Write(snake.BodySymbol);
                Console.SetCursorPosition(snake.Last.X * _xSize + x, snake.Last.Y * _ySize + y);
                Console.Write(snake.BodySymbol);
                if (oldTailTip != new Vector2Int(-1, -1))
                {
                    Console.SetCursorPosition(oldTailTip.X * _xSize + x, oldTailTip.Y * _ySize + y);
                    Console.Write(" ");
                }
            }
        }
    }

    ConsoleColor GetRandomColor(bool includeBlack)
    {
        Random random = new();
        var startIndex = includeBlack ? 0 : 1;
        return (ConsoleColor)random.Next(startIndex, 16);
    }

    void ScheduleNextTick()
    {
        _timer = new ScheduleTimer(_tickRate, Tick);
    }
}