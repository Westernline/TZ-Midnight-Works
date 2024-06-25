using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgresPump : MonoBehaviour
{
    public GameObject Stantion1botum;
    public GameObject clickParticles; // Додано поле для партиклів

    private void Start()
    {
        // Initialization if needed
    }

    void OnMouseUpAsButton()
    {
        // Toggle the active state of Stantion1botum
        Stantion1botum.SetActive(!Stantion1botum.activeSelf);
        clickParticles.SetActive(!Stantion1botum.activeSelf);

    }
}
