public struct Vector2Int {
    public int X { get; set; }
    public int Y { get; set; }

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static bool operator ==(Vector2Int a, Vector2Int b)
    {
        if (a.X == b.X && a.Y == b.Y)
        {
            return true;
        }
        return false;
    }

    public static bool operator !=(Vector2Int a, Vector2Int b)
    {
        if (a.X == b.X && a.Y == b.Y)
        {
            return false;
        }
        return true;
    }
}