using UnityEngine;
using UnityEngine.AI;

public class NavigatorAnimatorSynchronize : MonoBehaviour
{
    public NavMeshAgent Agent;
    public AnimatorManager AnimatorManager;
    public PersonageController PersonageController;
    public bool IsSynchronizeRotation;
    public bool NormalizeSpeed;

    private Transform _rootTransform;

    private float normalAgentSpeed;

    private void Start()
    {
        if (NormalizeSpeed)
        {
            normalAgentSpeed = Agent.speed;
        }
        Agent.updatePosition = false;
        Agent.updateRotation = !IsSynchronizeRotation;
        _rootTransform = Agent.transform;
        SceneSaveLoadManager.OnSceneLoaded.AddListener(() =>
            {
                AnimatorManager.enabled = true;
                _rootTransform.position = Agent.nextPosition;
            }
        );
    }

    private void OnAnimatorMove()
    {
        if(Agent == null)
        {
            Destroy(this);
            return;
        }
        if (Agent.enabled && PersonageController.IsMoving)
        {
            Vector3 target = Agent.nextPosition;
            Vector3 deltaPos = target - _rootTransform.position;
            deltaPos.y += Agent.baseOffset;
            if (deltaPos != Vector3.zero)
            {
                target.y = _rootTransform.position.y;
                Vector2 velocity = ToLocalSpace(deltaPos);
                if (NormalizeSpeed)
                {
                    velocity /= normalAgentSpeed;
                }
                if (IsSynchronizeRotation)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(target - _rootTransform.position);
                    float angle = Quaternion.Angle(lookRotation, _rootTransform.rotation);
                    float t = Mathf.Min(1, Agent.angularSpeed * Time.deltaTime / angle);
                    _rootTransform.rotation = Quaternion.Slerp(_rootTransform.rotation, lookRotation, t);
                    AnimatorManager.SetVelocity(velocity.y);
                }
                else
                {
                    AnimatorManager.SetVelocity(velocity);
                }
                _rootTransform.position = Agent.nextPosition;
            }
        }
        else if(Agent.nextPosition != _rootTransform.position)
        {
            _rootTransform.position = Agent.nextPosition;
            if (IsSynchronizeRotation)
            {
                AnimatorManager.SetVelocity(0);
            }
            else
            {
                AnimatorManager.SetVelocity(Vector2.zero);
            }
        }
    }

    public Vector2 ToLocalSpace(Vector3 deltaPos)
    {
        //Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(_rootTransform.right, deltaPos);
        float dy = Vector3.Dot(_rootTransform.forward, deltaPos);
        Vector2 currentVelocity = new Vector2(dx, dy);

        // Update velocity if time advances.
        if (Time.deltaTime > 0)
        {
            currentVelocity = currentVelocity / Time.deltaTime;
        }
        else
        {
            currentVelocity = Vector2.zero;
        }
        return currentVelocity;
    }
}