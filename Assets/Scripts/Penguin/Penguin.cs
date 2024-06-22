using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public struct PenguinStats
{
    public int damage;
    public int hitpoint;
    public float slidingTime;
    public float guardGage;
    public int lethalMoveGage;
    public int lethalMoveHitCount;
}

public class Penguin : MonoBehaviour
{
    const int maxLethalMoveGage = 100;

    [SerializeField] Animator animator;
    [SerializeField] float slidingTime, velocity;
    [SerializeField] Vector3 homePos;
    [SerializeField] AnimationCurve risingCurve, fallingCurve, fallingGuardCurve;
    [SerializeField] Sword sword;

    PenguinStats _stats;

    bool isSliding, isForwardToBuilding, isAlreadyFalling, isDead, isLethalMove;
    CancellationTokenSource risingCts, fallingCts, falling_BulidingHitCts, falling_GuardCts;
    Action<int> onHitpointChanged, onHit;
    Action<int, int> onLethalMoveGageChanged;
    Action onDead;

    void Start()
    {
        _stats = new PenguinStats();
        _stats.damage = 10;
        _stats.hitpoint = 3;
        _stats.slidingTime = slidingTime;
        _stats.guardGage = 100;
        _stats.lethalMoveGage = 0;
        _stats.lethalMoveHitCount = 6;

        sword.SetActionOnGuard(Guard);
    }

    public Penguin SetActionOnDead(Action callback)
    {
        onDead = callback;
        return this;
    }

    public Penguin SetActionOnHit(Action<int> callback)
    {
        onHit = callback;
        return this;
    }

    public Penguin SetActionOnHitpointChanged(Action<int> callback)
    {
        onHitpointChanged = callback;
        return this;
    }

    public Penguin SetActionOnLethalMoveGageChanged(Action<int, int> callback)
    {
        onLethalMoveGageChanged = callback;
        return this;
    }

    public Penguin IncreaseDamage(int amount)
    {
        _stats.damage += amount;
        return this;
    }

    public Penguin IncreaseSlidingTime(float amount)
    {
        _stats.slidingTime += amount;
        return this;
    }

    public Penguin IncreaseGuardGage(int amount)
    {
        _stats.guardGage += amount;
        return this;
    }

    public Penguin IncreaseHitpoint(int amount)
    {
        _stats.hitpoint += amount;
        onHitpointChanged?.Invoke(_stats.hitpoint);
        return this;
    }

    public Penguin DecreaseHitpoint(int amount)
    {
        _stats.hitpoint -= amount;
        onHitpointChanged?.Invoke(_stats.hitpoint);

        if (_stats.hitpoint <= 0)
        {
            isDead = true;
            onDead?.Invoke();
        }
        return this;
    }

    void Update()
    {
        if (isDead)
            return;

        if (isLethalMove)
            return;

        CheckPosition();

#if UNITY_EDITOR
        GetInput();
#endif
    }

    void CheckPosition()
    {
        if (BrickContainer.instance.currPosition.x <= transform.position.x)
        {
            var targetVec = transform.position;
            targetVec.x = Mathf.Max(-4.8f, BrickContainer.instance.currPosition.x);
            transform.position = targetVec;
        }
    }

    // TODO:Penguin Input Manager������ ���ָ� ���� ��?
    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Slide();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Swing();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Guard();
        }

        if (Input.GetMouseButtonDown(2))
        {
            LethalMove();
        }
    }

    bool canInteractWithBrick =>
        Mathf.Abs(BrickContainer.instance.currPosition.x - transform.position.x) <= 1.5f ||
                  BrickContainer.instance.currPosition.x < transform.position.x;

    bool isLethalMoveValid =>
        Mathf.Abs(BrickContainer.instance.currPosition.x - transform.position.x) <= 3f ||
                  BrickContainer.instance.currPosition.x < transform.position.x;

    void LethalMove()
    {
        //if (_stats.lethalMoveGage < maxLethalMoveGage)
        //    return;

        // �ñر�� �ֻ��� UX�� ���� ��밡�� �Ÿ��� ���� ���� ��� �����ϰ� �Ѵ�
        if (!isLethalMoveValid)
            return;
        sword.Swing();
        BrickContainer.instance.SetEnableMove(false);

        isLethalMove = true;
        _stats.lethalMoveGage = 0;
        onLethalMoveGageChanged?.Invoke(_stats.lethalMoveGage, maxLethalMoveGage);

        var targetPos = transform.position + FollowingCamera.instance.targetRevision + new Vector3(_stats.lethalMoveHitCount * 3.5f, 0f);
        var duration = _stats.lethalMoveHitCount * 0.05f;
        var delay = 0.3f;
        FollowingCamera.instance.MoveCameraTo(targetPos, duration, delay, null);
        BrickContainer.instance.DestroyBricks(_stats.lethalMoveHitCount, () =>
        {
            FollowingCamera.instance.SetEnableFollowing(true);
            isLethalMove = false;
            BrickContainer.instance.SetEnableMove(true);
        });
    }

    void Slide()
    {
        if (isSliding)
            return;

        animator.SetBool("IsSliding", true);
        isSliding = true;
        isForwardToBuilding = true;
        ToForward();
    }

    void Swing()
    {
        sword.Swing();
        if (!canInteractWithBrick)
            return;

        onHit?.Invoke(10);
        var brick = BrickContainer.instance.currBrick;
        brick.GetDamaged(_stats.damage, () =>
        {
            // ��� �μ����� 5���� ������ ��������
            IncreaseLethalMoveGage();
        });
    }

    void IncreaseLethalMoveGage()
    {
        _stats.lethalMoveGage += 5;
        _stats.lethalMoveGage = Mathf.Min(_stats.lethalMoveGage, maxLethalMoveGage);
        onLethalMoveGageChanged?.Invoke(_stats.lethalMoveGage, maxLethalMoveGage);
    }


    void Guard()
    {
        sword.Guard();
        if (!canInteractWithBrick)
            return;

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
        while (t < _stats.slidingTime)
        {
            t += Time.deltaTime;
            var currVelocity = velocity * risingCurve.Evaluate(t / _stats.slidingTime);
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

            var currVelocity = velocity * fallingCurve.Evaluate(t / _stats.slidingTime);
            transform.position -= Vector3.right * currVelocity * Time.deltaTime;

            await UniTask.Yield(fallingCts.Token);
        }
        SetPositionToHomePos();
    }

    async UniTaskVoid InvokeToBackward_BuildingHit()
    {
        var t = 0f;

        while (transform.position.x > homePos.x)
        {
            t += Time.deltaTime;

            var currVelocity = BrickContainer.instance.currVelocity;
            transform.position -= Vector3.right * currVelocity * Time.deltaTime;

            await UniTask.Yield(falling_BulidingHitCts.Token);
        }
        SetPositionToHomePos();
    }

    async UniTaskVoid InvokeToBackward_Guard()
    {
        var t = 0f;

        while (transform.position.x > homePos.x)
        {
            t += Time.deltaTime;

            var currVelocity = velocity * fallingGuardCurve.Evaluate(t / _stats.slidingTime);
            transform.position -= Vector3.right * currVelocity * Time.deltaTime;

            await UniTask.Yield(falling_GuardCts.Token);
        }
        SetPositionToHomePos();
    }

    void SetPositionToHomePos()
    {
        isSliding = false;
        isAlreadyFalling = false;
        animator.SetBool("IsSliding", false);
        transform.position = homePos;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isForwardToBuilding)
            return;

        risingCts?.Cancel();
        fallingCts?.Cancel();
        falling_GuardCts?.Cancel();
        ToBackward_BuildingHit();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isAlreadyFalling)
            return;

        isAlreadyFalling = true;
        isForwardToBuilding = false;

        falling_BulidingHitCts?.Cancel();
        ToBackward();
    }

    private void OnDestroy()
    {
        risingCts?.Cancel();
        fallingCts?.Cancel();
        falling_BulidingHitCts?.Cancel();
        falling_GuardCts?.Cancel();
    }
}
