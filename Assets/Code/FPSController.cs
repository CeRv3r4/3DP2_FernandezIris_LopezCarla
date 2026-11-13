using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    bool m_AngleLocked = false;

    public Transform m_PitchController;

    bool m_AttachingObject = false;
    bool m_ObjectAttached = false;
    Rigidbody m_GrabObject;
    float m_GrabTime;


    float m_Yaw;
    float m_Pitch;
    Vector3 m_Direction;
    public float m_Yawspeed;
    public float m_Pitchspeed;
    public bool m_InvertPitch;
    public bool m_InvertYaw;
    public float m_PitchMin;
    public float m_PitchMax;

    CharacterController m_CharacterController;

    public float m_Speed;
    public float m_VerticalSpeed = 0.0f;
    public float m_JumpSpeed = 5.0f;
    public float m_DashSpeed = 10.0f;

    [Header("Input")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_DashKeyCode = KeyCode.LeftShift;
    public KeyCode m_DebugLockAngleCode = KeyCode.I;
    public KeyCode m_DebugLockCursorCode = KeyCode.O;
    public KeyCode m_ExitKeyCode = KeyCode.Escape;
    public KeyCode m_RestartKeyCode = KeyCode.F;
    public KeyCode m_grabObjectkeyCode = KeyCode.E;

    [Header("Shooting")]
    public Camera m_Camera;
    public int m_ShootButton = 0;
    public float m_MaxShootDistance = 200f;
    public LayerMask m_ShootLayerMask;


    public Vector3 m_StartPos;
    public Quaternion m_StartRot;

    public GameObject m_Deathpanel;
    public bool m_isDead = false;

    public bool m_hasKey = false;

    [Header("Portals")]
    public Portal m_BluePortal;
    public Portal m_OrangePortal;
    public float TeleportPortalOffset = 1.0f;
    public float m_MaxValidAngle = 40.0f;

    [Header("Grab")]
    public Transform m_GrabTransform;
    public float m_TimeToGrab = 0.8f;
    public float m_ShootAttachedObjectForce = 20.0f;


    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }


    private void Start()
    {
        m_StartPos = transform.position;
        m_StartRot = transform.rotation;
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = m_PitchController.eulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;
        //GameController.GetGameController().Setplayer(this);
    }

    private void Update()
    {
        if (m_isDead && Input.GetKeyDown(m_RestartKeyCode))
        {
            Restart();
        }
        if (m_isDead && Input.GetKeyDown(m_ExitKeyCode))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(m_DebugLockAngleCode))
        {
            m_AngleLocked = !m_AngleLocked;
        }
        if (Input.GetKeyDown(m_DebugLockCursorCode))
        {
            Cursor.lockState = (Cursor.lockState != CursorLockMode.Locked) ? CursorLockMode.Locked : CursorLockMode.None;
        }

        float l_MouseX = Input.GetAxis("Mouse X");
        float l_MouseY = Input.GetAxis("Mouse Y");

        if (m_InvertYaw)
        {
            l_MouseX = -l_MouseX;
        }
        if (m_InvertPitch)
        {
            l_MouseY = -l_MouseY;
        }

        if (!m_AngleLocked)
        {
            m_Yaw = m_Yaw + l_MouseX * m_Yawspeed * Time.deltaTime;
            m_Pitch = m_Pitch + l_MouseY * m_Pitchspeed * Time.deltaTime;
        }

        m_Pitch = Mathf.Clamp(m_Pitch, m_PitchMin, m_PitchMax);

        transform.rotation = Quaternion.Euler(0, m_Yaw, 0);
        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0, 0);

        m_Direction = Vector3.zero;
        Vector3 l_Right = transform.right;
        Vector3 l_Forward = transform.forward;
        l_Right.y = 0.0f;
        l_Forward.y = 0.0f;
        l_Right.Normalize();
        l_Forward.Normalize();


        if (Input.GetKey(m_RightKeyCode) && !m_isDead)
        {
            m_Direction = l_Right;
        }
        else if (Input.GetKey(m_LeftKeyCode) && !m_isDead)
        {
            m_Direction = -l_Right;
        }

        if (Input.GetKey(m_UpKeyCode) && !m_isDead)
        {
            m_Direction += l_Forward;
        }
        if (Input.GetKey(m_DownKeyCode) && !m_isDead)
        {
            m_Direction -= l_Forward;
        }

        float l_Speed = m_Speed;
        if (Input.GetKey(m_DashKeyCode) && !m_isDead)
        {
            l_Speed = m_DashSpeed;
        }
        if (Input.GetKeyDown(m_JumpKeyCode) && (m_VerticalSpeed < 0.2f && m_VerticalSpeed > -0.2f) && !m_isDead)
        {
            m_VerticalSpeed = m_JumpSpeed;
        }

        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;

        m_Direction.Normalize();
        Vector3 l_Direction = m_Direction * l_Speed * Time.deltaTime;
        l_Direction.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Direction);

        if ((l_CollisionFlags & CollisionFlags.CollidedBelow) != 0)
        {
            m_VerticalSpeed = 0.0f;
        }
        else if ((l_CollisionFlags & CollisionFlags.CollidedAbove) != 0 && m_VerticalSpeed > 0.0f)
        {
            m_VerticalSpeed = 0.0f;
        }

        //if (GameController.GetGameController().GetIngameUI() != null)
        //{
        //    UpdateLifeBarEnemies();
        //}

        if (CanShoot() && MustShoot())
        {
            Shoot(m_BluePortal);
        }

        if (CanGrabObject())
        {
            GrabObject();
        }
        UpdateAttachingObject();
        UpdateAttachedObject();
    }

    bool CanGrabObject()
    {
        if (Input.GetKeyDown(m_grabObjectkeyCode) && !m_AttachingObject && !m_ObjectAttached)
        { return true; }
        return false;
    }

    void GrabObject()
    {
        Ray l_Rray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(l_Rray, out RaycastHit l_RaycastHit, m_MaxShootDistance, m_ShootLayerMask.value))
        {
            if (l_RaycastHit.collider.CompareTag("Cube"))
            {
                GrabObject(l_RaycastHit.collider);
            }
        }
    }

    void UpdateAttachingObject()
    {
        if (m_AttachingObject)
        {
            float l_DiffTime = Time.time - m_GrabTime;
            float l_Pct = Mathf.Min(1.0f, l_DiffTime / m_TimeToGrab);
            m_GrabObject.transform.position = Vector3.Lerp(m_GrabObject.transform.position, m_GrabTransform.position, l_Pct);
            m_GrabObject.transform.rotation = Quaternion.Lerp(m_GrabObject.transform.rotation, m_GrabTransform.rotation, l_Pct);
            if (l_Pct == 1.0f)
            {
                m_AttachingObject = false;
                m_ObjectAttached = true;
            }
        }
    }

    void UpdateAttachedObject()
    {
        if (m_ObjectAttached)
        {
            if (Input.GetKeyDown(m_grabObjectkeyCode))
            {
                ThrowAttachedObject(0.0f);
            }
            else if (Input.GetMouseButtonDown(m_ShootButton))
            {
                ThrowAttachedObject(m_ShootAttachedObjectForce);
            }
        }
    }

    void ThrowAttachedObject(float _Force)
    {
        m_GrabObject.isKinematic = false;
        m_GrabObject.linearVelocity = m_PitchController.forward * _Force;
        m_GrabObject.transform.SetParent(null);
        m_ObjectAttached = false;
    }

    void GrabObject(Collider _Collider)
    {
        m_GrabObject = _Collider.GetComponent<Rigidbody>();
        m_GrabObject.isKinematic = true;
        m_AttachingObject = true;
        m_GrabObject.transform.SetParent(m_GrabTransform);
        m_GrabTime = Time.time;
    }

    bool CanShoot()
    {
        return true;
    }

    bool MustShoot()
    {
        return Input.GetMouseButtonDown(m_ShootButton);
    }
    void Shoot(Portal _Portal)
    {

        Ray ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit l_RaycastHit, m_MaxShootDistance, m_ShootLayerMask.value))
        {
            if (_Portal.IsValidPosition(l_RaycastHit.point, l_RaycastHit.normal))
            {
                _Portal.transform.position = l_RaycastHit.point + l_RaycastHit.normal * 0.05f;  // per a que no traspasi el mirror dins la paret
                _Portal.transform.rotation = Quaternion.LookRotation(l_RaycastHit.normal);

                _Portal.gameObject.SetActive(true);

            }
            else
            {
                _Portal.gameObject.SetActive(false);

            }

            //if (hit.collider.CompareTag("HitCollider"))
            //{
            //    hit.collider.GetComponent<HitCollider>().Hit();
            //}
            //else
            //{
            //    if (m_ShootHitParticlesPrefab != null)
            //    {
            //        CreateShoothitparticles(hit.point, hit.normal);
            //    }
            //}
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            Kill();
        }
        else if (other.CompareTag("Portal"))
        {
            Portal l_Portal = other.GetComponent<Portal>();
            if (CanTeleport(l_Portal))
            { Teleport(l_Portal); }

        }
        else if (other.CompareTag("CompanionSpawner"))
        {
            other.GetComponent<CompanionSpawner>().Execute();
        }
    }
    private bool CanTeleport(Portal _Portal)
    {
        float l_MaxDotAngle = Mathf.Cos(m_MaxValidAngle * Mathf.Deg2Rad);
        float l_DotAngle = Vector3.Dot(-m_Direction, _Portal.transform.forward);
        return l_DotAngle >= l_MaxDotAngle;
    }

    public void Kill()
    {
        m_Deathpanel.SetActive(true);
        m_isDead = true;
    }

    public void Restart()
    {
        m_CharacterController.enabled = false;
        transform.position = m_StartPos;
        transform.rotation = m_StartRot;
        m_CharacterController.enabled = true;
        m_isDead = false;
        m_Deathpanel.SetActive(false);
    }

    void Teleport(Portal _Portal)
    {
        Vector3 l_WorldPosition = transform.position + m_Direction * TeleportPortalOffset;
        Vector3 l_WorldForward = transform.forward;
        Vector3 l_LocalPosition = _Portal.m_OtherPortalTransform.InverseTransformPoint(l_WorldPosition);
        Vector3 l_LocalForward = _Portal.m_OtherPortalTransform.InverseTransformDirection(l_WorldForward);

        m_CharacterController.enabled = false;
        transform.position = _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition);
        transform.forward = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalForward);
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Direction = Vector3.zero;
        m_CharacterController.enabled = true;
    }
}