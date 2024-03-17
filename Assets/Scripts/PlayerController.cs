using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public delegate void Interact(GameObject gameObject, Component interactComponent);

    public float Speed;
    public ActionType ActiveAction { get => _activeAction; set => _activeAction = value; }

    private NavMeshAgent _controller;
    private Rigidbody _rigidBody;
    private Interact _interact;
    private GameObject _interactObject;
    private Component _interactComponent;
    private ActionType _activeAction;

    private bool _isGrounded = true;
    private bool _jumpStartJump;

    private bool _isFree => _isGrounded;

    private ActionType _defaultAction = ActionType.Movement;

    public void Setup()
    {
        _controller = GetComponent<NavMeshAgent>();
        _rigidBody = GetComponent<Rigidbody>();
        _activeAction = ActionType.Movement;
    }

    private void Update()
    {
        if (_interact != null && _interactObject && _interactComponent && (!_controller.hasPath || _controller.remainingDistance <= _controller.stoppingDistance))
        {
            _interact.Invoke(_interactObject, _interactComponent);
            _interact = null;
            _interactObject = null;
            _interactComponent = null;
            _controller.ResetPath();
        }
    }

    public void OnGroundPressed(Vector3 hitPoint)
    {
        if (!_isFree) return;
        switch (_activeAction)
        {
            case ActionType.Movement:
                GoToPosition(hitPoint);
                break;
            case ActionType.Jumping:
                JumpToPosition(hitPoint);
                break;
        }
    }

    private void GoToPosition(Vector3 position, float maxTargetOffset = 0.1f)
    {
        _controller.SetDestination(position);
        _controller.stoppingDistance = maxTargetOffset;
        _interact = null;
        _interactObject = null;
    }

    public void InteractWith(GameObject interactObject, float maxInteractDistance, Interact interact, Component interactComponent)
    {
        GoToPosition(interactObject.transform.position, maxInteractDistance);
        _interact = interact;
        _interactObject = interactObject;
        _interactComponent = interactComponent;
    }

    public void PickupItem(Item item)
    {
        GameData.Inventory.Add(item);
        item.gameObject.SetActive(false);
        item.IsInInventory = true;
    }

    public void DropItem(Item item)
    {
        GameData.Inventory.Remove(item);
        item.transform.position = gameObject.transform.position + gameObject.transform.forward;
        item.gameObject.SetActive(true);
        item.IsInInventory = false;
    }

    public void JumpToPosition(Vector3 target)
    {
        _isGrounded = false;
        _controller.enabled = false;
        _rigidBody.isKinematic = false;
        _jumpStartJump = true;

        float gravity = Physics.gravity.magnitude;
        float angle = 45 * Mathf.Deg2Rad;

        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(target.x, 0, target.z);
        Vector3 planarPostion = new Vector3(transform.position.x, 0, transform.position.z);

        float planarDistance = Vector3.Distance(planarTarget, planarPostion);
        float yOffset = transform.position.y - target.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(planarDistance, 2)) / (planarDistance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion) * (planarTarget.x > planarPostion.x ? 1 : -1);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        _rigidBody.AddForce(finalVelocity, ForceMode.VelocityChange);
        SetDefaultAction();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_jumpStartJump)
        {
            _jumpStartJump = false;
        }
        else if (!_isGrounded && collision.GetContact(0).point.y < transform.position.y)
        {
            _isGrounded = true;
            _controller.enabled = true;
            _rigidBody.isKinematic = true;
        }
    }

    public void SetDefaultAction()
    {
        ActiveAction = _defaultAction;
    }
}