using UnityEngine;
using UnityEngine.UI;

public class ItemObject: MonoBehaviour
{
	protected Image m_image;

	protected virtual void Start()
	{
		m_image = GetComponent<Image>();
	}

	public void UpdateItem(Item newItemHeld)
	{
		if (newItemHeld == null)
		{
			m_image.color = new Color(0, 0, 0, 0);
			return;
		}

		m_image.color = Color.white;
		m_image.sprite = newItemHeld.m_sprite;
	}
}