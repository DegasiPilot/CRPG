using CRPG.DataSaveSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CRPG
{
	public class CameraController : MonoBehaviour
	{
		public int MouseSensitivity;
		public int MoveSpeed;
		public float MaxDistanceFromPlayer;
		public int RotateSpeed;
		public int ZoomSpeed;
		public float MinZoomDistance;
		public float MaxZoomDistance;

		private float _standartAngleX;
		private RaycastHit _cursorRaycastHit;

		private Vector3 _playerPosition => GameData.ActivePlayer.gameObject.transform.position;

		public void Awake()
		{
			_standartAngleX = transform.eulerAngles.x;
		}

		public void Setup(GameManager gameManager)
		{
			gameManager.OnDeathEvent.AddListener(() => enabled = false);
		}

		void Update()
		{
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				bool IsClick = Input.GetMouseButtonDown(0);
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out _cursorRaycastHit, 100f))
				{
					Component objectUnderPointer = _cursorRaycastHit.rigidbody;
					if (objectUnderPointer == null)
					{
						objectUnderPointer = _cursorRaycastHit.collider;
					}

					if (objectUnderPointer.TryGetComponent(out PlayerController player))
					{
						GameManager.Instance.OnPlayerUnderPointer(player);
					}
					else if (objectUnderPointer.TryGetComponent(out PersonageController personageController))
					{
						if (IsClick)
						{
							GameManager.Instance.OnPersonagePressed(personageController);
						}
						else
						{
							GameManager.Instance.OnPersonageUnderPointer(personageController);
						}
					}
					else if (objectUnderPointer.TryGetComponent(out Item item))
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
						if (IsClick)
						{
							GameManager.Instance.OnGroundPressed(_cursorRaycastHit.point);
						}
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
			transform.parent.Translate(Vector3.up * (_playerPosition.y - transform.parent.position.y));
			transform.parent.Translate(new Vector3(h, 0, v) * Time.deltaTime * MoveSpeed);
			Vector3 cameraPos = new Vector3(transform.parent.position.x, transform.parent.position.y, transform.parent.position.z);
			float sqrDistance = Vector3.SqrMagnitude(cameraPos - _playerPosition);
			if (sqrDistance > MaxDistanceFromPlayer * MaxDistanceFromPlayer)
			{
				transform.parent.position = Vector3.MoveTowards(_playerPosition, cameraPos, MaxDistanceFromPlayer);
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
					vRotation = (int)(90 - currentXAngle);
				}
				else if (vRotation <= 15 - currentXAngle)
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

		public void OnSetActivePlayer(Transform playerTransform)
		{
			transform.parent.position = new Vector3(playerTransform.position.x, transform.parent.position.y, playerTransform.position.z);
		}

		private void OnDisable()
		{
			GameManager.Instance?.NothingUnderPointer();
		}
	}
}