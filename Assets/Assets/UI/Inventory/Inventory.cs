using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
	public static Inventory m_instance;

	public UnityAction<Item> m_onItemHeld;

	public List<Item> m_items;
	private Item m_heldItem = null;

	[SerializeField] private int m_inventorySize = 20;
	[SerializeField] private int m_rowLength = 5;

	[SerializeField] private Transform m_gridParent;
	[SerializeField] private GameObject m_itemSlotPrefab;

	[SerializeField] private Sprite m_testItemSprite;
	[SerializeField] private GameObject m_testItemPrefab;

	private void Start()
	{
		m_items = new List<Item>();
		SetInventorySize(m_inventorySize);
		SetupItemSlots();

		// add test item
		Item testItem = new LimbItem() { m_isItemInSlot = true, m_name = "Test Limb", m_sprite = m_testItemSprite, m_limbPrefab = m_testItemPrefab };
		AddItem(testItem);

		m_onItemHeld.Invoke(null);
	}

	private void Update()
	{

	}

	// possible states:
	// item is currently held / held item is null
	// slot being clicked contains items / slot is null
	// if both are full, swap
	public void ClickItemSlot(int index)
	{
		Debug.Log("Clicked on inventory slot=" + index);
		Item currentHeldItem = m_heldItem; // item buffer, holds current held item

		// if slot was filled, pick up item previously in slot
		Debug.Log("Clicked slot with item = " + (m_items[index].m_isItemInSlot ? "none" : m_items[index].m_name));

		if (m_items[index].m_isItemInSlot)
			m_heldItem = m_items[index];
		else
			m_heldItem = null;
		
		m_onItemHeld.Invoke(m_heldItem);

		// if item held, place in slot
		Debug.Log("Placed held item = " + (currentHeldItem == null ? "none" : currentHeldItem.m_name));
		if (currentHeldItem == null)
			m_items[index] = new();
		else
			m_items[index] = currentHeldItem;
	}

	[Button]
	private void SetupItemSlots()
	{
		int childCount = 0;
		foreach(Transform child in m_gridParent)
		{
			//child.GetComponent<Button>().onClick.RemoveAllListeners();
			//child.GetComponent<Button>().onClick.AddListener(() => ClickItemSlot(childCount));
			childCount++;
		}

		if (childCount > m_inventorySize)
		{
			// delete excess slots
			for (int i = 0; i < (childCount - m_inventorySize); i++)
				DestroyImmediate(m_gridParent.GetChild(childCount - 1 - i).gameObject);

			return;
		}

		// ELSE fill missing slots
		for (int i = childCount; i < m_inventorySize; i++)
		{
			GameObject newSlot = Instantiate(m_itemSlotPrefab, m_gridParent);
			newSlot.GetComponent<ItemUI>().UpdateItem(null);
			newSlot.GetComponent<ItemSlot>().id = i;
		}
	}

	private void SetInventorySize(int newInventorySize)
	{
		m_inventorySize = newInventorySize;
		int inventorySizeDifference = m_items.Count - m_inventorySize; // owo

		// add empty slots to list equal to difference
		for (int i = inventorySizeDifference; i < 0; i++)
		{
			m_items.Add(new());
		}

		// remove slots from list equal to difference
		for (int i = inventorySizeDifference; i > 0; i--)
		{
			m_items.RemoveAt(m_items.Count - 1);
		}

	}

	// Returns true if item slot found, and false if not
	public bool AddItem(Item item)
	{
		for (int i = 0; i < m_inventorySize; i++)
		{
			if (m_items[i].m_isItemInSlot) continue;

			SetItemSlot(i, item);
			return true;
		}
		return false;
	}

	// Sets item slot, will overwrite 
	public void SetItemSlot(int index, Item item)
	{
		Debug.Log("Set item at index=" + index);
		m_items[index] = item;
		m_gridParent.GetChild(index).GetComponent<ItemUI>().UpdateItem(item);
	}

	private void Awake()
	{
		if (m_instance == null)
			m_instance = this;
		else
			Destroy(this);
	}
}
