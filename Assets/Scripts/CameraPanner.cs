using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CameraPanner : MonoBehaviour
{
    // Button Input Vars
    public float panSpeed = 20;
    public float scrollSpeed = 20;
    public float minY = 5f;
    public float maxY = 30f;
    public float zoom = 4f;

    // Mouse Input Vars
    private Vector3 cameraDragOrigin;
    private Vector3 currentPosition;
    public float dragSpeed = 2;
    private Vector3 dragOrigin;

    [SerializeField]
    VisualEffect visualEffect;



    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");


        Vector3 newPosition = new Vector3(transform.position.x + h * Time.deltaTime * panSpeed, transform.position.y + v * Time.deltaTime * panSpeed, transform.position.z);
        transform.position = newPosition;
        ZoomCamera();

        visualEffect.SetFloat("Zoom", zoom);
    }

    // zoom via scrollwheel
    void ZoomCamera()
    {
        Vector3 pos = transform.position;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoom -= scroll * scrollSpeed * Time.deltaTime * 300f;
        zoom = Mathf.Clamp(zoom, minY, maxY);
    }
}

