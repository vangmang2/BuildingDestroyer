using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class Penguin : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float slidingTime, velocity;
    [SerializeField] Vector3 homePos;
    [SerializeField] AnimationCurve risingCurve, fallingCurve, fallingGuardCurve;
    [SerializeField] Sword sword;
    bool isSliding, isForwardToBuilding;
    CancellationTokenSource risingCts, fallingCts, falling_BulidingHitCts, falling_GuardCts;

    void Start()
    {
        sword.SetActionOnSwing(Swing)
             .SetActionOnGuard(Guard);
    }

    void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isSliding)
                return;

            animator.SetBool("IsSliding", true);
            isSliding = true;
            isForwardToBuilding = true;
            ToForward();
        }

        if (Input.GetMouseButtonDown(0))
        {
            sword.Swing();
        }

        if (Input.GetMouseButtonDown(1))
        {
            sword.Guard();
        }
    }

    void Swing(Brick brick)
    {
        // TODO: ��Ͽ� ������ ������ ���� �߰����ֱ�
        // ����� �ı��Ǹ� ����ؼ� �������� ��ó�� ���� �ۼ�
    }

    void Guard()
    {
        risingCts?.Cancel();
        fallingCts?.Cancel();
        falling_BulidingHitCts?.Cancel();
        ToBackward_Guard();

        BrickContainer.instance.EffectedByGuard();
    }

    void ToForward()
    {
        risingCts?.Cancel();
        risingCts = new CancellationTokenSource();

        InvokeToForward().Forget();
    }

    void ToBackward()
    {
        fallingCts?.Cancel();
        fallingCts = new CancellationTokenSource();

        InvokeToBackward().Forget();
    }

    void ToBackward_BuildingHit()
    {
        falling_BulidingHitCts?.Cancel();
        falling_BulidingHitCts = new CancellationTokenSource();
        InvokeToBackward_BuildingHit().Forget();
    }

    void ToBackward_Guard()
    {
        falling_GuardCts?.Cancel();
        falling_GuardCts = new CancellationTokenSource();
        InvokeToBackward_Guard().Forget();
    }

    async UniTaskVoid InvokeToForward()
    {
        var t = 0f;
        while (t < slidingTime)
        {
            t += Time.deltaTime;
            var currVelocity = velocity * risingCurve.Evaluate(t / slidingTime);
            transform.position += Vector3.right * currVelocity * Time.deltaTime;
            await UniTask.Yield(risingCts.Token);
        }

        ToBackward();
    }

    async UniTaskVoid InvokeToBackward()
    {
        var t = 0f;

        while (transform.position.x > homePos.x)
        {
            t += Time.deltaTime;
            var currVelocity = velocity * fallingCurve.Evaluate(t / slidingTime);
            transform.position -= Vector3.right * currVelocity * Time.deltaTime;
            await UniTask.Yield(fallingCts.Token);
        }
        SetPositionToHomePos();
    }

    // �ǹ��� ���� �ӵ��� �������� �Ѵ�.
    async UniTaskVoid InvokeToBackward_BuildingHit()
    {
        var currVelocity = BrickContainer.instance.currVelocity;
        var t = 0f;

        while (transform.position.x > homePos.x)
        {
            t += Time.deltaTime;
            transform.position -= Vector3.right * currVelocity * Time.deltaTime;
            await UniTask.Yield(falling_BulidingHitCts.Token);
        }
        SetPositionToHomePos();
    }

    // ����ϸ� ������ �ӵ��� �������� �Ѵ�.
    async UniTaskVoid InvokeToBackward_Guard()
    {
        var t = 0f;

        while (transform.position.x > homePos.x)
        {
            t += Time.deltaTime;
            var currVelocity = velocity * fallingGuardCurve.Evaluate(t / slidingTime);
            transform.position -= Vector3.right * currVelocity * Time.deltaTime;
            await UniTask.Yield(falling_GuardCts.Token);
        }
        SetPositionToHomePos();
    }

    void SetPositionToHomePos()
    {
        if (transform.position.x <= homePos.x)
        {
            isSliding = false;
            animator.SetBool("IsSliding", false);
            transform.position = homePos;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isForwardToBuilding)
            return;

        risingCts?.Cancel();
        fallingCts?.Cancel();
        ToBackward_BuildingHit();
    }

    private void OnDestroy()
    {
        risingCts?.Cancel();
        fallingCts?.Cancel();
        falling_BulidingHitCts?.Cancel();
        falling_GuardCts?.Cancel();
    }
}
