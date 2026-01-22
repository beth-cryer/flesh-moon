using System;
using System.Collections.Generic;

[Serializable]
public class CharacterData
{
	public CharacterStats baseStats;
	public List<Condition> conditions;
	public List<Limb> limbs;
}

[Serializable]
public class CharacterStats
{
	public float health;
	public float sanity;
	public float hunger;
	public float hungerDecay;
	public float attack;
	public float defense;
	public float stamina;
}