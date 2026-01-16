using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

	[SerializeField] private Sprite m_testItemSprite;
	[SerializeField] private GameObject m_testItemPrefab;

	private void Start()
	{
		m_items = new List<Item>(m_inventorySize);
		SetupItemSlots();


		// add test item
		Item testItem = new LimbItem() { m_sprite = m_testItemSprite, m_limbPrefab = m_testItemPrefab };
		AddItem(testItem);
	}

	private void Update()
	{

	}

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

	private void SetInventorySize(int newInventorySize)
	{
		int inventorySizeDifference = m_items.Capacity - newInventorySize; // owo
	}

	// Returns true if item slot found, and false if not
	public bool AddItem(Item item)
	{
		for (int i = 0; i < m_inventorySize; i++)
		{
			if (m_items[i] != null) continue;

			SetItemSlot(i, item);
			return true;
		}
		return false;
	}

	// Sets item slot, will overwrite 
	public void SetItemSlot(int index, Item item)
	{
		m_items[index] = item;
		m_gridParent.GetChild(index).GetComponent<Image>().sprite = item.m_sprite;
	}

	private void Awake()
	{
		if (m_instance == null)
			m_instance = this;
		else
			Destroy(this);
	}
}
