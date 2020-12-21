using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Selector : MonoBehaviour
{
    private CameraMovement cameraMovement;
    private GameObject selection;
    public Material selectionMaterial;
    public Material oldMaterial;

    private Vector2 touch0Down = Vector2.zero;
    private Vector2 touch1Down = Vector2.zero;
    private float previousAmount = 0;


    // Start is called before the first frame update
    public void Start()
    {
        cameraMovement = GetComponentInChildren<CameraMovement>();
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.touchCount == 3)
        {
            if (AllTouchesAreInPhase(TouchPhase.Ended))
            {
                DeSelect();
                cameraMovement.ResetCamera();
            }
        }
        //two finger movement
        else if (Input.touchCount == 2)
        {
            var touch0 = Input.touches[0];
            var touch1 = Input.touches[1];

            if (AllTouchesAreInPhase(TouchPhase.Began))
            {
                touch0Down = touch0.position;
                touch1Down = touch1.position;
            }

            if (AllTouchesAreInPhase(TouchPhase.Moved))
            {
                var downDelta = (touch0Down - touch1Down).magnitude;
                var delta = (touch0.position - touch1.position).magnitude;

                if (downDelta > delta)
                {
                    var amount = delta / downDelta;
                    cameraMovement.Move(-amount);
                }
                else if (downDelta < delta)
                {
                    var amount = downDelta / delta;
                    cameraMovement.Move(amount);
                }
                
                touch0Down = touch0.position;
                touch1Down = touch1.position;
            }
        }
        else if (Input.GetMouseButtonDown(0))
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
                        Select(hit.transform.gameObject);
                    }
                    //if already something selected
                    else
                    {
                        //press same object again
                        if (selection == hit.rigidbody.gameObject)
                        {
                            var position = hit.rigidbody.position;
                            var newHitPosition = position;

                            cameraMovement.SetDestination(newHitPosition, true, true);
                        }
                        else
                        {
                            //press something else
                            Select(hit.transform.gameObject);
                        }
                    }
                }
            }

            //hit nothing
            // else
            // {
            //     turnOffSelectionMaterial();
            //     Debug.Log($"{selection}");
            //     selection = null;
            //     cameraMovement.ResetDestination();
            // }
        }
    }

    private bool AllTouchesAreInPhase(TouchPhase phase)
    {
        return Input.touches.All(touch => touch.phase == phase);
    }

    private void Select(GameObject gameObject)
    {
        TurnOffSelectionMaterial();
        selection = gameObject;
        TurnOnSelectionMaterial();
    }

    private void DeSelect()
    {
        TurnOffSelectionMaterial();
        selection = null;
    }

    private void TurnOnSelectionMaterial()
    {
        var renderer = selection.GetComponent<Renderer>();
        if (renderer != null)
        {
            oldMaterial = renderer.material;
            renderer.material = selectionMaterial;
        }
    }

    private void TurnOffSelectionMaterial()
    {
        if (selection == null) return;

        var renderer = selection.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = oldMaterial;
            oldMaterial = null;
        }
    }
}