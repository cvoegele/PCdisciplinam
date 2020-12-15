using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{

    private CameraMovement cameraMovement;
    private GameObject selection;
    public Material selectionMaterial;
    public Material oldMaterial;

    // Start is called before the first frame update
    void Start()
    {
        cameraMovement = GetComponentInChildren<CameraMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction, Color.magenta, 20);
            if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Hittable")))
            {
                if (hit.rigidbody != null)
                {
                    //if nothing selected prior
                    if (selection == null)
                    {
                        selection = hit.transform.gameObject;
                        turnOnSelectionMaterial();
                    }
                    //if already something selected
                    else
                    {
                        //press same object again
                        if (selection == hit.rigidbody.gameObject)
                        {

                            var position = hit.rigidbody.position;
                            Vector3 newHitPosition = position;

                            cameraMovement.SetDestination(newHitPosition);

                        }
                        else
                        //press something else
                        {
                            turnOffSelectionMaterial();
                            selection = hit.transform.gameObject;
                            turnOnSelectionMaterial();
                        }

                    }
                }
            }
            //hit nothing
            else
            {
                turnOffSelectionMaterial();
                selection = null;
                cameraMovement.ResetDestination();
            }

        }

    }

    private void turnOnSelectionMaterial()
    {

        var renderer = selection.GetComponent<Renderer>();
        if (renderer != null)
        {
            oldMaterial = renderer.material;
            renderer.material = selectionMaterial;
        }

    }

    private void turnOffSelectionMaterial()
    {
        var renderer = selection.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = oldMaterial;
            oldMaterial = null;
        }
    }
}
