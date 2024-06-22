using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    public static FollowingCamera instance { get; private set; }

    [SerializeField] Transform target;
    [SerializeField] Vector3 revison;
    bool canFollow;

    public Vector3 targetRevision => revison;

    private void Awake()
    {
        instance = this;
        canFollow = true;
    }

    private void LateUpdate()
    {
        if (!canFollow)
            return;
        transform.position = Vector3.Lerp(transform.position, target.position + revison, 10f * Time.deltaTime);
    }

    public void SetEnableFollowing(bool enable)
    {
        canFollow = enable;
    }

    public void MoveCameraTo(Vector3 target, float duration, float delay, Action<FollowingCamera> onComplete)
    {
        transform.DOKill();
        canFollow = false;
        transform.DOMove(target, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            onComplete?.Invoke(this);
        }).SetDelay(delay);
    }
}
