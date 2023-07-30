using UnityEngine;
using UnityEngine.UI;

public class PlayButtonsMenu : MonoBehaviour
{
    private bool isFastForwarded = false;
    private bool isPaused = false;

    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private Button fastforwardButton;
    [SerializeField]
    private Sprite playButtonSprite;
    [SerializeField]
    private Sprite fastforwardButtonSprite;
    [SerializeField]
    private float fastForwardTimeScale = 5f;

    public void OnPauseButtonClicked()
    {
        isPaused = true;

        pauseMenu.SetActive(true);

        UpdateTimeScale();
    }

    public void OnResumeButtonClicked()
    {
        isPaused = false;

        pauseMenu.SetActive(false);

        UpdateTimeScale();
    }

    public void OnFastForwardButtonClicked()
    {
        isFastForwarded = !isFastForwarded;

        UpdateTimeScale();

        fastforwardButton.image.sprite = isFastForwarded ? playButtonSprite : fastforwardButtonSprite;
    }

    public void UpdateTimeScale()
    {
        Time.timeScale = isPaused ? 0f : isFastForwarded ? fastForwardTimeScale : 1f;
    }
}