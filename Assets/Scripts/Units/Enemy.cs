using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : Unit
{
    private UnityAction<Enemy> onDied;

    [SerializeField]
    private int _moneyGain;

    [SerializeField]
    private float _moveTime;

    private Vector2 _prevPostion;

    private LineRenderer _lineRenderer;
    [SerializeField]
    private Transform _nextInLine;

    private IEnumerator _distanceCoroutine;

    public void Init(UnityAction<Enemy> died, LineRenderer lineRenderer, Transform nextInLine)
    {
        onDied = died;
        _lineRenderer = lineRenderer;
        _nextInLine = nextInLine;

        Vector3[] path = new Vector3[_lineRenderer.positionCount];
        _lineRenderer.GetPositions(path);
        _transform.DOPath(path, _moveTime);
        _prevPostion = transform.position;

        if (_nextInLine != null)
        {
            _distanceCoroutine = DistanceCoroutine();
            StartCoroutine(CheckFrountCoroutine());
            StartCoroutine(_distanceCoroutine);
        }
    }

    private IEnumerator CheckFrountCoroutine()
    {
        yield return new WaitUntil(() => _nextInLine == null);
        StopCoroutine(_distanceCoroutine);
        if(!_isAttacking)
            _transform.DOPlay();
    }

    private IEnumerator DistanceCoroutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => Vector2.Distance(_transform.position, _nextInLine.position) <= _attackDistance / 2);
            _transform.DOPause();
            yield return new WaitUntil(() => Vector2.Distance(_transform.position, _nextInLine.position) > _attackDistance);
            _transform.DOPlay();
        }
               
    }

    private void FixedUpdate()
    {
        Vector2 direction = _prevPostion - (Vector2)_transform.position;
        if (direction == Vector2.zero)
            return;

        if (direction.x > 0)
            _renderer.flipX = true;
        else
            _renderer.flipX = false;

        _prevPostion = transform.position;
    }

    protected override void Death()
    {
        onDied?.Invoke(this);
        LevelController.Instance.AddMoney(_moneyGain);

        if(Random.Range(0, 101) <= 10)
            LevelController.Instance.EnableCoins();

        base.Death();
    }
}
