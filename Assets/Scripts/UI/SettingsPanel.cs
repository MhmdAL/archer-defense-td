using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    private bool _isFastForwarded = false;
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
        if (Input.GetKeyDown(KeyCode.P))
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
        _isFastForwarded = Time.timeScale > 1;
        _isPaused = Time.timeScale == 0;
        _gameManager.timeState = _isPaused ? TimeState.Paused : TimeState.Playing;

        fastforwardButton.image.sprite = _isFastForwarded ? playButtonSprite : fastforwardButtonSprite;
    }

    public void OnPauseButtonClicked()
    {
        _isPaused = true;
        _gameManager.timeState = TimeState.Paused;

        AudioUtils.FadeOutAllSounds();

        pauseMenu.SetActive(true);

        UpdateTimeScale();
    }

    public void OnResumeButtonClicked()
    {
        _isPaused = false;
        _gameManager.timeState = TimeState.Playing;

        AudioUtils.FadeInAllSounds();

        pauseMenu.SetActive(false);

        UpdateTimeScale();
    }

    public void OnRestartButtonClicked()
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
        _isFastForwarded = !_isFastForwarded;

        UpdateTimeScale();

        fastforwardButton.image.sprite = _isFastForwarded ? playButtonSprite : fastforwardButtonSprite;
    }

    private void UpdateTimeScale()
    {
        Time.timeScale = _isPaused ? 0f : _isFastForwarded ? fastForwardTimeScale : 1f;
        _currentTimeScale = Time.timeScale;
    }
}