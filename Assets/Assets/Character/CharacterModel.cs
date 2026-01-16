using UnityEngine;

public class CharacterModel : MonoBehaviour
{
	private Vector3 m_lastMoveDirection;
	private Vector3 m_lastSurfaceNormal;
	private RaycastHit[] m_raycastHits = new RaycastHit[10];

	private void Start()
	{
		m_lastMoveDirection = transform.forward;
		m_lastSurfaceNormal = Vector3.up;
	}

	// Sets Y rotation to face desired direction
	// Sets X and Z rotation to align to the surface being stood on
	public void SetRotation(bool isGrounded, Vector3 moveDirection)
	{
		Vector3 playerUp = m_lastSurfaceNormal;
		if (isGrounded)
		{
			int results = Physics.SphereCastNonAlloc(transform.position, 0.5f, Vector3.down, m_raycastHits, 1.0f, LayerMask.GetMask("Terrain"));
			if (results > 0)
			{
				Vector3 average = Vector3.zero;
				for (int i = 0; i < results; i++)
				{
					average += m_raycastHits[i].normal;
				}
				playerUp = average / results;
				m_lastSurfaceNormal = playerUp;
			}
		}

		transform.rotation = Quaternion.LookRotation(moveDirection, playerUp);
		m_lastMoveDirection = moveDirection;
	}

	public void SetRotation(bool isGrounded)
	{
		SetRotation(isGrounded, m_lastMoveDirection);
	}
}