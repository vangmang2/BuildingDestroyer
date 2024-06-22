using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameUIManager : MonoBehaviour
{
    [SerializeField] HUDLifeShower lifeShower;
    [SerializeField] Text txtScore;
    [SerializeField] Image imgLethalMoveFilledBar, imgGuardGageFilledBar;
    [SerializeField] GameObject goLethalMoveStress;

    public void SetActiveLethalMoveStress(bool enable)
    {
        goLethalMoveStress.SetActive(enable);
    }

    public void SetFilledAmountLethalMove(float amount)
    {
        imgLethalMoveFilledBar.fillAmount = amount;
    }

    public void SetFilledAmountGuardGage(float amount)
    {
        imgGuardGageFilledBar.fillAmount = amount;
    }

    public void SetLife(int life)
    {
        lifeShower.SetLifeActive(life);
    }

    public void SetTextScore(string text)
    {
        txtScore.text = text;
    }
}
