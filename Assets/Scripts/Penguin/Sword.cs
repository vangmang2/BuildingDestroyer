using DG.Tweening;
using System;
using UnityEngine;


public class Sword : MonoBehaviour
{
    bool isSwing, isGaurd;

    Action onGuard;

    public Sword SetActionOnGuard(Action callback)
    {
        onGuard = callback;
        return this;
    }


    // origin 12.28
    // 112.36 -> -26.4
    public void Swing()
    {
        if (isGaurd)
            return;

        isSwing = true;
        transform.DOKill();

        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.Euler(0f, 0f, 12.28f);

        transform.DORotate(new Vector3(0f, 0f, 112.36f), 0.05f);
        transform.DORotate(new Vector3(0f, 0f, -26.4f), 0.05f).SetDelay(0.05f);
        transform.DORotate(new Vector3(0f, 0f, 12.28f), 0.05f).SetDelay(0.1f).OnComplete(() =>
        {
            isSwing = false;
        });
    }

    public void Guard()
    {
        if (isSwing)
            return;

        isGaurd = true;
        transform.DOKill();

        transform.localPosition = new Vector3(0.330000013f, -2.6500001f, 0f);
        transform.rotation = Quaternion.Euler(0f, 0f, 109.233f);

        Invoke(nameof(Rollback), 0.1f);
    }

    void Rollback()
    {
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.Euler(0f, 0f, 12.28f);

        isGaurd = false;
    }
}
