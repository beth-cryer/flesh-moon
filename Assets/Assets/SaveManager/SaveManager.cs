using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager: MonoBehaviour
{
	[SerializeField] private string m_saveFileName;

	private List<SaveData> m_exampleData;

	private void Save()
	{
		SaveFile save = new();

		// Get all serializable objects
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		GameObject[] itemObjects = GameObject.FindGameObjectsWithTag("Items");

		foreach(GameObject obj in enemies)
		{
			EnemyData data = (EnemyData) SaveDefaultData(obj);
			save.enemyData.Add(data);
		}

		foreach(GameObject obj in itemObjects)
		{
			SaveData data = SaveDefaultData(obj);
			save.itemData.Add(data);
		}

		string json = JsonUtility.ToJson(save);

		using StreamWriter sr = File.CreateText(m_saveFileName);
		sr.Write(json);
	}

	private void Load()
	{
		if (!File.Exists(m_saveFileName))
			return;

		using StreamReader sr = File.OpenText(m_saveFileName);
		string json = sr.ReadToEnd();
		SaveFile save = JsonUtility.FromJson<SaveFile>(json);
	}

	private SaveData SaveDefaultData(GameObject obj)
	{
		SaveData data = new();
		Vector3 t = obj.transform.position;
		data.position = new float[] { t.x, t.y, t.z };

		return data;
	}
}

[Serializable]
public class SaveFile
{
	public List<EnemyData> enemyData;
	public List<SaveData> itemData;
}

[Serializable]
public class SaveData
{
	public int name;
	public float[] position;
}

[Serializable]
public class EnemyData: SaveData
{
	public bool isDead;
}
