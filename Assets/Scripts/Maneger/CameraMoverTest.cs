using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoverTest : MonoBehaviour
{
    public bool isMoveStarting;
    Plane Plane;
    public Transform camTransform;
    public bool lockUp;
    public static CameraMoverTest instance { get; private set; }
    public float mapMinX, mapMaxX, mapMinZ, mapMaxZ;
    [SerializeField] float moveSpeed;
    public Vector3 newPos;
    public Vector3 dragStartPos;
    public Vector3 dragCurrPos;
    public float zoomAmount;

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
        zoomAmount = (_camera.orthographic ? _camera.orthographicSize : _camera.fieldOfView);
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
        if (Input.touchCount >= 1)
        {
            Plane.SetNormalAndPosition(transform.up, transform.position);
        }

        if (Input.touchCount >= 2)
        {
            isMoveStarting = false;
            var pos1 = PlanePos(Input.GetTouch(0).position);
            var pos2 = PlanePos(Input.GetTouch(1).position);
            var pos1b = PlanePos(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition);
            var pos2b = PlanePos(Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);
            var zoom = Vector3.Distance(pos1, pos2) / Vector3.Distance(pos1b, pos2b);
            if (zoom == 0 || zoom > 10)
                return;

            // zoomout
            if (zoom < 1 && zoomAmount < minZoom)
            {
                _camera.transform.position = Vector3.LerpUnclamped(pos1, _camera.transform.position, 1 / zoom);
                var _zoom = 80f * Time.deltaTime;
                zoomAmount += _zoom;
            }
            // zoomin
            else if (zoom > 1 && zoomAmount > maxZoom)
            {
                _camera.transform.position = Vector3.LerpUnclamped(pos1, _camera.transform.position, 1 / zoom);
                var _zoom = 80f * Time.deltaTime;
                zoomAmount -= _zoom;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            isMoveStarting = true;
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            float entry;
            if (plane.Raycast(ray, out entry))
            {
                dragStartPos = ray.GetPoint(entry);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (!isMoveStarting)
            {
                isMoveStarting = true;
                Plane _plane = new Plane(Vector3.up, Vector3.zero);
                Ray _ray = _camera.ScreenPointToRay(Input.mousePosition);
                float _entry;
                if (_plane.Raycast(_ray, out _entry))
                {
                    dragStartPos = _ray.GetPoint(_entry);
                }
            }
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            float entry;
            if (plane.Raycast(ray, out entry))
            {
                dragCurrPos = ray.GetPoint(entry);
                newPos = transform.position + dragStartPos - dragCurrPos;
            }
        }

        // Масштабування миші
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (zoomAmount > minZoom)
            {
                zoomAmount -= 1f;
                ApplyZoom();
            }
        }
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (zoomAmount < maxZoom)
            {
                zoomAmount += 1f;
                ApplyZoom();
            }
        }

        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * moveSpeed);
        transform.position = ClampCam(this.transform.position);
    }

    private void ApplyZoom()
    {
        if (_camera.orthographic)
        {
            _camera.orthographicSize = Mathf.Clamp(zoomAmount, minZoom, maxZoom);
        }
        else
        {
            _camera.fieldOfView = Mathf.Clamp(zoomAmount, minZoom, maxZoom);
        }
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

    Vector3 PlanePos(Vector2 screenPos)
    {
        var rayNow = _camera.ScreenPointToRay(screenPos);
        if (Plane.Raycast(rayNow, out var enterNow))
            return rayNow.GetPoint(enterNow);
        return Vector3.zero;
    }
}
