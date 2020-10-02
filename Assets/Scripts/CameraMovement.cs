using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float dist;
    private Vector3 MouseStart;

    private Camera cam;
    private float targetZoom;
    private float zoomFactor = 3f;
    [SerializeField] private float zoomLerpSpeed = 10f;

    void Start()
    {
        dist = transform.position.z;  // Distance camera is above map
        cam = Camera.main;
        targetZoom = cam.orthographicSize;
    }

    void Update()
    {
        //right click to move
        if (Input.GetMouseButtonDown(1))
        {
            MouseStart = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
            MouseStart = Camera.main.ScreenToWorldPoint(MouseStart);
            MouseStart.z = transform.position.z;

        }
        else if (Input.GetMouseButton(1))
        {
            var MouseMove = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
            MouseMove = Camera.main.ScreenToWorldPoint(MouseMove);
            MouseMove.z = transform.position.z;
            transform.position = transform.position - (MouseMove - MouseStart);
        }

        //scroll in/out
        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scrollData * zoomFactor;
        targetZoom = Mathf.Clamp(targetZoom, 5f, 16f);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
    }
}
