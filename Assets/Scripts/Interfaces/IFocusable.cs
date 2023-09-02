public interface IFocusable
{
    bool HasFocus { get; }
    void Focus();
    void UnFocus();
}

public interface IMovable
{
    Stat MoveSpeed { get; set; }
}