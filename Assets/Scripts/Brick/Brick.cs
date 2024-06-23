using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Brick : MonoBehaviour
{
    [SerializeField] List<Sprite> spriteList;
    [SerializeField] SpriteRenderer sprUp, sprDown;
    [SerializeField] ParticleSystem brickParticle;
    BoxCollider2D boxCollider2D;
    int hitpoint;
    Action<Brick> onDead;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public Brick SetRandomSprites()
    {
        var sprites = spriteList.OrderBy(item => Guid.NewGuid()).ToList();
        sprUp.sprite = sprites[0];
        sprDown.sprite = sprites[1];
        return this;
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
