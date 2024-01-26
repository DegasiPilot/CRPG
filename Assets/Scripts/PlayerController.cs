using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;

    private Vector3 targetPosition;
    private Vector3 direction;
    private CharacterController controller;

    public void Setup()
    {
        controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        if(direction != Vector3.zero)
        {
            controller.SimpleMove(direction * Speed);
            if((targetPosition - transform.position).sqrMagnitude <= 0.01)
            {
                direction = Vector3.zero;
            }
        }
    }

    public void GoToPosition(Vector3 position)
    {
        targetPosition = position;
        direction = (targetPosition - transform.position).normalized;
        transform.forward = direction;
    }
}
