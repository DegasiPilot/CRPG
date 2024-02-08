using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public delegate void Interact(Transform transform);

    public float Speed;

    private Vector3 _targetPosition;
    private float _maxSqrTargetOffset;
    private Vector3 _direction;
    private CharacterController _controller;
    private Interact _interact;
    private Transform _interactObject;

    public void Setup()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        if (_direction != Vector3.zero)
        {
            _controller.SimpleMove(_direction * Speed);
            if ((_targetPosition - transform.position).sqrMagnitude <= _maxSqrTargetOffset)
            {
                _direction = Vector3.zero;
                if (_interact != null && _interactObject)
                {
                    _interact.Invoke(_interactObject);
                    _interact = null;
                    _interactObject = null;
                }
            }
        }
    }

    public void GoToPosition(Vector3 position, float maxTargetOffset = 0.1f)
    {
        _maxSqrTargetOffset = maxTargetOffset * maxTargetOffset;
        _targetPosition = position;
        _direction = (_targetPosition - transform.position).normalized;
        transform.forward = _direction;
        _interact = null;
        _interactObject = null;
    }

    public void InteractWith(Transform transform, float maxInteractDistance, Interact interact, Transform interactObject)
    {
        GoToPosition(transform.position, maxInteractDistance);
        _interact = interact;
        _interactObject = interactObject;
    }
}