using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butom : MonoBehaviour
{
    void Update()
    {
        LookAtCamera();
    }

    void OnMouseUpAsButton()
    {
        //Debug.Log("This is a button, I must have a collider to detect");
    }

    void LookAtCamera()
    {
        // Отримуємо поточну активну камеру
        Camera camera = Camera.main;
        if (camera != null)
        {
            // Встановлюємо об'єкт, щоб він дивився на камеру
            transform.LookAt(camera.transform);
        }
    }
}
