using System;
using System.Collections.Generic;

[Serializable]
public class Limb
{
	public string limbSocketID;
	public float health;
	public List<Condition> conditions;
	public CharacterStats statModifiers;

	public List<float> abilities; // placeholder
}

