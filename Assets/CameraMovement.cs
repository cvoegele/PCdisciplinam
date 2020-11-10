﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 startPosition;

    public Camera mainCamera;
    public float minDistance;
    public float zoomRounding;

    void Start()
    {
        startPosition = transform.position;
    }

    private Vector3 moveTo;
    private bool zooming;
    private Vector3 hitObjectPosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin,ray.direction, Color.magenta, 20);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.rigidbody != null)
                {
                    Vector3 newHitPosition = hit.rigidbody.position;
                    if (hitObjectPosition != newHitPosition)
                    {
                        hitObjectPosition = newHitPosition;
                        Debug.Log($"{hitObjectPosition}");
                        var direction = (startPosition - hitObjectPosition).normalized;
                        direction.Scale(new Vector3(minDistance, minDistance, minDistance));
                        moveTo = hitObjectPosition + direction;
                    }
                }
                
            }
            else
            {
                hitObjectPosition = startPosition;
                Debug.Log("i hit nothing");
                moveTo = startPosition;
            }
        }

        if (moveTo != Vector3.zero && Math.Abs((moveTo - transform.position).magnitude) > zoomRounding)
        {
            Debug.Log($"we are zoomin");
            var moveThisFrame = (moveTo - transform.position) * Time.deltaTime;
            transform.position += moveThisFrame;
        }

        // //we have zoomed in
        // if (zooming && Math.Abs((moveTo - transform.position).magnitude) < zoomRounding)
        // {
        //     Debug.Log("we are zoomed");
        //     moveTo = startPosition;
        //     zooming = false;
        // }
    }
}