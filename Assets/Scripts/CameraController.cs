using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _camera;
    private float _targetZoom;
    private float _zoomFactor = 3f;
    private float _zoomLerpSpeed = 10f;
    private float _cameraMoveSpeed = 0.3f;

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
        _targetZoom = Mathf.Clamp(_targetZoom, 3f, 8f);
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetZoom, Time.deltaTime * _zoomLerpSpeed);

        var pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, -1.5f, 1.5f);
        pos.y = Mathf.Clamp(transform.position.y, -1f, 1f);
        transform.position = pos;

        if (Input.GetMouseButton(0))
        {
            float xSpeed = Input.GetAxis("Mouse X");
            float ySpeed = Input.GetAxis("Mouse Y");

            transform.position -= new Vector3(xSpeed, ySpeed, 0) * _cameraMoveSpeed;
        }
    }
}
