using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameUIManager : MonoBehaviour
{
    [SerializeField] HUDLifeShower lifeShower;
    [SerializeField] Text txtScore;

    public void SetLife(int life)
    {
        lifeShower.SetLifeActive(life);
    }

    public void SetTextScore(string text)
    {
        txtScore.text = text;
    }
}
