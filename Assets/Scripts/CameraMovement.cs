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
    private float parentTick;
    private Vector3 destination;
    private Vector3 originalViewDirection;
    private Vector3 movementStartPosition;
    private Quaternion parentRotation;
    private Quaternion startRotation;
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
        SetRotationPoint(Vector3.zero);
        SetDestination(startPosition, true);
    }

    public void SetDestination(Vector3 cameraDestination, bool rotateCamera, bool moveCamera = true,
        bool offsetEnabled = false)
    {

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
        var targetDirection = transform.parent.position - transform.position;
        var target = targetDirection * amount;
        SetDestination(transform.position + target, false);
    }

    /// <summary>
    /// Set's transform of parent to certain point in world scape, so that the camera can later "rotate around" it.
    /// </summary>
    /// <param name="point"></param>
    public void SetRotationPoint(Vector3 point)
    {
        transform.parent.localRotation = Quaternion.identity;
        var oldParentPosition = transform.parent.position;
        transform.parent.position = point;
        var difference = transform.parent.position - oldParentPosition;
        transform.localPosition -= difference;
    }

    public void RotateAroundLookAt(Vector3 axis)
    {
        parentTick = 0;
        parentRotation = Quaternion.Euler(axis).normalized;
        startRotation = transform.parent.rotation;
        //var rotation = Quaternion.Lerp(startRotation, parentRotation, Time.deltaTime);
        transform.parent.localRotation *= parentRotation;
        //StartCoroutine(nameof(RotateParent));

    }

    IEnumerator RotateParent()
    {
        while (parentTick < movementTime)
        {
            parentTick += Time.deltaTime;
            var rotation = Quaternion.Lerp(startRotation, parentRotation, parentTick / movementTime);
            transform.parent.localRotation = startRotation * rotation;
            yield return null;
        }
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
                var forward = transform.parent.position - movementStartPosition;
                var newDirection = Vector3.RotateTowards(forward.normalized, targetDirection, singleRotationStep, 0f);
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