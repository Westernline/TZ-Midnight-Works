using System.Collections;
using UnityEngine;

public class FieldOfViewSync : MonoBehaviour
{
    public Camera sourceCamera; // Камера, з якої беремо field of view
    public Camera targetCamera; // Камера, для якої встановлюємо field of view

    void Start()
    {

    }
    void Update()
    {
        if (sourceCamera != null && targetCamera != null)
            {
                // Встановлюємо поле огляду targetCamera з sourceCamera
                targetCamera.fieldOfView = sourceCamera.fieldOfView;
            }
            else
            {
                Debug.LogWarning("Source or Target camera is not assigned.");
            }
    }


}
