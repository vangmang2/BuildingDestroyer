using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDLifeShower : MonoBehaviour
{
    [SerializeField] List<GameObject> goList;

    public void SetLifeActive(int currLife)
    {
        for (int i = 0; i < goList.Count; i++)
        {
            var go = goList[i];
            var enable = i < currLife;
            go.transform.DOKill();
            go.transform.localScale = Vector3.one;

            if (currLife == 1 && i == 0)
            {
                go.transform.DOScale(1.1f, 0.5f).SetEase(Ease.InBack).SetLoops(-1, LoopType.Yoyo);
            }
            
            go.SetActive(enable);
        }
    }
}
