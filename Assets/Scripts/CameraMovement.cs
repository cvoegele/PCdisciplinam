using System;
using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 startPosition;

    public float minDistance;
    public float movementTime;
    public Vector3 initialDestination;

    private float tick;
    private Vector3 destination;
    private Vector3 originalViewDirection;
    private Vector3 movementStartPosition;
    private bool rotateCamera;
    private bool moveCamera;


    public float rotationSpeed;

    private void Start()
    {
        var position = transform.position;
        startPosition = position;
        originalViewDirection = Vector3.zero - position;
        destination = initialDestination;
    }

    public void ResetCamera()
    {
        SetDestination(startPosition, Vector3.zero, true);
    }

    public void SetDestination(Vector3 cameraDestination, Vector3? lookAt, bool rotateCamera, bool moveCamera = true,
        bool offsetEnabled = false)
    {
        //set lookAt position in parent without moving the camera, so we can later rotate around it
        if  (lookAt is Vector3 lA)
        {
            transform.parent.localRotation = Quaternion.identity;
            var oldParentPosition = transform.parent.position;
            transform.parent.position = lA;
            var difference = transform.parent.position - oldParentPosition;
            transform.localPosition -= difference;
        }
        
        this.moveCamera = moveCamera;
        this.rotateCamera = rotateCamera;

        if (offsetEnabled)
        {
            var offset = (startPosition - cameraDestination).normalized;
            offset.Scale(new Vector3(minDistance, minDistance, minDistance));
            this.destination = cameraDestination + offset;
        }
        else
        {
            this.destination = cameraDestination;
        }

        movementStartPosition = transform.position;

        tick = 0f;
        StartCoroutine(nameof(MoveAndRotate));
    }

    /**
     * amount is a variable between -1 and 1.
     * -1 = movement in negative target direction
     * 0 = no movement
     * 1 = movement to target
     */
    public void Move(float amount)
    {
        var isMovementBackwards = amount < 0;
        var targetDirection = transform.forward;
        var target = targetDirection * amount;
        SetDestination(transform.position + target, null, !isMovementBackwards);
    }

    public void RotateAroundLookAt(Vector3 axis)
    {
        // var cos = Mathf.Cos(angle);
        // var cosInv = Mathf.Cos(-angle);
        // var sin = Mathf.Cos(angle);
        // var sinInv = Mathf.Cos(-angle);
        // var q = new Quaternion(cos, sin * axis.x, sin * axis.y, sin * axis.z);
        // var qInvesere = new Quaternion(cosInv, sinInv * axis.x, sinInv * axis.y, sinInv * axis.z);
        // var p = transform.parent.rotation;
        transform.parent.localRotation *= Quaternion.Euler(axis).normalized;
    }
    
    

    IEnumerator MoveAndRotate()
    {
        while (tick < movementTime)
        {
            tick += Time.deltaTime;
            if (rotateCamera)
            {
                Vector3 targetDirection = destination - transform.position;
                //rotate Camera
                if (destination == startPosition)
                {
                    targetDirection = originalViewDirection;
                }

                var singleRotationStep = tick / movementTime;
                var newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleRotationStep, 0f);
                transform.rotation = Quaternion.LookRotation(newDirection);
            }

            if (moveCamera)
            {
                //move camera
                var moveThisFrame = Vector3.Lerp(movementStartPosition, destination, tick / movementTime);
                transform.position = moveThisFrame;
            }

            yield return null;
        }
    }
}