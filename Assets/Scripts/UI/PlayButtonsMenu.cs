using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    private float _currentTimeScale;

    private void LateUpdate()
    {
        if (_currentTimeScale != Time.timeScale)
            OnTimescaleChanged();

        _currentTimeScale = Time.timeScale;
    }

    private void OnTimescaleChanged()
    {
        isFastForwarded = Time.timeScale > 1;
        isPaused = Time.timeScale == 0;

        fastforwardButton.image.sprite = isFastForwarded ? playButtonSprite : fastforwardButtonSprite;
    }

    public void OnPauseButtonClicked()
    {
        isPaused = true;

        AudioUtils.FadeOutAllSounds();

        pauseMenu.SetActive(true);

        UpdateTimeScale();
    }

    public void OnResumeButtonClicked()
    {
        isPaused = false;

        AudioUtils.FadeInAllSounds();

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
        _currentTimeScale = Time.timeScale;
    }
}