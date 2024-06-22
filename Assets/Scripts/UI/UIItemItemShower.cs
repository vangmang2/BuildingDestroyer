using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RandomItem
{
    increaseDamage,
    increaseLethalMoveHitCount,
    heal,
    increaseGuardGageFactor
}
public class UIItemItemShower : MonoBehaviour
{
    [SerializeField] List<Sprite> spriteList;
    [SerializeField] Image imgType;
    [SerializeField] Text txtDescription;
    RandomItem item;

    Action<RandomItem> onClick;

    public UIItemItemShower SetScale(Vector3 scale)
    {
        transform.localScale = scale;
        return this;
    }

    public UIItemItemShower SetItemType(RandomItem item)
    {
        this.item = item;
        return this;
    }
    
    public UIItemItemShower SetActionOnClick(Action<RandomItem> onClick)
    {
        this.onClick = onClick;
        return this;
    }

    public UIItemItemShower SetItemSprite()
    {
        var sprite = spriteList[(int)item];
        imgType.sprite = sprite;
        return this;
    }

    public UIItemItemShower SetItemDescription()
    {
        switch (item)
        {
            case RandomItem.increaseDamage:
                txtDescription.text = "데미지 증가";
                break;
            case RandomItem.increaseGuardGageFactor:
                txtDescription.text = "가드 게이지 회복 속도 증가";
                break;
            case RandomItem.increaseLethalMoveHitCount:
                txtDescription.text = "궁극기 타격 횟수 증가";
                break;
            case RandomItem.heal:
                txtDescription.text = "체력 회복";
                break;
        }
        return this;
    }

    public void Appear()
    {
        transform.DOKill();
        transform.localScale = Vector3.zero;

        transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnClick()
    {
        onClick?.Invoke(item);
    }
}
