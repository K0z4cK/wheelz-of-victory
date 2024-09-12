using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : SingletonComponent<LevelController>
{
    [SerializeField]
    private Hero _hero;

    [SerializeField]
    private Slider _heroHealth;
    [SerializeField]
    private List<GameObject> _heroHearts;

    [SerializeField]
    private TMP_Text _time;
    [SerializeField]
    private TMP_Text _coins;

    //level ui controller
    [SerializeField]
    private WaveController _waveController;

    [SerializeField]
    private List<WaveStruct> _waves;

    //private int _waveCount = 0;

    [SerializeField]
    private int _startMoney;
    private int _money;

    [SerializeField]
    private int _levelTime;
    private int _timer;

    [SerializeField]
    private int _skillDamage;
    [SerializeField]
    private int _skillCooldown;
    [SerializeField]
    private int _defenseSeconds;
    [SerializeField]
    private int _coinsSeconds;

    [SerializeField]
    private Button _skillButton;
    [SerializeField]
    private GameObject _skillTimerFrame;
    [SerializeField]
    private TMP_Text _skillTimeText;
    [SerializeField]
    private Button _defenceButton;
    [SerializeField]
    private Image _shieldImage;
    [SerializeField]
    private Sprite _shieldActiveSprite;
    [SerializeField]
    private Sprite _shieldDeactiveSprite;
    [SerializeField]
    private Button _coinsButton;
    [SerializeField]
    private Image _coinsImage;
    [SerializeField]
    private Sprite _coinsActiveSprite;
    [SerializeField]
    private Sprite _coinsDeactiveSprite;

    [SerializeField]
    private GameObject _winPanel;
    [SerializeField]
    private GameObject _losePanel;

    private bool _doubleCoins = false;  

    private void Awake()
    {
        _money = _startMoney;
        _timer = _levelTime;
        _coins.text = _money.ToString();
        ShowTime();

        _skillButton.onClick.AddListener(DamageAllEnemies);
        _defenceButton.onClick.AddListener(ActivateShield);
        _coinsButton.onClick.AddListener(ActivateDoubleCoins);

        _waveController.Init(_waves[0].enemies);
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (_timer > 0) 
        {
            yield return new WaitForSeconds(1f);
            _timer--;
            ShowTime();
        }
    }

    private void ShowTime()
    {
        int minutes = _timer / 60;
        int secons = _timer - minutes * 60;
        string secondsString;
        if (secons >= 10)
            secondsString = secons.ToString();
        else
            secondsString = "0" + secons.ToString();
        _time.text = $"{minutes}:{secondsString}";
    }

    public void AddMoney(int count)
    {
        if (_doubleCoins)
            count *= 2;
        _money += count;
        _coins.text = _money.ToString();
    }

    public bool DeductMoney(int count)
    {
        if(count > _money)
            return false;
        _money -= count;
        _coins.text = _money.ToString();
        return true;
    }

    public void SetHeroHealth(int health, int maxHealth)
    {
        int hpRocentage = (int)((float)health / (float)maxHealth * 100f);
        _heroHealth.value = hpRocentage / 100f;

        if (hpRocentage < 65)      
            _heroHearts[0].SetActive(false);
        if (hpRocentage < 35)
            _heroHearts[1].SetActive(false);
        if(hpRocentage <= 0)
            _heroHearts[2].SetActive(false);


    }

    public void StartNewWave()
    {
        /*_waveCount++;
        if (_waves.Count == _waveCount)
            print("win");
        else
            _waveController.Init(_waves[_waveCount].enemies);*/
        if (_timer > 0)
            _waveController.Init(_waves[UnityEngine.Random.Range(0, _waves.Count)].enemies);
        else
            _winPanel.SetActive(true);
    }

    public void DamageAllEnemies()
    {
        _waveController.DamageEnemies(_skillDamage);
        StartCoroutine(SkillCooldownCoroutine());
    }

    private IEnumerator SkillCooldownCoroutine()
    {
        _skillButton.interactable = false;
        _skillTimerFrame.SetActive(true);
        _skillTimeText.text = _skillCooldown.ToString();
        int timer = _skillCooldown;
        while(timer > 0)
        {
            yield return new WaitForSeconds(1f);
            timer--;
            _skillTimeText.text = timer.ToString();
        }
        _skillButton.interactable = true;
        _skillTimerFrame.SetActive(false);
    }

    public void ActivateShield()
    {
        _shieldImage.sprite = _shieldDeactiveSprite;
        _defenceButton.interactable = false;
        StartCoroutine(DefendTimeCoroutine());
    }

    private IEnumerator DefendTimeCoroutine()
    {
        _hero.StartDefend();
        int timer = _coinsSeconds;
        while (timer > 0)
        {
            yield return new WaitForSeconds(1f);
            timer--;
        }
        _hero.StopDefend();
    }

    public void EnableCoins() 
    {
        print("coins enabled");
        _coinsButton.interactable = true; 
        _coinsImage.sprite = _coinsActiveSprite;
    }

    public void ActivateDoubleCoins()
    {
        _coinsButton.interactable = false;
        _coinsImage.sprite = _coinsDeactiveSprite;
        StartCoroutine(CoinsTimeCoroutine());
    }

    private IEnumerator CoinsTimeCoroutine()
    {
        _doubleCoins = true;
        int timer = _coinsSeconds;
        while (timer > 0)
        {
            yield return new WaitForSeconds(1f);
            timer--;
        }
        _doubleCoins = false;
    }

    public void ShowLosePanel() => _losePanel.SetActive(true);

    public void RestartLevel() => SceneManager.LoadScene("Game");

}

[Serializable]
public struct WaveStruct
{
    public List<Enemy> enemies;
}
