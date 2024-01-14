using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class btntest : MonoBehaviour
{
    private void Awake()
    {
        this.AttachTimer(1f, (t) =>
        {
            var obj = GetComponent<RectTransform>();
            Debug.Log(obj.anchoredPosition);
            Debug.Log(obj.rect);
            Debug.Log(obj.anchorMin);
            Debug.Log(obj.anchorMax);
            Debug.Log(obj.localPosition);
        }, isLooped: true);
    }

    private void Update()
    {
    }
}
