using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public FPSController m_Player;
    public Camera m_Camera;
    public Transform m_OtherPortalTransform;
    public Portal m_MirrorPortal;
    public float m_CameraOffset = 0.5f;
    public List<Transform> m_ValidPositions;
    public float m_RayCastValidationOffsetDistance = 0.1f;
    public float m_MaxAngleValidation = 5.0f;

    private void Update()
    {
        Camera l_Camera = m_Player.m_Camera;
        
        Vector3 l_CameraWorldPosition = l_Camera.transform.position;
        Vector3 l_DirectionWorldPosition = l_Camera.transform.forward;
        Vector3 l_CameraLocalPosition = m_MirrorPortal.transform.InverseTransformPoint(l_CameraWorldPosition);
        Vector3 l_DirectionLocalPosition = m_MirrorPortal.transform.InverseTransformDirection(l_DirectionWorldPosition);
       
        m_Camera.transform.position = m_OtherPortalTransform.TransformPoint(l_CameraLocalPosition);
        m_Camera.transform.forward = m_OtherPortalTransform.TransformDirection(l_DirectionLocalPosition);

        float l_Distance = Vector3.Distance(m_Camera.transform.position, transform.position);
        m_Camera.nearClipPlane = l_Distance + m_CameraOffset;
    }
    public bool IsValidPosition(Vector3 Position, Vector3 Normal)
    {
        Camera l_Camera = m_Player.m_Camera;
        transform.position = Position;
        transform.rotation = Quaternion.LookRotation(Normal);
        float l_MaxValidDotAngle = Mathf.Cos(m_MaxAngleValidation * Mathf.Deg2Rad);
        for (int i = 0; i < m_ValidPositions.Count; ++i)
        {
            Vector3 l_Direction = m_ValidPositions[i].position -l_Camera.transform.position;
            float l_Distance = l_Direction.magnitude;
            l_Direction.Normalize();
            Ray l_Ray = new Ray(l_Camera.transform.position, l_Direction);
            if (Physics.Raycast(l_Ray,out RaycastHit l_HitInfo, l_Distance, m_Player.m_ShootLayerMask.value, QueryTriggerInteraction.Ignore))
            {
                if (l_HitInfo.collider.CompareTag("PortalEnabled"))
                {
                    float l_DistanceToHitPoint= Vector3.Distance(l_HitInfo.point, m_ValidPositions[i].position);
                    if (l_DistanceToHitPoint < m_RayCastValidationOffsetDistance)
                    {
                        float l_DotAngle = Vector3.Dot(Normal, l_HitInfo.normal);
                        if (l_DotAngle < l_MaxValidDotAngle)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}
