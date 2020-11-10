using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Vector3 startPosition = new Vector3(10, 2, 0);

    public Camera mainCamera;
    public float minDistance;

    void Start()
    {
        startPosition = mainCamera.transform.position;
    }
    
    private Vector3 moveTo;
    
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if(hit.rigidbody != null)
                {
                    Vector3 temp = hit.rigidbody.position;
                    Debug.Log($"{temp}");
                    var direction = (transform.position - temp).normalized;
                    direction.Scale(new Vector3(minDistance, minDistance, minDistance));
                    moveTo = temp + direction;
                }
            }
        }

        if (moveTo != Vector3.zero && moveTo != transform.position)
        {
            //Debug.Log($"{moveTo}");
            var moveThisFrame = (moveTo - transform.position) * Time.deltaTime;
            transform.position += moveThisFrame;
        }
    }
}
