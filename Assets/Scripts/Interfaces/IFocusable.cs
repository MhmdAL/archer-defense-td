public interface IFocusable
{
    bool HasFocus { get; }
    void Focus();
    void UnFocus();
}

public interface IMoving
{
    Stat MoveSpeed { get; set; }
}