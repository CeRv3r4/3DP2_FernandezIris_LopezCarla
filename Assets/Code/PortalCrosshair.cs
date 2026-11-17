using UnityEngine;
using UnityEngine.UI;

public class PortalCrosshair : MonoBehaviour
{
    public FPSController m_Player;
    public Image m_HUDImage;

    [Header("Portal HUD Images")]
    public Sprite m_NoPortal;
    public Sprite m_PortalBlue;
    public Sprite m_PortalOrange;
    public Sprite m_PortalBoth;

    private void Update()
    {
        bool l_Blue = m_Player.m_BluePortal.gameObject.activeSelf;
        bool l_Orange = m_Player.m_OrangePortal.gameObject.activeSelf;

        if (l_Blue && l_Orange)
        {
            ChangeHUD("PortalBoth");
        }
        else if (l_Blue)
        {
            ChangeHUD("PortalBlue");
        }
        else if (l_Orange)
        {
            ChangeHUD("PortalOrange");
        }
        else
        {
            ChangeHUD("NoPortal");
        }
    }

    public void ChangeHUD(string State)
    {
        switch (State)
        {
            case "NoPortal":
                m_HUDImage.sprite = m_NoPortal;
                break;
            case "PortalBlue":
                m_HUDImage.sprite = m_PortalBlue;
                break;
            case "PortalOrange":
                m_HUDImage.sprite = m_PortalOrange;
                break;
            case "PortalBoth":
                m_HUDImage.sprite = m_PortalBoth;
                break;
        }
    }
}