using UnityEngine;
using UnityEngine.InputSystem;

public class Ritual: MonoBehaviour
{
	public static Ritual m_instance;

	[SerializeField] private Camera m_characterCamera;
	[SerializeField] private Transform m_characterModelOrigin;

	[SerializeField] private Canvas m_canvas;
	[SerializeField] private RectTransform m_ritualWindowPos;

	public Transform m_heldLimb;
	private PlayerInput m_playerInput;
	private float m_canvasScale;

	private void Start()
	{
		m_playerInput = GameStateManager.m_instance.m_playerInput;
		m_canvasScale = m_canvas.scaleFactor;

		m_playerInput.actions.FindAction("Click").performed += OnClick;
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
		Vector2 mousePos = Mouse.current.position.ReadValue();
		m_heldLimb.position = GetMousePositionInWorld(mousePos);
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

	private Vector3 GetMousePositionInWorld(Vector2 mousePos)
	{
		// need to convert from mouse pos to canvas pos, then get relative distance from bottom-left of the RenderTexture image
		Vector2 mousePosRelativeToWindow = (mousePos - new Vector2(m_ritualWindowPos.position.x, m_ritualWindowPos.position.y)) / m_canvasScale;
		Vector3 mousePos3d = new Vector3(mousePosRelativeToWindow.x, mousePosRelativeToWindow.y, m_characterModelOrigin.localPosition.z - 1.0f);

		Vector3 worldPos = m_characterCamera.ScreenToWorldPoint(mousePos3d);
		return worldPos;
	}

	private void OnClick(InputAction.CallbackContext ctx)
	{
		Debug.Log("click function started");

		Vector2 mousePos = Mouse.current.position.ReadValue();
		Vector2 ritualPos = m_ritualWindowPos.transform.position;
		float ritualWidth = m_ritualWindowPos.rect.width;
		float ritualHeight = m_ritualWindowPos.rect.height;

		Debug.Log("ritual width = " + ritualWidth + ", height = " + ritualHeight);

		if (mousePos.x > ritualPos.x && mousePos.y > ritualPos.y
		&& mousePos.x < (ritualPos.x + ritualWidth) && mousePos.y < (ritualPos.y + ritualHeight))
			return;

		Debug.Log("clicked inside ritual window");

		Vector3 mouseWorldPos = GetMousePositionInWorld(mousePos);
		Vector3 camPos = m_characterCamera.transform.position;

		RaycastHit hit;
		string layerName = (m_heldLimb == null) ? "Limb" : "LimbSocket";
		bool didHit = Physics.Raycast(camPos, (mouseWorldPos - camPos).normalized, out hit, 10.0f, LayerMask.GetMask(layerName), QueryTriggerInteraction.Collide);

		// pick up limb
		if (m_heldLimb == null)
		{
			if (didHit) m_heldLimb = hit.transform;
			return;
		}

		// drop limb
		// found socket, place currently held limb in socket before dropping
		if (didHit)
			m_heldLimb.position = hit.transform.position;
		m_heldLimb = null;

		Debug.Log("click function finishd");
	}

	private void Awake()
	{
		if (m_instance == null)
			m_instance = this;
		else
			Destroy(this);
	}
}