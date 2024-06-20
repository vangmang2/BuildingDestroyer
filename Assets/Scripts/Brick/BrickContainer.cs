using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

// ������ ��°�� �����ϴ� ��ũ��Ʈ
// �ϳ��� ������ ������ �������� ������ �ϳ� ������ŭ �ڷ� �̵��Ǵ� ���,
// �׸��� �ϳ����� ��ŭ �ٸ� ����� ������ ��ܿ���
public class BrickContainer : MonoBehaviour
{
    const int maxBrickCount = 12;

    [SerializeField] Vector2 startPosition, endPosition;
    [SerializeField] float gap;
    [SerializeField] List<Brick> brickList;
    [SerializeField] AnimationCurve guardCurve;
    [SerializeField] float targetVelocity;

    public float currVelocity { get; private set; }

    bool hasFloorTouched; 
    int currBrickCount;

    Action onFloorTouched;

    public static BrickContainer instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currVelocity = targetVelocity;
    }

    public BrickContainer SetActionOnFloorTouched(Action callback)
    {
        onFloorTouched = callback;
        return this;
    }


    [Button]
    void Init()
    {
        brickList.Clear();
        brickList = GetComponentsInChildren<Brick>().ToList();

        var count = brickList.Count;
        for (int i = 0; i < count; i++)
        {
            var brick = brickList[i];
            var targetPos = startPosition + new Vector2(gap * i, 0f);
            brick.SetLocalPosition(targetPos);
        }
    }

    public void EffectedByGuard()
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        InvokeOnGuard().Forget();
    }

    CancellationTokenSource cts;
    async UniTaskVoid InvokeOnGuard()
    {
        var t = 0f;
        var time = 1.25f;
        while (t <= 1f)
        {
            t += Time.deltaTime * 1f / time;
            currVelocity = 3.75f * guardCurve.Evaluate(t);
            await UniTask.Yield(cts.Token);
        }
        currVelocity = targetVelocity;
    }

    private void Update()
    {
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
