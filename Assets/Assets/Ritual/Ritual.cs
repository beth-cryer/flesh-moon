using UnityEngine;
using UnityEngine.InputSystem;

public class Ritual: MonoBehaviour
{
	[SerializeField] private Camera m_characterCamera;
	[SerializeField] private Transform m_characterModelOrigin;

	[SerializeField] private Canvas m_canvas;
	[SerializeField] private RectTransform m_ritualWindowPos;

	public Transform m_heldLimb;
	private PlayerInput m_playerInput;
	private float m_canvasScale;

	private void Start()
	{
		m_playerInput = GetComponent<PlayerInput>();
		m_canvasScale = m_canvas.scaleFactor;

		m_playerInput.currentActionMap.FindAction("Click").performed += OnClick;
	}

	private void Update()
	{
		if (m_heldLimb == null) return;

		PositionLimb();
		RotateAndScaleLimb();
	}

	// set limb to mouse position
	private void PositionLimb()
	{
		m_heldLimb.position = GetMousePositionInWorld();
	}

	// rotate limb to face away from body center, and -1 xscale if to the left of center
	private void RotateAndScaleLimb()
	{
		bool isRightOfCenter = (m_heldLimb.position.x > m_characterModelOrigin.position.x);

		Vector2 direction = (Vector2)(m_heldLimb.position - m_characterModelOrigin.position).normalized;
		if (isRightOfCenter)
			m_heldLimb.transform.right = direction;
		else
			m_heldLimb.transform.right = -direction;

		float xScale = isRightOfCenter ? 1 : -1;
		m_heldLimb.localScale = new(xScale, 1f, 1f);
	}

	private Vector3 GetMousePositionInWorld()
	{
		Vector2 mousePos = Mouse.current.position.ReadValue();

		// need to convert from mouse pos to canvas pos, then get relative distance from bottom-left of the RenderTexture image
		Vector2 mousePosRelativeToWindow = (mousePos - new Vector2(m_ritualWindowPos.position.x, m_ritualWindowPos.position.y)) / m_canvasScale;
		Vector3 mousePos3d = new Vector3(mousePosRelativeToWindow.x, mousePosRelativeToWindow.y, m_characterModelOrigin.localPosition.z);

		Vector3 worldPos = m_characterCamera.ScreenToWorldPoint(mousePos3d);
		return worldPos;
	}

	private void OnClick(InputAction.CallbackContext ctx)
	{
		Debug.Log("click function started");

		Vector3 mouseWorldPos = GetMousePositionInWorld();
		Vector3 camPos = m_characterCamera.transform.position;

		RaycastHit hit;
		string layerName = (m_heldLimb == null) ? "Limb" : "LimbSocket";
		bool didHit = Physics.Raycast(camPos, (mouseWorldPos - camPos).normalized, out hit, 10.0f, LayerMask.GetMask(layerName), QueryTriggerInteraction.Collide);

		// pick up limb
		if (m_heldLimb == null)
		{
			if (didHit)
				m_heldLimb = hit.transform;
			return;
		}

		// drop limb
		// found socket, place currently held limb in socket before dropping
		if (didHit)
			m_heldLimb.position = hit.transform.position;
		m_heldLimb = null;

		Debug.Log("click function finishd");
	}


}