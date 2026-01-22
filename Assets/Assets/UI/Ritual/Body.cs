using System;
using System.Collections.Generic;
using UnityEngine;

public class BodyParts
{
	public Transform socket;
	public Transform part = null;
}

public class Body: MonoBehaviour
{
	public List<BodyParts> m_parts;
}