using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform m_RespawnPoint;
    bool m_Activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!m_Activated && other.CompareTag("Player"))
        {
            FPSController l_Player = other.GetComponentInParent<FPSController>();
            if (l_Player != null)
            {
                l_Player.SetCheckpoint(m_RespawnPoint.position, m_RespawnPoint.rotation);
                m_Activated = true;
            }
        }
    }
}