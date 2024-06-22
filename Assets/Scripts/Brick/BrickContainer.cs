using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

// 벽돌을 통째로 관리하는 스크립트
// 하나의 벽돌이 깨지면 포지션의 기준이 하나 깨진만큼 뒤로 이동되는 방식,
// 그리고 하나깨진 만큼 다른 블록이 앞으로 당겨와짐
public class BrickContainer : MonoBehaviour
{
    const int maxBrickCount = 12;

    [SerializeField] Vector2 startPosition, endPosition;
    [SerializeField] float gap;
    [SerializeField] float targetVelocity, targetAcceleration;


    public Vector3 currPosition => transform.position;
    public float currVelocity { get; private set; }
    public float currAcceleration { get; private set; }
    public Brick currBrick => brickQueue.Peek();

    bool hasFloorTouched; 
    int currBrickCount;

    Action onFloorTouched;
    Queue<Brick> brickQueue = new Queue<Brick>();

    public static BrickContainer instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        var brickArr = GetComponentsInChildren<Brick>();
        foreach (var brick in brickArr)
        {
            brickQueue.Enqueue(brick);
        }

        currVelocity = targetVelocity;
        
        foreach (var brick in brickQueue)
        {
            brick.SetColliderActive(false)
                 .SetHitpoint(10)
                 .SetActionOnDead(OnDead);
        }
        brickQueue.Peek().SetColliderActive(true);
    }

    void OnDead(Brick brick)
    {
        transform.position += Vector3.right * gap;
        var destroyedBrick = brickQueue.Dequeue();
        brickQueue.Enqueue(destroyedBrick);

        var count = brickQueue.Count;
        for (int i = 0; i < count; i++)
        {
            var targetBrick = brickQueue.ElementAt(i);
            var targetPos = startPosition + new Vector2(gap * i, 0f);
            targetBrick.SetColliderActive(false)
                       .SetHitpoint(10)
                       .SetLocalPosition(targetPos);
        }
        brickQueue.Peek().SetColliderActive(true);
    }

    public BrickContainer SetActionOnFloorTouched(Action callback)
    {
        onFloorTouched = callback;
        return this;
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
        currAcceleration = -targetAcceleration * 0.65f;
    }

    private void Update()
    {
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
}
