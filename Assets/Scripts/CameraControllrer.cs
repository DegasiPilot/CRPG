using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DialogueSystem.Runtime;

public class CameraController : MonoBehaviour
{
    public int MouseSensitivity;
    public int MoveSpeed;
    public int RotateSpeed;
    public int ZoomSpeed;
    public float MinDistance;
    public float MaxDistance;

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
                    DialogueParser.Instance.SetSecondDialogueActor(dialogueActor);
                    DialogueParser.Instance.TryStartDialogue();
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
        if (h == 0 && v == 0 && Input.GetMouseButton(0))
        {
            h = Input.GetAxis("Mouse X") * -MouseSensitivity;
            v = Input.GetAxis("Mouse Y") * -MouseSensitivity;
        }
        transform.parent.Translate(new Vector3(h, 0, v) * Time.deltaTime * MoveSpeed);
    }

    private void Rotate()
    {
        float rotation;
        if (Input.GetKey(KeyCode.Q))
            rotation = 1;
        else if (Input.GetKey(KeyCode.E))
            rotation = -1;
        else if (Input.GetMouseButton(1))
            rotation = Input.GetAxis("Mouse X") * MouseSensitivity;
        else
            return;

        rotation = rotation * RotateSpeed * Time.deltaTime;
        transform.parent.Rotate(Vector3.up, rotation);
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
                (moveDistance > 0 && distance > MinDistance) ||
                (moveDistance < 0 && distance < MaxDistance))
            {
                var newPosition = Vector3.MoveTowards(position, parentPosition, moveDistance);
                var newDistance = Vector3.Distance(newPosition, parentPosition);
                if (newDistance < MinDistance)
                {
                    moveDistance = distance - MinDistance;
                }
                transform.position = Vector3.MoveTowards(position, parentPosition, moveDistance);
            }
        }
    }
}