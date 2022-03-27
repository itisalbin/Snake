public class Snake
{
    public bool SuperMode { get; set; } = false;
    List<Vector2Int> _body { get; set; } = new List<Vector2Int>();
    public int Length => _body.Count;
    public Vector2Int Head => _body[0];
    public Vector2Int Last => _body[Length - 1];
    public string HeadSymbol => "O";
    public string BodySymbol => "\u2588";

    Dir _currentDir;
    bool _grow = false;
    public Snake(int length)
    {
        var headPos = length - 1;
        for (int i = 0; i < length; i++)
        {
            _body.Add(new Vector2Int(headPos - i, 0));
        }
        _currentDir = Dir.Right;
    }

    public bool CheckIfIntersecting()
    {
        for (int i = 1; i < _body.Count; i++)
        {
            if (Head == _body[i])
            {
                return true;
            }
        }
        return false;
    }

    public IEnumerable<Vector2Int> GetBody()
    {
        return _body;
    }

    public Vector2Int GetBody(int index)
    {
        return _body[index];
    }

    public void TeleportHead(Vector2Int position)
    {
        _body[0] = position;
    }

    public void Grow()
    {
        _grow = true;
    }

    public Vector2Int Move(Dir dir)
    {
        var newBody = new List<Vector2Int>();
        var oldHead = _body[0];

        if (CheckValidTurn(dir))
        {
            newBody.Add(GetNextHead(dir, oldHead));
            _currentDir = dir;
        }
        else {
            newBody.Add(GetNextHead(_currentDir, oldHead));
        }
        for (int i = 0; i < _body.Count - 1; i++)
        {
            newBody.Add(_body[i]);
        }

        var oldTailTip = _body[_body.Count - 1];

        if (_grow)
        {
            _grow = false;
            newBody.Add(oldTailTip);
            oldTailTip = new Vector2Int(-1, -1);
        }

        _body = newBody;

        return oldTailTip;
    }


    Vector2Int GetNextHead(Dir dir, Vector2Int oldHead)
    {
        switch (dir)
        {
            case Dir.Left:
                return new Vector2Int(oldHead.X - 1, oldHead.Y);
            case Dir.Right:
                return new Vector2Int(oldHead.X + 1, oldHead.Y);
            case Dir.Up:
                return new Vector2Int(oldHead.X, oldHead.Y - 1);
            case Dir.Down:
                return new Vector2Int(oldHead.X, oldHead.Y + 1);
        }
        return new Vector2Int();
    }

    bool CheckValidTurn(Dir dir)
    {
        if (dir == _currentDir)
        {
            return false;
        }

        if (Math.Abs((int)dir - (int)_currentDir) == 1)
        {
            return false;
        }

        return true;
    }

    
}

public enum Dir {Left = 0, Right = 1, Up = 3, Down = 4 }