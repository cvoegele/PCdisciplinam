using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Vector3 startPosition = new Vector3(10, 2, 0);

    void Start()
    {
        transform.position = startPosition;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if(hit.rigidbody != null)
                {
                    Vector3 temp = hit.rigidbody.position;
                    temp.x = 5;
                    transform.position = temp;
                }
            }
        }
    }
}
