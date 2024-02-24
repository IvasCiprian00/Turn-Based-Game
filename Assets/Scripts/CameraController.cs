using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _camera;
    private float _targetZoom;
    private float _zoomFactor = 3f;
    private float _zoomLerpSpeed = 10f;

    private void Start()
    {
        _camera = Camera.main;
        _targetZoom = _camera.orthographicSize;
    }

    private void Update()
    {
        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");

        _targetZoom -= scrollData * _zoomFactor;
        _targetZoom = Mathf.Clamp(_targetZoom, 3f, 8);
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetZoom, Time.deltaTime * _zoomLerpSpeed);
    }
}
