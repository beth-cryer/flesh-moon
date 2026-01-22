using UnityEngine.UI;

public class ItemSlot: ItemUI
{
	public int id;

	protected override void Start()
	{
		base.Start();
		GetComponent<Button>().onClick.AddListener(OnClickSlot);
	}

	public void OnClickSlot()
	{
		Inventory.m_instance.ClickItemSlot(id);
		UpdateItem(Inventory.m_instance.m_items[id]);
	}
}