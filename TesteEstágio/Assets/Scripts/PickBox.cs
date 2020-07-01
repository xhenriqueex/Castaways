using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickBox : MonoBehaviour
{
    // Box picking variables
    public Transform theDest;
    public bool isCarrying = false;

    // Selection highlighting variables
    [SerializeField] private string selectableTag = "Boxes";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;
    private Transform _selection;

    // Throw Variables
    public float throwForce; 

    // Selection highlighting functions
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isCarrying == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.tag == "Boxes")
                {
                    isCarrying = true;
                }
            }
        }
        else
        {
            if(Input.GetMouseButtonDown(0) && isCarrying)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 hitPoint = hit.point;
                    Vector3 mouseDir = hitPoint - this.transform.position;
                    mouseDir = mouseDir.normalized;
                    this.GetComponent<Rigidbody>().AddForce(mouseDir * throwForce);
                    isCarrying = false;
                }
            }
        }

        //if(Input.GetMouseButtonDown(0) && isCarrying)
        //{
            // Arremessar na direção do mouse
            //this.transform.parent = null;
            //GetComponent<Rigidbody>().useGravity = true;
            //GetComponent<BoxCollider>().enabled = true;
            //isCarrying = false;
        //}

        if (isCarrying)
        {
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Rigidbody>().useGravity = false;
            this.transform.position = theDest.position;
            this.transform.parent = GameObject.Find("ti_hand").transform;
        }
        else
        {
            this.transform.parent = null;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<BoxCollider>().enabled = true;
        }

        if (_selection != null)
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            _selection = null;
        }
        var rayHighlight = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitHighlight;
        if (Physics.Raycast(rayHighlight, out hitHighlight))
        {
            var selection = hitHighlight.transform;
            if (selection.CompareTag(selectableTag))
            {
                var selectionRenderer = selection.GetComponent<Renderer>();
                if (selectionRenderer != null)
                {
                    selectionRenderer.material = highlightMaterial;
                }
                _selection = selection;
            }
        }

    }
}