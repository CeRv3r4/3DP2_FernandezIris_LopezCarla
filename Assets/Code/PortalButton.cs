using UnityEngine;
using UnityEngine.Events;

public class PortalButton : MonoBehaviour
{
    public string m_InteractorTag = "Player";
    public UnityEvent m_Event;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(m_InteractorTag))
        {
            m_Event.Invoke();
        }
    }
}
