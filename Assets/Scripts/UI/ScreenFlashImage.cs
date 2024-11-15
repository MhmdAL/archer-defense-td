using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScreenFlashImage : MonoBehaviour
{
    public float FlashInDuration = 0.2f;
    public float FlashInAlpha = 0.7f;
    public float FlashOutDuration = 0.3f;
    public float FlashOutAlpha = 0f;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void Flash(float? flashInDuration = null, float? flashOutDuration = null, float? flashInAlpha = null, float? flashOutAlpha = null)
    {
        flashInDuration ??= FlashInDuration;
        flashOutDuration ??= FlashOutDuration;

        flashInAlpha ??= FlashInAlpha;
        flashOutAlpha ??= FlashOutAlpha;

        var tween = _image.DOFade(flashInAlpha.Value, flashInDuration.Value);

        tween.onComplete = () =>
        {
            _image.DOFade(flashOutAlpha.Value, flashOutDuration.Value);
        };
    }
}
