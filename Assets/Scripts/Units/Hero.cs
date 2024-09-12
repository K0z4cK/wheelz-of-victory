using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Unit
{
    [SerializeField]
    private SpriteRenderer _shield;

    private bool _isDefended;

    public void StartDefend() 
    {
        _isDefended = true;
        _shield.gameObject.SetActive(true);
    }
    public void StopDefend() 
    {
        _isDefended = false;
        _shield.gameObject.SetActive(false);
    }

    public override bool GetDamage(int damage)
    {
        if (_isDefended) return false;

        _currentHealth -= damage;

        LevelController.Instance.SetHeroHealth(_currentHealth, _maxHealth);

        _damageParticles.Play();
        _unitAnimator.SetTrigger("GetDamage");

        if (_currentHealth <= 0)
        {
            Death();
            LevelController.Instance.ShowLosePanel();
            return true;
        }
        return false;
    }
}

