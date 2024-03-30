using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D raidCursor;
    public Texture2D possibleRaidCursor;

    private Texture2D _currentCursor;

    private void Awake()
    {
        UseCursor(CursorType.Default);
    }

    /// <summary>
    /// Updates cursor based on whether the cursor is above the given collider or not.
    /// </summary>
    public void UpdateBasedOnCollider(Collider2D collider, CursorType onColliderType, CursorType offColliderType)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics2D.RaycastAll(ray.origin, ray.direction);
        
        if (hits.Select(x => x.collider).Contains(collider))
        {
            UseCursor(onColliderType, new Vector2(16, 16));
        }
        else
        {
            UseCursor(offColliderType, new Vector2(16, 16));
        }
    }

    public void UseCursor(CursorType cursorType, Vector2? offset = null)
    {
        var cursorTexture = defaultCursor;

        switch (cursorType)
        {
            case CursorType.PendingRaid:
                cursorTexture = raidCursor;
                break;
            case CursorType.PossibleRaid:
                cursorTexture = possibleRaidCursor;
                break;
            case CursorType.Default:
            default:
                break;
        }

        offset ??= Vector2.zero;

        _currentCursor = cursorTexture;
        Cursor.SetCursor(cursorTexture, offset.Value, CursorMode.Auto);
    }
}

public enum CursorType
{
    Default,
    PendingRaid,
    PossibleRaid
}