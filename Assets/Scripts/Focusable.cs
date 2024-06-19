using EPOOutline;
using UnityEngine;

public class Focusable : MonoBehaviour, IFocusable
{
    public GameObject focusIndicatorArrow;
    public Outlinable outlinable; 

    public bool HasFocus { get; set; }

    public virtual void Focus()
    {
        HasFocus = true;

        if (focusIndicatorArrow != null)
            focusIndicatorArrow.SetActive(true);
    }

    public virtual void UnFocus()
    {
        HasFocus = false;

        if (focusIndicatorArrow != null)
            focusIndicatorArrow.SetActive(false);
    }

    public void Highlight()
    {
        outlinable.OutlineParameters.Enabled = true;
    }

    public void UnHighlight()
    {
        outlinable.OutlineParameters.Enabled = false;
    }
}
