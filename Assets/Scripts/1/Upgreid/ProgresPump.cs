using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgresPump : MonoBehaviour
{
    public GameObject Stantion1botum;

    private void Start()
    {
        // Initialization if needed
    }

    void OnMouseUpAsButton()
    {
        // Toggle the active state of Stantion1botum
        Stantion1botum.SetActive(!Stantion1botum.activeSelf);
    }
}
