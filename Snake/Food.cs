public class Food {
    public string Symbol { get; } = "X";
    public bool IsSuperFood { get; } = false;

    public Vector2Int Position { get; }

    public Food(int x, int y, bool isSuperFood)
    {
        Position = new Vector2Int(x, y);
        IsSuperFood = isSuperFood;
    }
}