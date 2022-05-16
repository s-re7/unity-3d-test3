using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePoint : MonoBehaviour
{
    private Camera _mainCamera;
    private Vector3 _clickedPointOnField;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!Input.GetMouseButton(0)) return;
        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out var hit);
        if (hit.collider == null) return;
        if (hit.collider.CompareTag("Field"))
        {
            _clickedPointOnField = hit.point;
        }
        if (hit.collider.CompareTag("Building"))
        {
            var colliderTransform = hit.collider.transform;
            var buildingHeightFromCenter = new Vector3(0, colliderTransform.localScale.y * 0.5f, 0);
            _clickedPointOnField = colliderTransform.position - buildingHeightFromCenter;
        }
    }

    public Vector3 GetTargetPointForObject(float objectPositionHeight)
    {
        return new Vector3(_clickedPointOnField.x, _clickedPointOnField.y + objectPositionHeight, _clickedPointOnField.z);
    }
}