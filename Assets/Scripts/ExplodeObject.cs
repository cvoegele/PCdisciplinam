using System.Collections;
using UnityEngine;

public class ExplodeObject : MonoBehaviour
{

    private Vector3 originalPosition;

    private Vector3 expandOrigin;
    private Vector3 expandDirection; //maximum expand of this object
    private float animationTime;

    private Vector3 newPosition;
    private float tick;

    public void AttachToExploder(Exploder exploder, int index)
    {
        originalPosition = gameObject.transform.position;
        expandOrigin = exploder.explodeOrigin;
        animationTime = exploder.animationTime;

        //calculate maximum allowed expand of this object
        var step = exploder.explodeDirection / exploder.explodies.Count;
        expandDirection = (step * index) + expandOrigin;

        newPosition = originalPosition;
    }
    
    public Vector3 OriginalPosition => originalPosition;

    public void Expand(float f = 1f)
    {
        originalPosition = newPosition;
        newPosition = expandDirection * f;
        tick = 0f;
        StartCoroutine("Move");
    }

    public void Collapse()
    {
        var tmp = newPosition;
        newPosition = OriginalPosition;
        originalPosition = tmp;
        tick = 0f;
        StartCoroutine("Move");
    }

    IEnumerator Move()
    {
        while (tick < animationTime)
        {
            tick += Time.deltaTime;
            var thisPos = Vector3.Lerp(OriginalPosition, newPosition, tick / animationTime);
            transform.position = thisPos;
            yield return null;
        }
    }
}