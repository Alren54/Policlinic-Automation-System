using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class RendezvousButtonControl : MonoBehaviour
{
    [SerializeField] private ButtonManager buttonManager;
    public void RendezvousButton(int element)
    {
        buttonManager.RendezvousPrefab(element, transform.parent.gameObject);
    }
}
