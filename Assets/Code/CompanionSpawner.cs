using UnityEngine;

public class CompanionSpawner : MonoBehaviour
{
    public GameObject m_CompanionCubePrefab;
    public Transform m_SpawnPosition;

    public void Execute()
    {
        GameObject l_GameObject = GameObject.Instantiate(m_CompanionCubePrefab);
        l_GameObject.transform.position = m_SpawnPosition.position;
        l_GameObject.transform.rotation = m_SpawnPosition.rotation;
    }
}
