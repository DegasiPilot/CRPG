using UnityEngine;
using DialogueSystem.Runtime;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public int MouseSensitivity;
    public int MoveSpeed;
    public float MaxDistanceFromPlayer;
    public int RotateSpeed;
    public int ZoomSpeed;
    public float MinZoomDistance;
    public float MaxZoomDistance;

    private float _standartAngleX;
    private Vector3 _playerPosition => GameManager.Instance.PlayerController.gameObject.transform.position;

    public void Awake()
    {
        Instance = this;
    }

    public void Setup()
    {
        _standartAngleX = transform.eulerAngles.x;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider.transform.TryGetComponent(out TerrainCollider _))
                {
                    GameManager.Instance.PlayerController.GoToPosition(hit.point);
                }
                else if (hit.collider.transform.TryGetComponent(out DialogueActor dialogueActor))
                {
                    GameManager.Instance.OnDialogueActorPressed(dialogueActor);
                }
                else if(hit.collider.transform.TryGetComponent(out Item item))
                {
                    Debug.Log(item != null);
                    GameManager.Instance.OnItemPressed(item);
                }
            }
        }
        Move();
        Rotate();
        Zoom();
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.parent.Translate(new Vector3(h, 0, v) * Time.deltaTime * MoveSpeed);
        Vector3 cameraPos = new Vector3(transform.parent.position.x, 0, transform.parent.position.z);
        Vector3 player2DPos = new Vector3(_playerPosition.x, 0, _playerPosition.z);
        float sqrDistance = Vector3.SqrMagnitude(cameraPos - player2DPos);
        if(sqrDistance > MaxDistanceFromPlayer*MaxDistanceFromPlayer)
        {
            transform.parent.position = Vector3.MoveTowards(player2DPos, cameraPos, MaxDistanceFromPlayer);
        }
    }

    private void Rotate()
    {
        float hRotation;
        if (Input.GetKey(KeyCode.Q))
        {
            hRotation = 1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            hRotation = -1;
        }
        else if (Input.GetMouseButton(1))
        {
            hRotation = Input.GetAxis("Mouse X") * MouseSensitivity;
            float currentXAngle = transform.eulerAngles.x;
            float vRotation = -Input.GetAxis("Mouse Y") * MouseSensitivity * RotateSpeed * Time.deltaTime;
            if (vRotation >= 90 - currentXAngle)
            {
                vRotation = (int)(90-currentXAngle);
            }
            else if (vRotation <= 15- currentXAngle)
            {
                vRotation = (int)(15 - currentXAngle);
            }
            transform.RotateAround(transform.parent.position, transform.parent.right, vRotation);
        }
        else
        {
            return;
        }

        hRotation = hRotation * RotateSpeed * Time.deltaTime;
        transform.parent.Rotate(Vector3.up, hRotation);
    }

    private void Zoom()
    {
        var moveDistance = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * ZoomSpeed;
        if (moveDistance != 0)
        {
            var parentPosition = transform.parent.position;
            var position = transform.position;
            var distance = Vector3.Distance(position, parentPosition);
            if (
                (moveDistance > 0 && distance > MinZoomDistance) ||
                (moveDistance < 0 && distance < MaxZoomDistance))
            {
                var newPosition = Vector3.MoveTowards(position, parentPosition, moveDistance);
                var newDistance = Vector3.Distance(newPosition, parentPosition);
                if (newDistance < MinZoomDistance)
                {
                    moveDistance = distance - MinZoomDistance;
                }
                transform.position = Vector3.MoveTowards(position, parentPosition, moveDistance);
            }
        }
    }

    public void FocusOn(GameObject focusObject)
    {
        transform.position = focusObject.transform.position;
        transform.parent.eulerAngles = focusObject.transform.eulerAngles + new Vector3(0, 180, 0);
        transform.localPosition = new Vector3(0, 1, -2);
        transform.localEulerAngles = Vector3.zero;
    }

    public void StandartView()
    {
        transform.parent.eulerAngles = Vector3.zero;
        transform.localEulerAngles = new Vector3(_standartAngleX, 0, 0);
        transform.localPosition = -transform.forward * MinZoomDistance;
    }
}