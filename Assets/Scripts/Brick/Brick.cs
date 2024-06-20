using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public Brick SetLocalPosition(Vector3 position)
    {
        transform.localPosition = position;
        return this;
    }
}
