using UnityEngine;
using UnityEngine.InputSystem;

public class Ritual: MonoBehaviour
{
	[SerializeField] private Camera m_characterCamera;
	[SerializeField] private Transform m_characterModelOrigin;

	[SerializeField] private Canvas m_canvas;
	[SerializeField] private RectTransform m_ritualWindowPos;

	public Transform m_heldLimb;
	private float m_canvasScale;

	private void Start()
	{
		m_canvasScale = m_canvas.scaleFactor;
	}

	private void Update()
	{
		PositionLimb();
		RotateAndScaleLimb();
	}

	// set limb to mouse position
	private void PositionLimb()
	{
		Vector2 mousePos = Mouse.current.position.ReadValue();

		// need to convert from mouse pos to canvas pos, then get relative distance from bottom-left of the RenderTexture image
		Vector2 mousePosRelativeToWindow = (mousePos - new Vector2(m_ritualWindowPos.position.x, m_ritualWindowPos.position.y)) / m_canvasScale;
		Vector3 mousePos3d = new Vector3(mousePosRelativeToWindow.x, mousePosRelativeToWindow.y, m_characterModelOrigin.localPosition.z);

		Vector3 worldPos = m_characterCamera.ScreenToWorldPoint(mousePos3d);
		m_heldLimb.position = worldPos;
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
}