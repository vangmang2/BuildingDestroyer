using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    int hitpoint;
    Action<Brick> onDead;

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

    public Brick GetDamaged(int damage)
    {
        hitpoint -= damage;
        if (hitpoint <= 0)
            onDead?.Invoke(this);

        return this;
    }
}
