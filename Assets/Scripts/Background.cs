using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] Transform target;

    private void Update()
    {
        var position = target.position * 0.6f;
        position.y = -2.36f;
        transform.position = position;
    }

}
