using UnityEngine;
using DialogueSystem.Runtime;
using UnityEngine.EventSystems;

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
    private RaycastHit _cursorRaycastHit;

    private Vector3 _playerPosition => GameManager.Instance.PlayerController.gameObject.transform.position;

    public void Awake()
    {
        Instance = this;
        _standartAngleX = transform.eulerAngles.x;
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            bool IsClick = Input.GetMouseButtonDown(0);
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out _cursorRaycastHit, 100f))
            {
                if (_cursorRaycastHit.collider.transform.TryGetComponent(out TerrainCollider _))
                {
                    GameManager.Instance.NothingUnderPointer();
                    if (IsClick)
                    {
                        GameManager.Instance.OnGroundPressed(_cursorRaycastHit.point);
                    }
                }
                else if (_cursorRaycastHit.collider.transform.TryGetComponent(out PersonageController personageController))
                {
                    if (IsClick)
                    {
                        GameManager.Instance.OnPersonagePressed(personageController);
                    }
                    else
                    {
                        GameManager.Instance.OnPersonageUnderPointer(personageController.Personage);
                    }
                }
                else if(_cursorRaycastHit.collider.transform.TryGetComponent(out Item item))
                {
                    if (IsClick)
                    {
                        GameManager.Instance.OnItemPressed(item);
                    }
                    else
                    {
                        GameManager.Instance.OnItemUnderPointer(item);
                    }
                }
                else
                {
                    GameManager.Instance.NothingUnderPointer();
                }
            }
            else
            {
                GameManager.Instance.NothingUnderPointer();
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
        if (sqrDistance > MaxDistanceFromPlayer * MaxDistanceFromPlayer)
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

    public void FocusOn(DialogueActor dialogueActor)
    {
        transform.parent.position = dialogueActor.transform.position;
        transform.parent.rotation = dialogueActor.PlaceForCamera.rotation;
        transform.position = dialogueActor.PlaceForCamera.position;
        transform.localEulerAngles = Vector3.zero;
    }

    public void StandartView()
    {
        transform.parent.eulerAngles = Vector3.zero;
        transform.localEulerAngles = new Vector3(_standartAngleX, 0, 0);
        transform.localPosition = -transform.forward * MinZoomDistance;
    }
}