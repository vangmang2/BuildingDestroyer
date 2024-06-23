using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupGameOver : MonoBehaviour
{
    [SerializeField] Text txtScore;
    [SerializeField] CanvasGroup cg;

    public PopupGameOver SetTextScore(string text)
    {
        txtScore.text = text;
        return this;
    }

    public void SetActive(bool enable)
    {
        gameObject.SetActive(enable);
    }

    public void ShowPopup()
    {
        cg.DOFade(1f, 2f);
    }
}
