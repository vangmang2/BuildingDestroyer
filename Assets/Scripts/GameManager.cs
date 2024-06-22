using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum GamePhase
{
    abilitySelection,
    idle,
    pause
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] IngameUIManager gameUIManager;
    [SerializeField] BrickContainer brickContainer;
    [SerializeField] Penguin penguin;

    int score;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        penguin.SetActionOnDead(OnPenguinDead)
               .SetActionOnHitpointChanged(gameUIManager.SetLife)
               .SetActionOnHit(IncreaseScore);
        brickContainer.SetActionOnFloorTouched(DecreasePenguinHitpoint);
    }

    CancellationTokenSource cts;
    void IncreaseScore()
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();

        var prev = score;
        var curr = score + 100;
        score += 100;

        ShowTextScore(prev, curr).Forget();
    }

    async UniTaskVoid ShowTextScore(int prevScore, int currScore)
    {
        var t = 0f;
        while (t <= 0.5f)
        {
            t += Time.deltaTime;
            var targetScore = (int)Mathf.Lerp(prevScore, currScore, t * 2f);
            gameUIManager.SetTextScore(targetScore.ToString());
            await UniTask.Yield(cancellationToken: cts.Token);
        }
    }

    void DecreasePenguinHitpoint()
    {
        penguin.DecreaseHitpoint(1);
    }

    void OnPenguinDead()
    {
    }
}
