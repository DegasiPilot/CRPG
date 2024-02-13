using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public delegate void Interact(Transform transform, Component interactComponent);

    public float Speed;

    private Vector3 _targetPosition;
    private float _maxSqrTargetOffset;
    private Vector3 _direction;
    private CharacterController _controller;
    private Interact _interact;
    private Transform _interactObject;
    private Component _interactComponent;

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
                if (_interact != null && _interactObject && _interactComponent)
                {
                    _interact.Invoke(_interactObject, _interactComponent);
                    _interact = null;
                    _interactObject = null;
                    _interactComponent = null;
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

    public void InteractWith(Transform interactObject, float maxInteractDistance, Interact interact, Component interactComponent)
    {
        GoToPosition(interactObject.position, maxInteractDistance);
        _interact = interact;
        _interactObject = interactObject;
        _interactComponent = interactComponent;
    }
}