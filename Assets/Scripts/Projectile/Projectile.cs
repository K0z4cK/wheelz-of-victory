using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _basicSpeed;
    private string _targetTag;

    private UnityAction<Unit> onTargetReach;

    public void Init(Transform target, UnityAction<Unit> targetReach)
    {
        onTargetReach = targetReach;
        _targetTag = target.tag;

        float time = Vector2.Distance(transform.position, target.position) / _basicSpeed;
        RotateArrow(target);
        transform.DOMove(target.position, time).SetEase(Ease.Linear).OnComplete(delegate { ProjectilesPool.Instance.ReleaseProjectile(this); });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(_targetTag))
        {
            transform.DOKill();
            onTargetReach?.Invoke(collision.GetComponent<Unit>());
            ProjectilesPool.Instance.ReleaseProjectile(this);
        }
    }

    private void RotateArrow(Transform target)
    {
        Vector3 targ = target.transform.position;
        targ.z = 0f;

        Vector3 objectPos = transform.position;
        targ.x = targ.x - objectPos.x;
        targ.y = targ.y - objectPos.y;

        float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }
}
