using DG.Tweening;
using System;
using UnityEngine;


public class Sword : MonoBehaviour
{
    [SerializeField] BoxCollider2D boxCollider;

    public Sword SetColliderActive(bool enable)
    {
        boxCollider.enabled = enable;
        return this;
    }

    // origin 12.28
    // 112.36 -> -26.4
    public void Swing()
    {
        SetColliderActive(true);
        transform.rotation = Quaternion.Euler(0f, 0f, 12.28f);
        transform.DOKill();
        transform.DORotate(new Vector3(0f, 0f, 112.36f), 0.05f);
        transform.DORotate(new Vector3(0f, 0f, -26.4f), 0.05f).SetDelay(0.05f);
        transform.DORotate(new Vector3(0f, 0f, 12.28f), 0.05f).SetDelay(0.1f).OnComplete(() =>
        {
            SetColliderActive(false);
        });
    }
}
