using UnityEngine;

public class Dismemberment : MonoBehaviour
{
	[SerializeField] private Transform m_testDismembermentBone;
	[SerializeField] private GameObject m_testLimbPrefab;

    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
		{
			m_testDismembermentBone.localScale = Vector3.zero;

			GameObject spawnedLimb = Instantiate(m_testLimbPrefab);
			spawnedLimb.transform.position = m_testDismembermentBone.position;
		}
    }
}
