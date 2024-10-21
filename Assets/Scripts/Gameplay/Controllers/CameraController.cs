using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementTime;
    [SerializeField] private PlayerControls playerMoveControls;
    private Camera mainCamera;
    private Vector3 newPosition;


    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;
    private void Start()
    {
        newPosition = transform.position;
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (IsPointerOverUIObject())
            return;

        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            OnMouseDragStart();
        }

        if (Input.GetMouseButton(0))
        {
            OnMouseDrag();
        }
        if (Input.GetMouseButtonUp(0))
        {
            playerMoveControls.SetCameraDragging(false);
        }
        transform.position = Vector3.Lerp(transform.position, newPosition, movementSpeed * Time.deltaTime);
    }

    private void OnMouseDragStart()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float entry))
        {
            dragStartPosition = ray.GetPoint(entry);
        }
    }

    private void OnMouseDrag()
    {
        playerMoveControls.SetCameraDragging(true);

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float entry))
        {
            dragCurrentPosition = ray.GetPoint(entry);
            newPosition = transform.position + dragStartPosition - dragCurrentPosition;
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0;
    }
}
