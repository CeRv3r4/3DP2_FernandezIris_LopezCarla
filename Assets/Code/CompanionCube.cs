using UnityEngine;

public class CompanionCube : MonoBehaviour
{
    public float m_MaxValidAngle = 40.0f;
    Rigidbody m_Rigidbody;
    public float m_TeleportPortalOffset = 1.0f;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            Portal l_Portal = other .GetComponent<Portal>();
            if (CanTeleport(l_Portal))
            {
                Teleport(l_Portal);
            }
        }
    }

    private bool CanTeleport(Portal _Portal)
    {
        float l_MaxDotAngle = Mathf.Cos(m_MaxValidAngle * Mathf.Deg2Rad);
        Vector3 l_Direction = m_Rigidbody.linearVelocity.normalized;
        float l_DotAngle = Vector3.Dot(-l_Direction, _Portal.transform.forward);
        return l_DotAngle >= l_MaxDotAngle;
    }

    void Teleport(Portal _Portal)
    {
        Vector3 l_Direction = m_Rigidbody.linearVelocity.normalized;
        Vector3 l_WorldPosition = transform.position + l_Direction * m_TeleportPortalOffset;
        Vector3 l_WorldForward = transform.forward;
        Vector3 l_WorldLinearVelocity = m_Rigidbody.linearVelocity;
        Vector3 l_LocalPosition = _Portal.m_OtherPortalTransform.InverseTransformPoint(l_WorldPosition);
        Vector3 l_LocalForward = _Portal.m_OtherPortalTransform.InverseTransformDirection(l_WorldForward);
        Vector3 l_LocalLinearVelocity = _Portal.m_OtherPortalTransform.InverseTransformDirection(l_WorldLinearVelocity);

        float l_Scale = _Portal.m_MirrorPortal.transform.localScale.x/ _Portal.transform.localScale.x; // per fer el cub mes petit
        
        m_Rigidbody.isKinematic = true;
        transform.position = _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition);
        transform.forward = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalForward);
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.linearVelocity = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalLinearVelocity);
        
        transform.localScale = Vector3.one * transform.localScale.x * l_Scale;  // per fer el cub mes petit
    }
}
