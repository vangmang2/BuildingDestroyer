using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] ParticleSystem brickParticle;
    BoxCollider2D boxCollider2D;
    int hitpoint;
    Action<Brick> onDead;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public Brick SetColliderActive(bool enable)
    {
        boxCollider2D.enabled = enable;
        return this;
    }

    public Brick SetActionOnDead(Action<Brick> callback)
    {
        onDead = callback;
        return this;
    }

    public Brick SetHitpoint(int hitpoint)
    {
        this.hitpoint = hitpoint;
        return this;
    }

    public Brick SetLocalPosition(Vector3 position)
    {
        transform.localPosition = position;
        return this;
    }

    public Brick GetDamaged(int damage, Action _onDead)
    {
        hitpoint -= damage;
        if (hitpoint <= 0)
        {
            onDead?.Invoke(this);
            _onDead?.Invoke();
        }
        return this;
    }

    public void InstantiateBrickBrokenParticle()
    {
        Instantiate(brickParticle, transform.position, Quaternion.identity);
    }
}
