using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public bool isMoveStarting;
    Plane Plane;
    public Transform camTransform;
    public bool lockUp;
    public static CameraMove instance { get; private set; }
    public float mapMinX, mapMaxX, mapMinZ, mapMaxZ;
    [SerializeField] float moveSpeed;
    public Vector3 newPos;
    public Vector3 dragStartPos;
    public Vector3 dragCurrPos;
    public float zoomAmount;
    public float zoomOutMin;
    public float zoomOutMax;
    public float maxZoom;
    public float minZoom;
    [SerializeField] Camera _camera;

    void Awake()
    {
        instance = this;
        if (_camera == null)
            _camera = Camera.main;
    }

    void Start()
    {
        newPos = transform.position;
        zoomOutMin = minZoom;
        zoomOutMax = maxZoom;
    }

    void Update()
    {
        if (!lockUp)
            HandleMouseInput();
    }

    public void MoveTarget(Vector3 targetPos)
    {
        StartCoroutine(SmoothMove(targetPos, 0.5f));
    }

    IEnumerator SmoothMove(Vector3 targetPos, float duration)
    {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }

    void HandleMouseInput()
    {
        if (Input.touchCount == 1)
        {
            // Обробка переміщення за один дотик (тягнення по екрану)
            Plane.SetNormalAndPosition(transform.up, transform.position);

            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                isMoveStarting = true;
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = _camera.ScreenPointToRay(Input.GetTouch(0).position);
                float entry;
                if (plane.Raycast(ray, out entry))
                {
                    dragStartPos = ray.GetPoint(entry);
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (!isMoveStarting)
                {
                    isMoveStarting = true;
                    Plane _plane = new Plane(Vector3.up, Vector3.zero);
                    Ray _ray = _camera.ScreenPointToRay(Input.GetTouch(0).position);
                    float _entry;
                    if (_plane.Raycast(_ray, out _entry))
                    {
                        dragStartPos = _ray.GetPoint(_entry);
                    }
                }
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = _camera.ScreenPointToRay(Input.GetTouch(0).position);
                float entry;
                if (plane.Raycast(ray, out entry))
                {
                    dragCurrPos = ray.GetPoint(entry);
                    newPos = transform.position + dragStartPos - dragCurrPos;
                }
            }
        }
        else if (Input.touchCount == 2)
        {
            // Обробка жесту пінча (масштабування)
            isMoveStarting = false;

            // Отримання позицій двох торків
            Vector2 touch1Pos = Input.GetTouch(0).position;
            Vector2 touch2Pos = Input.GetTouch(1).position;

            // Отримання попередніх позицій двох торків
            Vector2 touch1PrevPos = touch1Pos - Input.GetTouch(0).deltaPosition;
            Vector2 touch2PrevPos = touch2Pos - Input.GetTouch(1).deltaPosition;

            // Визначення відстані між торками на поточному та попередньому кадрах
            float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
            float touchDeltaMag = (touch1Pos - touch2Pos).magnitude;

            // Визначення зміни відстані між торками
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Зміна зуму камери в залежності від зміни відстані між торками
            _camera.fieldOfView += deltaMagnitudeDiff * 0.1f;

            // Обмеження значення зуму, щоб воно не виходило за межі minZoom і maxZoom
            _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView, minZoom, maxZoom);
        }

        // Плавне переміщення камери до нової позиції
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * moveSpeed);

        // Обмеження позиції камери в межах вказаної карти
        transform.position = ClampCam(this.transform.position);
    }

    private Vector3 ClampCam(Vector3 targetPos)
    {
        float minX = mapMinX;
        float maxX = mapMaxX;
        float minZ = mapMinZ;
        float maxZ = mapMaxZ;
        float newX = Mathf.Clamp(targetPos.x, minX, maxX);
        float newZ = Mathf.Clamp(targetPos.z, minZ, maxZ);
        return new Vector3(newX, targetPos.y, newZ);
    }

    void Zoom(float increment)
    {
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - increment, minZoom, maxZoom);
    }

    Vector3 PlanePos(Vector2 screenPos)
    {
        var rayNow = _camera.ScreenPointToRay(screenPos);
        if (Plane.Raycast(rayNow, out var enterNow))
            return rayNow.GetPoint(enterNow);
        return Vector3.zero;
    }
}
