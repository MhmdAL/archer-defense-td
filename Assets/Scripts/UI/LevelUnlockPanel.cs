using System;
using TMPro;
using UnityEngine;

public class LevelUnlockPanel : MonoBehaviour
{
    public TextMeshProUGUI unlockType;
    public TextMeshProUGUI unlockName;

    public void SetUnlockName(string name)
    {
        unlockName.text = name;
    }

    internal void SetUnlockType(string type)
    {
        unlockType.text = type;
    }
}
