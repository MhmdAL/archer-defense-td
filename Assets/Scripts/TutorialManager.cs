using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityTimer;

public class TutorialManager : MonoBehaviour
{
    public GameObject dialogMenu;
    public TextMeshProUGUI dialogText;

    public Image backdropImage;

    public int currentStage;

    private void Start()
    {
        this.transform.SetSiblingIndex(100);
    }

    public IEnumerator SetActiveStage(int n, float opacity = 0.6f)
    {
        currentStage = n - 1;

        dialogMenu.SetActive(true);

        var imageColor = backdropImage.color;
        imageColor.a = opacity;
        backdropImage.color = imageColor;

        var stage = dialogMenu.transform.Find($"Stage{n}");

        var canvasGroup = stage.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        stage.gameObject.SetActive(true);

        canvasGroup.DOFade(1f, 0.5f);

        yield return new WaitForSeconds(0.5f);
    }

    public void EnableBackdrop()
    {
        backdropImage.DOFade(0.58f, 0.5f);
    }

    public void DisableBackdrop()
    {
        backdropImage.DOFade(0f, 1f);
    }

    public void SetText(string text)
    {
        dialogMenu.SetActive(true);
        dialogText.gameObject.SetActive(true);
        dialogText.text = text;
    }

    public IEnumerator CloseActiveStage(float fadeOutDuration = 1f)
    {
        // print("Closing tutorial menu");

        var curStage = transform.GetChild(currentStage);

        print("fading out stage: " + curStage.name);

        var tween = curStage.GetComponent<CanvasGroup>().DOFade(0f, fadeOutDuration);
        tween.onComplete = () =>
        {
            curStage.gameObject.SetActive(false);
            dialogMenu.SetActive(false);

            print("done fading out stage: " + curStage.name);
        };

        yield return new WaitForSeconds(fadeOutDuration);
    }
}
