using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class BackgroundScaler : MonoBehaviour, IFocusable, IPointerClickHandler {

    public event Action BackgroundClicked;
	public GameObject spike;

	void Start () {
       /* SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) 
			return;

        float width = sr.sprite.bounds.size.x;
        float height = sr.sprite.bounds.size.y;

        float worldScreenHeight = (Camera.main.orthographicSize * 2f) + 2;
        float worldScreenWidth = (worldScreenHeight / Screen.height * Screen.width) + (1);

        Vector3 xWidth = transform.localScale;
        xWidth.x = worldScreenWidth / width;
        //transform.localScale = xWidth;
        //transform.localScale.x = worldScreenWidth / width;
        Vector3 yHeight = transform.localScale;
        yHeight.y = worldScreenHeight / height;
        //transform.localScale = yHeight;
        //transform.localScale.y = worldScreenHeight / height;
        */
    }

	#region IPointerClickHandler implementation
	public void OnPointerClick (PointerEventData eventData)
	{
		// ValueStore.sharedInstance.OnClick (ClickType.Background, gameObject);
        // BackgroundClicked?.Invoke();
	}

    public void Focus()
    {
        
    }

    public void UnFocus()
    {
        
    }
    #endregion
}
