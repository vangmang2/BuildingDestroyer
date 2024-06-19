using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public class Penguin : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float slidingTime, velocity;
    [SerializeField] Vector3 homePos;
    [SerializeField] AnimationCurve risingCurve, fallingCurve;
    bool isSliding, isTowardToBuildings;

    float t, currVelocity;
    private void Update()
    {
        GetInput();
        Slide();
    }

    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isSliding)
                return;

            animator.SetBool("IsSliding", true);
            isSliding = true;
            isTowardToBuildings = true;
            t = 0f;
            currVelocity = velocity;
        }
    }

    void Slide()
    {
        if (isSliding)
        {
            t += Time.deltaTime;

            if (isTowardToBuildings)
            {
                currVelocity = velocity * risingCurve.Evaluate(t * 2f / slidingTime);                ;
                transform.position += Vector3.right * currVelocity * Time.deltaTime;

                if (t >= slidingTime * 0.5f)
                {
                    t = 0f;
                    isTowardToBuildings = false;
                }

            }
            else
            {
                currVelocity = velocity * fallingCurve.Evaluate(t * 2f / slidingTime);
                transform.position -= Vector3.right * currVelocity * Time.deltaTime;
                if (transform.position.x <= homePos.x)
                {
                    isSliding = false;
                    animator.SetBool("IsSliding", false);
                    transform.position = homePos;
                }
            }
        }

    }
}
