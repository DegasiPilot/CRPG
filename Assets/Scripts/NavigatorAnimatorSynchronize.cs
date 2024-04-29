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
    private Vector2 _smoothDeltaPosition;

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
        if (Agent.enabled && PersonageController.IsMoving)
        {
            Vector3 target = Agent.nextPosition;
            Vector3 deltaPos = target - _rootTransform.position;
            deltaPos.y += Agent.baseOffset;
            if (deltaPos != Vector3.zero)
            {
                target.y = _rootTransform.position.y;
                Vector2 velocity = MakeSmoothStep(deltaPos);
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
                Agent.transform.position = Agent.nextPosition;
            }
        }
        else
        {
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

    public Vector2 MakeSmoothStep(Vector3 worldDeltaPosition)
    {
        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        _smoothDeltaPosition = Vector2.Lerp(_smoothDeltaPosition, deltaPosition, smooth);

        Vector3 currentVelocity;
        // Update velocity if time advances.
        if (Time.deltaTime > 0)
        {
            currentVelocity = _smoothDeltaPosition / Time.deltaTime;
        }
        else
        {
            currentVelocity = Vector2.zero;
        }
        return currentVelocity;
    }
}