using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollisionDetector : MonoBehaviour
{
    [SerializeField] Sword sword;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.tag.Equals("Brick"))
            return;

        var brick = collision.GetComponent<Brick>();
        sword.HitBrick(brick);
    }

}
