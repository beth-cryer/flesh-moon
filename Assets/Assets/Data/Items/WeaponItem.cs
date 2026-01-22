using System;

[Serializable]
public class WeaponItem : Item
{
	public float damage;
	public DamageType damageType;
}

[Serializable]
public class AttackConditionData
{
	public Condition condition;
	public bool affectsHeart;
	public float percent;
}

public enum DamageType
{
	None = 0,
	Slashing,
	Bludgeoning,
	Piercing,
	COUNT,
}