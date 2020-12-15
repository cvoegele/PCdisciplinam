using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 startPosition;
    
    public float minDistance;
    public float zoomRounding;

    public float rotationSpeed;
    void Start()
    {
        var position = transform.position;
        startPosition = position;
        hitObjectPosition = startPosition;
        originalViewDirection = Vector3.zero - position; 
    }

    private Vector3 moveTo;
    private Vector3 targetPosition;
    private Vector3 hitObjectPosition;
    private Vector3 originalViewDirection;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin,ray.direction, Color.magenta, 20);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.rigidbody != null)
                {
                    var position = hit.rigidbody.position;
                    Vector3 newHitPosition = position;
                    targetPosition = position;
                    if (hitObjectPosition != newHitPosition)
                    {
                        hitObjectPosition = newHitPosition;
                       
                        var direction = (startPosition - hitObjectPosition).normalized;
                        direction.Scale(new Vector3(minDistance, minDistance, minDistance));
                        moveTo = hitObjectPosition + direction;
                    }
                }
                
            }
            else
            {
                targetPosition = Vector3.zero;
                hitObjectPosition = startPosition;
                
                moveTo = startPosition;
            }
            StartCoroutine("MoveAndRotate");
        }
       
    }

    IEnumerable MoveAndRotate()
    {
        while (moveTo != Vector3.zero && Math.Abs((moveTo - transform.position).magnitude) > zoomRounding)
        {
            Vector3 targetDirection = targetPosition - transform.position;
            //rotate Camera
            if (targetPosition == Vector3.zero)
            {
                targetDirection = originalViewDirection;
            }

            float singleRotationStep = rotationSpeed * Time.deltaTime;
            var newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleRotationStep, 0f);
            transform.rotation = Quaternion.LookRotation(newDirection);

            //move camera
            Debug.Log($"we are zoomin");
            var moveThisFrame = (moveTo - transform.position) * Time.deltaTime;
            transform.position += moveThisFrame;
            yield return null;
        }
    }
    
}