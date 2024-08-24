using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    private TimeScaleState _timeScaleState;
    private bool _isPaused = false;

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
    private ValueStore _gameManager;

    private void Awake()
    {
        _gameManager = FindObjectOfType<ValueStore>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_isPaused)
                OnPauseButtonClicked();
            else
                OnResumeButtonClicked();
        }
    }

    private void LateUpdate()
    {
        if (_currentTimeScale != Time.timeScale)
            OnTimescaleChanged();

        _currentTimeScale = Time.timeScale;
    }

    private void OnTimescaleChanged()
    {
        _timeScaleState = Time.timeScale > 1 ? TimeScaleState.Fast : TimeScaleState.Normal;
        _isPaused = Time.timeScale == 0;
        _gameManager.timeState = _isPaused ? TimeState.Paused : TimeState.Playing;

        UpdateFastForwardButtonSprite();
    }

    public void OnPauseButtonClicked()
    {
        PauseGame();
    }

    public void PauseGame()
    {
        _isPaused = true;
        _gameManager.timeState = TimeState.Paused;

        AudioUtils.FadeOutAllSounds();

        pauseMenu.SetActive(true);

        UpdateTimeScale();
    }

    public void OnResumeButtonClicked()
    {
        ResumeGame();
    }

    public void ResumeGame()
    {
        _isPaused = false;
        _gameManager.timeState = TimeState.Playing;

        AudioUtils.FadeInAllSounds();

        pauseMenu.SetActive(false);

        UpdateTimeScale();
    }

    public void OnRestartButtonClicked()
    {
        RestartGame();
    }

    public void RestartGame()
    {
        _isPaused = false;
        _gameManager.timeState = TimeState.Playing;

        AudioUtils.FadeInAllSounds();

        pauseMenu.SetActive(false);

        _gameManager.RestartLevel();

        UpdateTimeScale();
    }

    public void OnHomeButtonClicked()
    {
        Time.timeScale = 1;
        GlobalManager.instance.LoadScene("menu", 1f);
    }

    public void OnFastForwardButtonClicked()
    {
        _timeScaleState = (TimeScaleState)((int)(_timeScaleState + 1) % Enum.GetNames(typeof(TimeScaleState)).Length); // iterate between different FF states

        SetTimeScale(_timeScaleState);
    }

    public void SetTimeScale(TimeScaleState timeScaleState)
    {
        _timeScaleState = timeScaleState;

        UpdateTimeScale();

        UpdateFastForwardButtonSprite();
    }

    private void UpdateTimeScale()
    {
        if (_isPaused)
        {
            Time.timeScale = 0f;
        }
        else if (_timeScaleState == TimeScaleState.Normal)
        {
            Time.timeScale = 1f;
        }
        else if (_timeScaleState == TimeScaleState.Fast)
        {
            Time.timeScale = fastForwardTimeScale;
        }

        _currentTimeScale = Time.timeScale;
    }

    private void UpdateFastForwardButtonSprite()
    {
        fastforwardButton.image.sprite = _timeScaleState == TimeScaleState.Fast ? playButtonSprite : fastforwardButtonSprite;
    }
}

public enum TimeScaleState
{
    Normal,
    Fast
}