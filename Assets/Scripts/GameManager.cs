using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] BrickContainer brickContainer;
    [SerializeField] Penguin penguin;

    // Start is called before the first frame update
    void Start()
    {
        penguin.SetActionOnDead(OnPenguinDead);
        brickContainer.SetActionOnFloorTouched(DecreasePenguinHitpoint);
    }

    void DecreasePenguinHitpoint()
    {
        penguin.DecreaseHitpoint(1);
    }

    void OnPenguinDead()
    {
    }
}
