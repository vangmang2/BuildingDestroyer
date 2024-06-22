using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class PopupItemSelection : MonoBehaviour
{
    [SerializeField] List<UIItemItemShower> itemShowers;
    Action onClickItem;

    private void Start()
    {
        foreach (var shower in itemShowers)
        {
            shower.SetActionOnClick((item) =>
            {
                var penguin = Penguin.instance;
                switch (item)
                {
                    case RandomItem.increaseDamage:
                        penguin.IncreaseDamage(4);
                        break;
                    case RandomItem.increaseGuardGageFactor:
                        penguin.IncreaseGuardGageFactor(3);
                        break;
                    case RandomItem.increaseLethalMoveHitCount:
                        penguin.IncreaseLethalMoveHitCount(1);
                        break;
                    case RandomItem.heal:
                        penguin.SetHitpoint(3);
                        break;
                }
                onClickItem?.Invoke();
            });
        }
    }

    public PopupItemSelection SetActionOnClickItem(Action onClick)
    {
        onClickItem = onClick;
        return this;
    }

    public void ShowRandomItems()
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        InvokeShowRandomItems().Forget();
    }

    CancellationTokenSource cts;
    async UniTaskVoid InvokeShowRandomItems()
    {
        foreach (var shower in itemShowers)
        {
            var item = (RandomItem)Random.Range(0, Enum.GetNames(typeof(RandomItem)).Length);
            shower.SetScale(Vector3.zero)
                  .SetItemType(item)
                  .SetItemSprite()
                  .SetItemDescription()
                  .Appear();
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: cts.Token);
        }
    }

    public void SetActive(bool enable)
    {
        gameObject.SetActive(enable);
    }

    private void OnDestroy()
    {
        cts?.Cancel();
    }
}
