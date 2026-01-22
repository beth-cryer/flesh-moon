using UnityEngine;
using UnityEngine.UI;

public class ItemUI: MonoBehaviour
{
	[SerializeField] protected Image m_image;

	protected virtual void Start()
	{
	}

	public void UpdateItem(Item newItemHeld)
	{
		if (newItemHeld == null || !newItemHeld.m_isItemInSlot)
		{
			m_image.color = new Color(0, 0, 0, 0);
			return;
		}

		m_image.color = Color.white;
		m_image.sprite = newItemHeld.m_sprite;
	}
}