public interface IFocusable
{
    bool HasFocus { get; }
    void Focus();
    void UnFocus();
}