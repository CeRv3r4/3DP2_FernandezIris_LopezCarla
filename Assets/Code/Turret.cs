using UnityEngine;

public class Turret : MonoBehaviour
{
    public LineRenderer m_Laser;
    public float m_MaxDistance;
    public LayerMask m_LaserLayerMask;
    public float m_AngleToFall = 30.0f;

    private void Update()
    {
        bool l_Fallen = Vector3.Dot(transform.up, Vector3.up)<Mathf.Cos(m_AngleToFall*Mathf.Deg2Rad);
        m_Laser.gameObject.SetActive(!l_Fallen);

        if (!l_Fallen)
        {
            Vector3 l_Position = Vector3.forward * m_MaxDistance;
            Ray l_Ray = new Ray(m_Laser.transform.position, m_Laser.transform.forward);

            if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxDistance, m_LaserLayerMask.value,QueryTriggerInteraction.Ignore))
            {
                l_Position = Vector3.forward * l_RaycastHit.distance;
                if (l_RaycastHit.collider.CompareTag("RefractionCube"))
                {
                    l_RaycastHit.collider.GetComponent<RefractionCube>().Refract();
                }
            }

            m_Laser.SetPosition(1, l_Position);

        }

    }
}
