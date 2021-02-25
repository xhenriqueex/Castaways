using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public Transform followTransform;
    public Transform cameraTransform;

    public float normalSpeed;
    public float fastSpeed;
    public float movementSpeed;
    public float movementTime;
    public float normalRotation;
    public float fastRotation;
    public float rotationAmount;
    public Vector3 zoomAmount;

    public float minZoom;
    public float maxZoom;

    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;

    public Vector3 dragStartPosition;
    public Vector3 dragCurrentPosition;
    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        //cameraOffset = transform.position - playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (followTransform != null)
        {
            transform.position = Vector3.Lerp(transform.position, followTransform.position, Time.deltaTime * movementTime);
        }
        else
        {
            HandleMouseInput();
            HandleMovementInput();
        }
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            followTransform = null;
        }
    }

    void HandleMouseInput()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom -= Input.mouseScrollDelta.y * zoomAmount;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitCamera;
            if(Physics.Raycast(ray, out hitCamera))
            {
                if (hitCamera.transform.tag == "Players")
                {
                    followTransform = hitCamera.transform;
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if(plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }

        if (Input.GetMouseButton(1))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);

                newPosition = transform.position + dragStartPosition - dragCurrentPosition;

            }
        }

        //rotation locked
        //if (Input.GetMouseButtonDown(0))
        //{
        //    rotateStartPosition = Input.mousePosition;
        //}
        //if (Input.GetMouseButton(0))
        //{
        //    rotateCurrentPosition = Input.mousePosition;

        //    Vector3 difference = rotateStartPosition - rotateCurrentPosition;

        //    rotateStartPosition = rotateCurrentPosition;

        //   newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 10f));
        //}
    }

    void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
            rotationAmount = fastRotation;
        }
        else
        {
            movementSpeed = normalSpeed;
            rotationAmount = normalRotation;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * movementSpeed);
        }
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed);
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed);
        }
        // rotation locked
        //if (Input.GetKey(KeyCode.E))
        //{
        //    newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        //}
        //if (Input.GetKey(KeyCode.Q))
        //{
        //   newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        //}

        if (Input.GetKey(KeyCode.F))
        {
            newZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.R))
        {
            newZoom -= zoomAmount;
        }

        newZoom.y = Mathf.Clamp(newZoom.y, minZoom, maxZoom);
        newZoom.z = Mathf.Clamp(newZoom.z, -maxZoom, -minZoom);

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
