using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    private Vector3 _startDragPosition;
    private bool _isDragging;
    private void Update()
    {
        if (_isDragging)
        {
            mainCam.transform.position -=
                mainCam.ScreenToWorldPoint(Input.mousePosition) - _startDragPosition;
            _startDragPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            _isDragging = true;
            _startDragPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
        }
        
        if (Input.GetMouseButtonUp(1))
        {
            _isDragging = false;
        }
    }
}