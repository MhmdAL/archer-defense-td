using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject dialogMenu;
    public TextMeshProUGUI dialogText;

    public int currentStage;

    private void Start()
    {
        this.transform.SetSiblingIndex(100);
    }

    public void SetActiveStage(int n, float opacity = 0.6f)
    {
        dialogMenu.SetActive(true);

        var image = dialogMenu.GetComponent<Image>();
        var imageColor = image.color;
        imageColor.a = opacity;
        image.color = imageColor;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        // print("Setting stage " + n + " to active");

        dialogMenu.transform.Find($"Stage{n}").gameObject.SetActive(true);
    }


    public void SetText(string text)
    {
        dialogMenu.SetActive(true);
        dialogText.gameObject.SetActive(true);
        dialogText.text = text;
    }

    public void Close()
    {
        // print("Closing tutorial menu");

        dialogMenu.SetActive(false);
    }
}
