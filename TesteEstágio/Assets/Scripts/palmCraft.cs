using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmCraft : MonoBehaviour
{
    // Selection highlighting variables
    [SerializeField] private string selectableTag = "Palms";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;
    private Transform _selection;

    // Update is called once per frame
    void Update()
    {
        // Spawn rope
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitPalm;
            if (Physics.Raycast(ray, out hitPalm))
            {
                if (hitPalm.transform.tag == "Palms")
                {
                    //spawn the rope near palm
                }
            }
        }
        // Highlight material
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
