using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillDisplayUI : MonoBehaviour
{
    public GameObject SkillIndicatorCirclePrefab;

    public GameObject SkillIndicatorGrid;

    [SerializeField]
    private int _maxLevel;
    public int MaxLevel
    {
        get
        {
            return _maxLevel;
        }
        set
        {
            _maxLevel = value;
            UpdateIndicator();
        }
    }

    [SerializeField]
    private int _currentLevel;
    public int CurrentLevel
    {
        get
        {
            return _currentLevel;
        }
        set
        {
            _currentLevel = value;
            UpdateIndicator();
        }
    }

    public Color DefaultColor;
    public Color SkillfulColor;

    private List<GameObject> _indicators = new List<GameObject>();

    private void Start()
    {
        UpdateIndicator();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            CurrentLevel++;
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            CurrentLevel--;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            MaxLevel++;
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            MaxLevel--;
        }
    }

    public void UpdateIndicator()
    {
        foreach (var indicator in _indicators)
        {
            Destroy(indicator.gameObject);
        }

        _indicators.Clear();

        for (int i = 0; i < MaxLevel; i++)
        {
            var indicator = Instantiate(SkillIndicatorCirclePrefab, Vector3.zero, Quaternion.identity, SkillIndicatorGrid.transform);

            indicator.GetComponent<Image>().color = DefaultColor;
            _indicators.Add(indicator);
        }

        for (int i = 0; i < Mathf.Clamp(CurrentLevel, 0, MaxLevel); i++)
        {
            _indicators[i].GetComponent<Image>().color = SkillfulColor;
        }
    }
}
