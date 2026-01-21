using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public struct Attack
{
	public bool isStarted;
	public Vector2 startPos;
	public Vector2 endPos;
}

[Serializable]
public class LimbState
{
	public string name;
	public float health;
	public int samplePointHits;
	public int lastSamplePointHits;
}

public class BattleManager : MonoBehaviour
{
	private const int m_attackSamplePoints = 20;
	[SerializeField] private float m_attackMaxDamage = 10.0f;
	[SerializeField] private float m_attackMinDamage = 1.0f;
	[SerializeField] private float m_attackMinLength = 10.0f;
	[SerializeField] private float m_attackMaxLength = 100.0f;
	[SerializeField] private List<LimbState> m_enemyLimbs;

	[SerializeField] private Camera m_camera;
	[SerializeField] private Transform m_enemyPosition;
	[SerializeField] private LineRenderer m_lineRenderer;

	private PlayerInput m_playerInput;
	private BattleState m_battleState;
	private Attack m_attackState;
	private float m_enemyDistance;

	private List<Vector3> m_debugDrawHitPositions = new();
	
	public enum BattleState
	{
		Choosing,
		Attacking,
		Defending,
		Item,
		EnemyAction,
		COUNT,
	}

	private void Start()
	{
		m_playerInput = GameStateManager.m_instance.m_playerInput;

		m_attackState = new();
		m_enemyDistance = Vector3.Distance(m_camera.transform.position, m_enemyPosition.position);
		m_debugDrawHitPositions = new();

		Debug.Log("Distance = " + m_enemyDistance);
	}

	private void Update()
	{
		m_playerInput.actions.FindAction("Click").performed += OnClick;
		m_playerInput.actions.FindAction("RightClick").performed += OnRightClick;

		if (!m_attackState.isStarted)
			return;

		DrawLine(m_attackState.startPos, Input.mousePosition);
	}

	private void OnDrawGizmos()
	{
		foreach(Vector3 point in m_debugDrawHitPositions)
		{
			Gizmos.DrawSphere(point, 0.025f);
		}
	}

	private void OnClick(InputAction.CallbackContext ctx)
	{
		Debug.Log("click function started");

		if (!m_attackState.isStarted)
		{
			m_attackState.isStarted = true;
			m_attackState.startPos = Input.mousePosition;
			Debug.Log(m_attackState.startPos);
			return;
		}

		m_attackState.endPos = Input.mousePosition;
		Debug.Log(m_attackState.endPos);

		m_debugDrawHitPositions = new();

		//execute attack
		Vector3 start_worldPos = MouseToWorldSpace(m_attackState.startPos);
		Vector3 end_worldPos = MouseToWorldSpace(m_attackState.endPos);

		SetLinePositions(start_worldPos, end_worldPos);

		// check for collision at x sample points along the line
		// total damage is inversely proportional to the line length, and then individual limb damage is calculated by num of sample points / total points collide with each limb
		float distance = Vector2.Distance(start_worldPos, end_worldPos);
		float distancePerPoint = distance / m_attackSamplePoints;

		for(int i = 0; i < m_attackSamplePoints; i++)
		{
			Vector3 samplePosition = Vector3.Lerp(start_worldPos, end_worldPos, (i*distancePerPoint) / distance);
			RaycastHit[] hits = Physics.RaycastAll(samplePosition, m_camera.transform.forward, 5.0f, LayerMask.GetMask("LimbBattle"));

			foreach(RaycastHit hit in hits)
			{
				Debug.Log("limb hit");
				LimbState limb = m_enemyLimbs.FirstOrDefault(x => x.name.Equals(hit.collider.name));

				if (limb == null)
					continue;

				limb.samplePointHits++;
				m_debugDrawHitPositions.Add(hit.point);
			}
		}

		//Calculate damage for each limb
		foreach (LimbState limb in m_enemyLimbs)
		{
			if (limb.samplePointHits == 0)
				continue;

			float damage = Mathf.Lerp(m_attackMinDamage, m_attackMaxDamage, (float)limb.samplePointHits / (float)m_attackSamplePoints);
			limb.health -= damage;

			limb.lastSamplePointHits = limb.samplePointHits;
			limb.samplePointHits = 0; // reset hits tracked (for next attack)
		}

		EndAttack();
	}

	private void OnRightClick(InputAction.CallbackContext ctx)
	{
		EndAttack();
	}

	private void EndAttack()
	{
		m_attackState = new();
		SetLinePositions(Vector3.zero, Vector3.zero);
	}

	private void DrawLine(Vector2 startPos, Vector2 endPos)
	{
		Vector3 start_worldPos = MouseToWorldSpace(startPos);
		Vector3 end_worldPos = MouseToWorldSpace(endPos);

		SetLinePositions(start_worldPos, end_worldPos);
	}

	private void SetLinePositions(Vector3 lineStart, Vector3 lineEnd)
	{
		Vector3[] positions = new Vector3[2];
		positions[0] = lineStart;
		positions[1] = lineEnd;
		m_lineRenderer.SetPositions(positions);
	}

	private Vector3 MouseToWorldSpace(Vector2 mousePos)
	{
		Vector3 mousePos3d = new Vector3(mousePos.x, mousePos.y, m_enemyDistance - 1.0f);
		return m_camera.ScreenToWorldPoint(mousePos3d);
	}

}