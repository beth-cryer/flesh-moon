using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[Serializable]
public class Item
{
	public Sprite m_sprite;
}

public class Inventory : MonoBehaviour
{
	public static Inventory m_instance;

	public UnityAction<Item> m_onItemHeld;

	private List<Item> m_items;
	private Item m_heldItem = null;

	[SerializeField] private int m_inventorySize = 20;
	[SerializeField] private int m_rowLength = 5;

	[SerializeField] private Transform m_gridParent;
	[SerializeField] private GameObject m_itemSlotPrefab;

	// possible states:
	// item is currently held / held item is null
	// slot being clicked contains items / slot is null
	// if both are full, swap
	public Item ClickItemSlot(int index)
	{
		Item currentHeldItem = m_heldItem; // item buffer, holds current held item

		// if slot was filled, pick up item previously in slot
		m_heldItem = m_items[index];
		m_onItemHeld.Invoke(m_heldItem);

		// if item held, place in slot
		m_items[index] = currentHeldItem;
		m_gridParent.GetChild(index).GetComponent<Image>().sprite = currentHeldItem.m_sprite;

		return m_heldItem;
	}

	[Button]
	private void SetupItemSlots()
	{
		int childCount = 0;
		foreach(Transform child in m_gridParent)
			childCount++;

		if (childCount > m_inventorySize)
		{
			// delete excess slots
			for (int i = 0; i < (childCount - m_inventorySize); i++)
				DestroyImmediate(m_gridParent.GetChild(childCount - 1 - i).gameObject);

			return;
		}

		// ELSE fill missing slots
		for (int i = childCount; i < m_inventorySize; i++)
			Instantiate(m_itemSlotPrefab, m_gridParent);
	}


    private void Start()
    {
		SetupItemSlots();
	}

	private void Update()
    {
        
    }

	private void Awake()
	{
		if (m_instance == null)
			m_instance = this;
		else
			Destroy(this);
	}
}
