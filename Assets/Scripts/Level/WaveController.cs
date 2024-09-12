using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class WaveController : MonoBehaviour
{
    [SerializeField]
    private Slider _waveProgress;
    [SerializeField]
    private List<GameObject> _waveHearts;

    [SerializeField]
    private LineRenderer _lineRenderer;

    private List<Enemy> _enemies = new List<Enemy>(); // i have no enemies
    private int _maxEnemiesInWave = 0;

    public void Init(List<Enemy> enemies)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Enemy enemy = Instantiate(enemies[i], transform);
            if (i > 0)
                enemy.Init(RemoveEnemy, _lineRenderer, _enemies[i - 1].transform);
            else
                enemy.Init(RemoveEnemy, _lineRenderer, null);
            _enemies.Add(enemy);
        }
        _maxEnemiesInWave = _enemies.Count;
        SetWaveProgress();
    }

    private void RemoveEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
        SetWaveProgress();
        if (_enemies.Count == 0)
        {
            print("wave end");
            LevelController.Instance.StartNewWave();
            _waveHearts.ForEach(x => x.SetActive(true));
        }
    }

    public void SetWaveProgress()
    {
        int procentage = (int)((float)_enemies.Count / (float)_maxEnemiesInWave * 100f);
        _waveProgress.value = procentage / 100f;
        if (procentage < 65)
            _waveHearts[0].SetActive(false);
        if (procentage < 35)
            _waveHearts[1].SetActive(false);
        if (procentage <= 0)
            _waveHearts[2].SetActive(false);
    }

    public void DamageEnemies(int damage)
    {
        for(int i = 0; i < _enemies.Count; i++)
            if (_enemies[i] != null)
                _enemies[i].GetDamage(damage);

        /*foreach (Enemy enemy in _enemies)
            if (enemy != null)
                enemy.GetDamage(damage);*/

        //_enemies.ForEach(x => x.GetDamage(damage));
    }
}


