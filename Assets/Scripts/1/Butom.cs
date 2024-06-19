using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butom : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Отримуємо позицію камери
        Vector3 cameraPosition = mainCamera.transform.position;

        // Обчислюємо напрямок від об'єкта до камери
        Vector3 directionToCamera = cameraPosition - transform.position;

        // Оновлюємо forward вектор об'єкта, щоб він завжди дивився на камеру
        transform.forward = directionToCamera;
    }
}
