using Cysharp.Threading.Tasks;
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
    [SerializeField] PopupItemSelection popupItemSelection;

    int score, stage;
    bool canStart;

    private void Awake()
    {
        stage = 0;
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        penguin.SetActionOnDead(OnPenguinDead)
               .SetActionOnHitpointChanged(gameUIManager.SetLife)
               .SetActionOnHit(IncreaseScore)
               .SetActionOnGuardGageChanged(OnGuardGageChanged)
               .SetActionOnLethalMoveGageChanged(OnLethalMoveGageChanged);
        brickContainer.SetActionOnFloorTouched(DecreasePenguinHitpoint)
                      .SetActionOnStageClear(StageClear);
        StartNextStage();

        popupItemSelection.SetActionOnClickItem(() =>
        {
            hasItemSelected = true;
        });
    }

    void OnGuardGageChanged(float amount, float maxValue)
    {
        gameUIManager.SetFilledAmountGuardGage(amount / maxValue);
    }
    
    void OnLethalMoveGageChanged(int amount, int maxValue)
    {
        var fillAmount = amount / (float)maxValue;
        gameUIManager.SetFilledAmountLethalMove(fillAmount);
        gameUIManager.SetActiveLethalMoveStress(fillAmount >= 1f);
    }

    void StageClear()
    {
        stage++;
        if (stage == 10)
        {
            // TODO: 게임 클리어
            return;
        }

        StartNextStage();
    }

    bool hasItemSelected;
    CancellationTokenSource stageCts;

    void StartNextStage()
    {
        stageCts?.Cancel();
        stageCts = new CancellationTokenSource();
        InvokeStartNextStage().Forget();
    }

    // 1. 스테이지 클리어
    // 2. 랜덤 보상 선택
    // 3. 다음 스테이지 시작
    async UniTaskVoid InvokeStartNextStage()
    {
        hasItemSelected = false;
        popupItemSelection.SetActive(true);
        popupItemSelection.ShowRandomItems();
        await UniTask.WaitUntil(() => hasItemSelected, cancellationToken: stageCts.Token);

        brickContainer.InitBricks(stage);
        brickContainer.SetEnableMove(true);
        brickContainer.IncreaseBrickCount(BrickContainer.startBrickCount + stage * 20);
        penguin.InitGage();
        popupItemSelection.SetActive(false);

    }

    CancellationTokenSource cts;
    public void IncreaseScore(int amount)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();

        var prev = score;
        var curr = score + amount;
        score += amount;

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
