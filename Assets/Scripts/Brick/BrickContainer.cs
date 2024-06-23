using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

// 벽돌을 통째로 관리하는 스크립트
// 하나의 벽돌이 깨지면 포지션의 기준이 하나 깨진만큼 뒤로 이동되는 방식,
// 그리고 하나깨진 만큼 다른 블록이 앞으로 당겨와짐
public class BrickContainer : MonoBehaviour
{
    public const int startBrickCount = 40;

    [SerializeField] Vector2 startPosition, endPosition;
    [SerializeField] float gap;
    [SerializeField] float targetVelocity, targetAcceleration;


    public Vector3 currPosition => transform.position;
    public float currVelocity { get; private set; }
    public float currAcceleration { get; private set; }
    public Brick currBrick => brickQueue.Peek();

    bool hasFloorTouched;
    int currBrickCount;

    Action onFloorTouched, onStageCleared;
    Queue<Brick> brickQueue = new Queue<Brick>();

    bool canMove;
    public static BrickContainer instance { get; private set; }

    private void Awake()
    {
        instance = this;
        var brickArr = GetComponentsInChildren<Brick>();
        foreach (var brick in brickArr)
        {
            brickQueue.Enqueue(brick);
        }
        currVelocity = targetVelocity;
    }

    public void InitBricks(int stage)
    {
        var i = 0;
        foreach (var brick in brickQueue)
        {
            var targetPos = startPosition + new Vector2(gap * i, 0f);

            brick.SetColliderActive(false)
                 .SetHitpoint(3 + stage * 4)
                 .SetLocalPosition(targetPos)
                 .SetActionOnDead(OnDead)
                 .SetRandomSprites();
            i++;
        }
        brickQueue.Peek().SetColliderActive(true);
    }

    public void IncreaseBrickCount(int amount)
    {
        currBrickCount += amount;
    }

    void OnDead(Brick brick)
    {
        currBrickCount--;
        if (currBrickCount == 0)
            onStageCleared?.Invoke();
        if (currBrickCount <= 0)
            return;

        brick.InstantiateBrickBrokenParticle();
        transform.position += Vector3.right * gap;
        var destroyedBrick = brickQueue.Dequeue();
        brickQueue.Enqueue(destroyedBrick);

        var count = brickQueue.Count;
        for (int i = 0; i < count; i++)
        {
            var targetBrick = brickQueue.ElementAt(i);
            if (i > currBrickCount)
            {
                continue;
            }

            var targetPos = startPosition + new Vector2(gap * i, 0f);
            targetBrick.SetColliderActive(false)
                       .SetHitpoint(10)
                       .SetLocalPosition(targetPos);
        }
        destroyedBrick.SetRandomSprites();
        brickQueue.Peek().SetColliderActive(true);
    }

    public BrickContainer SetActionOnFloorTouched(Action callback)
    {
        onFloorTouched = callback;
        return this;
    }

    public BrickContainer SetActionOnStageClear(Action callback)
    {
        onStageCleared = callback;
        return this;
    }

    CancellationTokenSource cts;
    public void DestroyBricks(int count, Action onComplete)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        InvokeDestroyBricks(count, onComplete).Forget();
    }

    async UniTaskVoid InvokeDestroyBricks(int count, Action onComplete)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: cts.Token);
        for (int i = 0; i < count; i++)
        {
            var brick = brickQueue.Peek();
            OnDead(brick);
            GameManager.instance.IncreaseScore(100);
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: cts.Token);
        }
        await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: cts.Token);
        onComplete?.Invoke();
    }

#if UNITY_EDITOR
    [Button]
    void SetBrickPosOnEditor()
    {
        var brickArr = GetComponentsInChildren<Brick>();

        var count = brickArr.Length;
        for (int i = 0; i < count; i++)
        {
            var brick = brickArr[i];
            var targetPos = startPosition + new Vector2(gap * i, 0f);
            brick.SetLocalPosition(targetPos);
        }
    }
#endif

    public void EffectedByGuard()
    {
        currAcceleration = -targetAcceleration * 1.25f;
    }


    public BrickContainer SetEnableMove(bool enable)
    {
        canMove = enable;
        return this;
    }

    private void Update()
    {
        if (currBrickCount <= 0)
        {
            transform.position = new Vector3(65f, 0f, 0f);
            return;
        }

        if (!canMove)
            return;

        currAcceleration = Mathf.Lerp(currAcceleration, targetAcceleration, Time.deltaTime);
        currVelocity = targetVelocity * currAcceleration;
        transform.position += Vector3.left * currVelocity * Time.deltaTime;

        if (transform.position.x <= endPosition.x)
        {
            transform.position = endPosition;
            if (hasFloorTouched)
                return;

            hasFloorTouched = true;
            onFloorTouched?.Invoke();
        }
        else
        {
            hasFloorTouched = false;
        }
    }

    private void OnDestroy()
    {
        cts?.Cancel();
    }
}
