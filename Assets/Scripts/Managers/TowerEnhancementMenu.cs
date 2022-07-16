using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TowerEnhancementMenu : MonoBehaviour
{
    [field: SerializeField]
    private GameObject _menu;

    [field: SerializeField]
    private List<Button> _enhancementButtons;

    [field: SerializeField]
    private List<TowerEnhancementSpriteData> _sprites { get; set; }

    private List<Image> _enhancementButtonSprites;

    private ValueStore _vs;

    private void Awake()
    {
        _vs = FindObjectOfType<ValueStore>();

        _enhancementButtonSprites = _enhancementButtons.Select(x => x.GetComponentInChildren<Image>()).ToList();
    }

    public void SetEnhancements(List<EnhancementType> types)
    {
        _enhancementButtons.ForEach(x => x.gameObject.SetActive(false));

        var activeButtons = _enhancementButtons.Take(types.Count).ToList();

        for (int i = 0; i < types.Count; i++)
        {
            var type = types[i];

            _enhancementButtons[i].gameObject.SetActive(true);
            _enhancementButtons[i].onClick.RemoveAllListeners();
            _enhancementButtons[i].onClick.AddListener(() => _vs.uiControllerInstance.ApplyTowerEnhancement((int)type));
            _enhancementButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = _sprites.FirstOrDefault(x => x.EnhancementType == types[i]).Sprite;
        }
    }
}

[System.Serializable]
public struct TowerEnhancementSpriteData
{
    [field: SerializeField]
    public EnhancementType EnhancementType { get; set; }

    [field: SerializeField]
    public Sprite Sprite { get; set; }
}