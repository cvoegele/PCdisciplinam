using System;
using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 startPosition;
    
    public float minDistance;
    public float movementTime;

    private float tick;
    private Vector3 destination;
    private Vector3 hitObjectPosition;
    private Vector3 originalViewDirection;
    private Vector3 movementStartPosition;

    public float rotationSpeed;
    void Start()
    {
        var position = transform.position;
        startPosition = position;
        hitObjectPosition = startPosition;
        originalViewDirection = Vector3.zero - position; 
    }



    void Update()
    {
       
       
    }

    public void ResetDestination()
    {
        SetDestination(startPosition);
    }

    public void SetDestination(Vector3 hitObjectPosition) 
    {
        var offset = (startPosition - hitObjectPosition).normalized;
        offset.Scale(new Vector3(minDistance, minDistance, minDistance));
        destination = hitObjectPosition + offset;
        movementStartPosition = transform.position;
        tick = 0f;
        StartCoroutine(nameof(MoveAndRotate));
    }

    IEnumerator MoveAndRotate()
    {
        while (tick < movementTime)
        {
            Vector3 targetDirection = destination - transform.position;
            //rotate Camera
            if (destination == startPosition)
            {
                targetDirection = originalViewDirection;
            }

            float singleRotationStep = rotationSpeed * Time.deltaTime;
            var newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleRotationStep, 0f);
            transform.rotation = Quaternion.LookRotation(newDirection);

            //move camera
            tick += Time.deltaTime;
            var moveThisFrame = Vector3.Lerp(movementStartPosition, destination, tick / movementTime);
            transform.position = moveThisFrame;
            yield return null;
        }
    }
    
}