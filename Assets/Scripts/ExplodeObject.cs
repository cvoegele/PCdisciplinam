using System.Collections;
using UnityEngine;

public class ExplodeObject : MonoBehaviour
{
    private Vector3 originalPosition;
    private Vector3 currentPosition;

    private Vector3 expandOrigin;
    private Vector3 expandDirection; //maximum expand of this object
    private float animationTime;

    private Vector3 newPosition;
    private float tick;
    private Exploder exploder;
    private Camera camera1;
    private float currentExpandF;

    public float rounding = 0.01f;

    public float CurrentExpandF => currentExpandF;

    private void Start()
    {
        camera1 = Camera.main;
    }

    public void AttachToExploder(Exploder exploder, int index)
    {
        this.exploder = exploder;
        originalPosition = gameObject.transform.position;
        expandOrigin = exploder.explodeOrigin;
        animationTime = exploder.animationTime;

        //calculate maximum allowed expand of this object
        var step = exploder.explodeDirection / exploder.explodies.Count;
        expandDirection = (step * index);

        newPosition = originalPosition;
    }

    public Vector3 OriginalPosition => originalPosition;


    public void ExpandAll(float f)
    {
        exploder.Expand(f);
    }

    public void Expand(float fDelta = 1f)
    {
        currentExpandF += fDelta;
        if (currentExpandF > 1)
        {
            currentExpandF = 1;
        }

        if (currentExpandF < 0)
        {
            currentExpandF = 0;
        }

        //originalPosition = newPosition;
        currentPosition = newPosition;
        newPosition = originalPosition + expandDirection * currentExpandF;
        tick = 0f;
        StartCoroutine("Move");
    }

    public float GetExpandDistanceInScreenSpace()
    {
        var end = camera1.WorldToScreenPoint(expandOrigin + expandDirection);
        var start = camera1.WorldToScreenPoint(expandOrigin);
        return (end - start).magnitude;
    }

    public Vector2 GetExpandVectorInScreenSpace()
    {
        var end = camera1.WorldToScreenPoint(expandOrigin + expandDirection);
        var start = camera1.WorldToScreenPoint(expandOrigin);
        return end - start;
    }


    IEnumerator Move()
    {
        while (tick < animationTime)
        {
            tick += Time.deltaTime;
            var thisPos = Vector3.Lerp(currentPosition, newPosition, tick / animationTime);
            transform.position = thisPos;
            yield return null;
        }
    }
}