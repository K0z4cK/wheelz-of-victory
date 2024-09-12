using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType { Melee, Range }

public class Unit : MonoBehaviour
{
    protected SpriteRenderer _renderer;
    protected Transform _transform;

    [SerializeField]
    protected string _enemyTag;

    [SerializeField]
    private int _price;
    public int Price => _price;

    [SerializeField]
    protected int _maxHealth;
    protected int _currentHealth;
    [SerializeField]
    protected int _damage;
    [SerializeField]
    protected float _attackDistance;
    [SerializeField]
    protected float _attackDelay;
    [SerializeField]
    protected AttackType _attackType;
    
    [SerializeField]
    protected Animator _effectsAnimator;
    protected string _slashTigger;

    [SerializeField]
    protected ParticleSystem _damageParticles;

    protected Animator _unitAnimator;
    protected List<Unit> _enemyList = new List<Unit>();
    protected Unit _currentEnemy;

    protected Coroutine _attackCoroutine;
    protected bool _isAttacking = false;

    protected bool _isFlipedLeft;
    protected bool _isAllie = true;

    protected virtual void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _transform = transform;
        _currentHealth = _maxHealth;
        _unitAnimator = GetComponent<Animator>();

        //_attackCoroutine = AttackCoroutine();

        if (gameObject.CompareTag("Allie"))
        {
            _slashTigger = "White";
            _isAllie = true;
        }
        else
        {
            _slashTigger = "Red";
            _isAllie = false;
        }

        if (_transform.position.x > 0 && _renderer.flipX)
            _renderer.flipX = false;
        else if (_transform.position.x < 0 && !_renderer.flipX)
            _renderer.flipX = true;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {       
        if (!collision.CompareTag(_enemyTag))
            return;
        if (_isAllie && collision.transform.position.y < _transform.position.y)
            return;
        else if (!_isAllie && collision.transform.position.y > _transform.position.y)
            return;

        print("trigger " + _enemyTag);
        var enemyUnit = collision.GetComponent<Unit>();
        if (_currentEnemy == null)
        {
            _currentEnemy = enemyUnit;
            StartCoroutine(CheckIsEnemyNullCoroutine());
            _attackCoroutine =  StartCoroutine(AttackCoroutine());
        }
        else if (!_enemyList.Contains(enemyUnit))
        {
            if (Vector2.Distance(_transform.position, collision.transform.position) < Vector2.Distance(_transform.position, _currentEnemy.transform.position))
            {
                _enemyList.Insert(0, _currentEnemy);
                _currentEnemy = enemyUnit;
            }
            else
                _enemyList.Add(enemyUnit);
            print("add enemy to list");
        }
    }

    private IEnumerator CheckIsEnemyNullCoroutine()
    {
        yield return new WaitUntil(() => _currentEnemy == null);

        if (_enemyList.Count > 0)
            SetNextEnemy();
        else
        {
            StopCoroutine(_attackCoroutine);
            _transform.DOPlay();
            _isAttacking = false;
        }
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitUntil(() => Vector2.Distance(_transform.position, _currentEnemy.transform.position) <= _attackDistance);
        _transform.DOPause();
        _isAttacking = true;
        while (_currentEnemy != null)
        {
            AttackCurrentEnemy();
            yield return new WaitForSeconds(_attackDelay);
        }
        _transform.DOPlay();
        _isAttacking = false;
        print("attack coroutine stop");
    }

    private void AttackCurrentEnemy()
    {   
        if(_attackType == AttackType.Melee)
            MeleeAttack();
        else
            RangeAttack();
    }

    private void MeleeAttack()
    {

        _effectsAnimator.SetTrigger(_slashTigger);
        DealDamage(_currentEnemy);
    }

    private void RangeAttack()
    {
        var newProjectile = ProjectilesPool.Instance.GetProjectile(_transform.position);
        newProjectile.Init(_currentEnemy.transform, DealDamage);
    }

    private void DealDamage(Unit enemy)
    {
        bool isDead = enemy.GetDamage(_damage);

        if (isDead && _enemyList.Count > 0)
            SetNextEnemy();
    }

    public virtual bool GetDamage(int damage)
    {
        _currentHealth -= damage;

        _damageParticles.Play();
        _unitAnimator.SetTrigger("GetDamage");

        if (_currentHealth <= 0)
        {
            Death();
            return true;
        }
        return false;
    }

    protected virtual void Death()
    {
        //play animation
        Destroy(gameObject);
    }
    private void SetNextEnemy()
    {
        _currentEnemy = _enemyList[0];
        _enemyList.RemoveAt(0);
        //StartCoroutine(AttackCoroutine());
        StartCoroutine(CheckIsEnemyNullCoroutine());

        print("set next enemy");
    }

    private void OnDestroy()
    {
        _transform.DOKill();
        StopAllCoroutines();
    }
}
