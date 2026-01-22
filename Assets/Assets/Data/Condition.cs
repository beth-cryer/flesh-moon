using System;

public enum Condition
{
	None = 0,
	Infected,
	Fleshrot,
	Poison,
	Burn,
	Bleed,
	Stun,
	Paralysis,
	COUNT,
}

[Serializable]
public class Resistance
{
	public DamageType resistanceDamageType;
	public Condition resistanceCondition;
	public float resistanceAmount;
}