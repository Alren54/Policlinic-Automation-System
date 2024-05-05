using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class RendezvousButtonControl : MonoBehaviour
{
    [HideInInspector] public ButtonManager buttonManager;
    [HideInInspector] public int buttonID;
    public void RendezvousButton(int element)
    {
        buttonManager.RendezvousPrefab(element, buttonID, transform.parent.gameObject);
    }
}
