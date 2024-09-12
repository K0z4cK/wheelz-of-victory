using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    [SerializeField]
    private GameObject _panel;

    [SerializeField]
    private Button _pause;
    [SerializeField]
    private Button _play;

    [SerializeField]
    private Sprite _pauseSprite;
    [SerializeField]
    private Sprite _playSprite;

    private bool _isPaused;

    private void Awake()
    {
        _pause.onClick.AddListener(PauseGame);
        _play.onClick.AddListener(ContinueGame);
    }

    private void PauseGame()
    {
        _pause.onClick.RemoveAllListeners();
        _pause.onClick.AddListener(ContinueGame);
        Time.timeScale = 0f;
        _panel.SetActive(true);
        _pause.image.sprite = _playSprite;
    }

    private void ContinueGame()
    {
        _pause.onClick.RemoveAllListeners();
        _pause.onClick.AddListener(PauseGame);
        Time.timeScale = 1.0f;
        _panel.SetActive(false);
        _pause.image.sprite = _pauseSprite;
    }

}
