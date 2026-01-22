using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HeldItem : ItemUI
{
	private RectTransform m_rect;
	private Canvas m_canvas;
	private RectTransform m_canvasRect;

	protected override void Start()
	{
		Inventory.m_instance.m_onItemHeld += UpdateItem;

		m_rect = GetComponent<RectTransform>();
		m_canvas = GetComponentInParent<Canvas>();
		m_canvasRect = m_canvas.GetComponent<RectTransform>();
	}

	private void Update()
    {
		m_rect.anchoredPosition = Mouse.current.position.ReadValue() / m_canvas.scaleFactor;
	}

	
}
