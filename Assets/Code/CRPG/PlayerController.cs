using CRPG;
using CRPG.Battle;
using CRPG.DataSaveSystem;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerChooseAttackForceModule))]
public class PlayerController : PersonageController
{
	/// <summary>
	/// Ќасколько близко надо нажать к концу пути
	/// </summary>
	public float PathEndHitAccuracy;
	public LineRenderer AccesableLineRenderer;
	public LineRenderer UnaccesableLineRenderer;
	public GameObject Sphere;

	public GameObject Inventory;

	private float _normalAgentSpeed;

	protected override void OnValidate()
	{
		base.OnValidate();
		if (_chooseAttackForceModule == null)
		{
			_chooseAttackForceModule = GetComponent<PlayerChooseAttackForceModule>();
		}
	}

	protected override void Setup()
	{
		base.Setup();
		_normalAgentSpeed = _controller.speed;
	}

	public void OnGroundPressedInFree(Vector3 hitPoint)
	{
		if (!IsFree || !NavMesh.SamplePosition(hitPoint, out NavMeshHit navMeshHit, 0.5f, NavMesh.AllAreas))
		{
			return;
		}
		switch (_activeAction)
		{
			case ActionType.Movement:
				GoToPosition(navMeshHit);
				break;
			case ActionType.Jumping:
				TryJump(navMeshHit);
				break;
		}
		if (_activeAction != _defaultAction)
		{
			SetDefaultAction();
		}
	}

	private void TryJump(NavMeshHit navMeshHit)
	{
		float jumpDistance = Vector3.Distance(navMeshHit.position, transform.position);
		if (jumpDistance < GlobalRules.MaxJumpDistance)
		{
			float calculatedHeight = Mathf.Clamp(
				jumpDistance * _distanceToHeightRatio,
				_minJumpHeight,
				_maxJumpHeight
			);
			float duration = PhysicHelper.CalculateJumpDuration(calculatedHeight);
			StartCoroutine(JumpToPosition(navMeshHit.position, calculatedHeight, duration));
		}
	}

	public void OnGroundPressedInBattle(Vector3 hitPoint)
	{
		if (!IsFree || !NavMesh.SamplePosition(hitPoint, out NavMeshHit navMeshHit, 0.5f, NavMesh.AllAreas))
		{
			return;
		}
		if (BattleManager.ActivePersonageController == this)
		{
			switch (_activeAction)
			{
				case ActionType.Movement:
					if(BattleManager.RemainMovement > 0)
					{
						GroundPressedInBattleForMovement(navMeshHit.position);
					}
					break;
				case ActionType.Jumping:
					if(Personage.RemainActions > 0)
					{
						TryJump(navMeshHit);
						Personage.RemainActions--;
					}
					break;
			}
		}
	}

	public void GroundPressedInBattleForMovement(Vector3 hitPoint)
	{
		if (_lastHitPoint != Vector3.positiveInfinity && _navMeshPath != null && _navMeshPath.status == NavMeshPathStatus.PathComplete &&
					Vector3.Distance(hitPoint, _lastHitPoint) <= PathEndHitAccuracy)
		{
			NavMesh.SamplePosition(_lastAccessablePathDot, out var hit, 0.01f, NavMesh.AllAreas);
			GoToPosition(hit, 0.01f);
			_navMeshPath.ClearCorners();
			_lastHitPoint = Vector3.positiveInfinity;
			UnaccesableLineRenderer.enabled = false;
			AccesableLineRenderer.enabled = false;
			BattleManager.RemainMovement -= Mathf.Min(BattleManager.RemainMovement, _pathLength);
			_pathLength = 0;
			return;
		}
		_controller.CalculatePath(hitPoint, _navMeshPath);
		_lastHitPoint = hitPoint;

		if (_navMeshPath.status == NavMeshPathStatus.PathComplete)
		{
			_pathLength = PathLength(_navMeshPath);
			if (_pathLength > BattleManager.RemainMovement)
			{
				UnaccesableLineRenderer.enabled = true;
				AccesableLineRenderer.enabled = true;
				_lastAccessablePathDot = CutPath(_navMeshPath, BattleManager.RemainMovement, out int lastIndex);
				var accesablePath = _navMeshPath.corners.Take(lastIndex + 1).ToArray();
				accesablePath[lastIndex] = _lastAccessablePathDot;
				AccesableLineRenderer.positionCount = accesablePath.Length;
				AccesableLineRenderer.SetPositions(accesablePath);
				var unaccesablePath = _navMeshPath.corners.TakeLast(_navMeshPath.corners.Length -
					accesablePath.Length + 2).ToArray();
				unaccesablePath[0] = _lastAccessablePathDot;
				UnaccesableLineRenderer.positionCount = unaccesablePath.Length;
				UnaccesableLineRenderer.SetPositions(unaccesablePath);
			}
			else
			{
				UnaccesableLineRenderer.enabled = false;
				AccesableLineRenderer.enabled = true;
				_lastAccessablePathDot = _navMeshPath.corners.Last();
				AccesableLineRenderer.positionCount = _navMeshPath.corners.Count();
				AccesableLineRenderer.SetPositions(_navMeshPath.corners);
			}
		}
		else
		{
			AccesableLineRenderer.enabled = false;
			UnaccesableLineRenderer.enabled = true;
			_lastAccessablePathDot = Vector3.zero;
			UnaccesableLineRenderer.positionCount = 2;
			UnaccesableLineRenderer.SetPositions(new Vector3[2] { transform.position, hitPoint });
		}
	}

	public override void SetDefaultAction()
	{
		base.SetDefaultAction();
	}

	public override void SetActiveAction(ActionType actionType)
	{
		if(_activeAction == ActionType.Movement)
		{
			AccesableLineRenderer.enabled = false;
			UnaccesableLineRenderer.enabled = false;
		}
		base.SetActiveAction(actionType);
		OnSetAction.Invoke(actionType);
		if (actionType == ActionType.Jumping)
		{
			float diametr = GlobalRules.MaxJumpDistance * 2;
			DisplaySphere(diametr, diametr);
		}
		else
		{
			HideSphere();
		}
	}
	public UnityEvent<ActionType> OnSetAction;

	private void DisplaySphere(float sphereDiametr, float shpereY)
	{
		Sphere.SetActive(true);
		Sphere.transform.localScale = new Vector3(sphereDiametr, shpereY, sphereDiametr);
	}

	private void HideSphere()
	{
		Sphere.SetActive(false);
	}

	public override void PickupItem(Item item)
	{
		base.PickupItem(item);
		GameData.Inventory.Add(item);
		item.transform.SetParent(Inventory.transform);
		item.OnTaked();
	}

	public override void DropItem(Item item)
	{
		base.DropItem(item);
		GameData.Inventory.Remove(item);
		item.transform.position = gameObject.transform.position + gameObject.transform.forward + Vector3.up;
		item.gameObject.SetActive(true);
	}

	protected override void GoToPosition(NavMeshHit hit, float maxTargetOffset = 0.1F)
	{
		base.GoToPosition(hit, maxTargetOffset);
		if (Vector3.Distance(transform.position, hit.position) > _normalAgentSpeed * 2)
		{
			_controller.speed = _normalAgentSpeed * 2;
		}
		else
		{
			_controller.speed = _normalAgentSpeed;
		}
	}

	protected override void OnJumpToPosition(Vector3 targetPosition, float jumpHeigth, float jumpDuration)
	{
		AnimatorManager.StartJumpAnim(targetPosition, jumpDuration);
	}

	protected override void OnDeath()
	{
		base.OnDeath();
		GameManager.Instance.OnDeathEvent.Invoke();
	}

	public override void EndAttack()
	{
		base.EndAttack();
		SetDefaultAction();
	}
}