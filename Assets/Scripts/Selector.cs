using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Selector : MonoBehaviour
{
    private CameraMovement cameraMovement;
    private GameObject selection;

    private Vector2 touch0Down = Vector2.zero;
    private Vector2 touch1Down = Vector2.zero;
    private Vector2 expandTouchDown = Vector2.zero;
    private float oldExpandDistance;
    private float oldExpandDelta;
    private Camera mainCamera;


    // Start is called before the first frame update
    public void Start()
    {
        mainCamera = Camera.main;
        cameraMovement = GetComponentInChildren<CameraMovement>();
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.touchCount == 3)
        {
            DeSelect();
            cameraMovement.ResetCamera();
        }
        //two finger movement
        else if (Input.touchCount == 2)
        {
            ClassifyTwoFingerInput();
        }
        else if (Input.touchCount == 1)
        {
            var touch0 = Input.touches[0];

            switch (touch0.phase)
            {
                case TouchPhase.Began:
                case TouchPhase.Moved:

                    if (selection.CompareTag("Expandable"))
                    {
                        FingerDragExpand();
                    }

                    break;
                case TouchPhase.Ended:
                    RayCastSelection();
                    break;
            }
        }
    }

    private void RayCastSelection()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
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

                        cameraMovement.SetDestination(newHitPosition,true, false);
                    }
                    else
                    {
                        //press something else
                        Select(hit.transform.gameObject);
                    }
                }
            }
        }
    }

    private void ClassifyTwoFingerInput()
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
            var move0 = touch0.position - touch0Down;
            var move1 = touch1.position - touch1Down;
            var dot = Vector2.Dot(move0.normalized, move1.normalized);

            if (dot > 0)
            {
                //fingers move into somewhat the same direction
                
            }
            else
            {
                //fingers do not move in the same direction
                TwoFingerZoom(touch0, touch1);
            }
        }
    }

    private void TwoFingerZoom(Touch touch0, Touch touch1)
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

    private void FingerDragExpand()
    {
        var touch0 = Input.touches[0];

        if (touch0.phase == TouchPhase.Began)
        {
            expandTouchDown = touch0.position;
        }

        if (touch0.phase == TouchPhase.Moved)
        {
            var explodeObject = selection.GetComponent<ExplodeObject>();
            var maxExpandDistance = Screen.height;

            var expandVector = touch0.position - expandTouchDown;
            var expandDistance = expandVector.magnitude;

            var dragPart = expandDistance / maxExpandDistance;

            if (Vector2.Angle(explodeObject.GetExpandVectorInScreenSpace(), expandVector) < 90)
            {
                explodeObject.ExpandAll(dragPart);
            }
            else
            {
                explodeObject.ExpandAll(-dragPart);
            }
        }
    }

    private static bool AllTouchesAreInPhase(TouchPhase phase)
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
        var outline = selection.AddComponent<Outline>();

        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.white;
        outline.OutlineWidth = 10f;
    }

    private void TurnOffSelectionMaterial()
    {
        if (selection == null) return;
        Destroy(selection.GetComponent<Outline>());
        
    }
}