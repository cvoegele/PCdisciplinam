using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Exploder : MonoBehaviour
{

    public Vector3 explodeDirection;
    public Vector3 explodeOrigin;

    public List<ExplodeObject> explodies;
    public float animationTime;

    // Start is called before the first frame update
    void Start()
    {
        explodeOrigin = transform.position;

        explodies = new List<ExplodeObject>();

        foreach (Transform child in transform)
        {
            var explodBody = child.GetComponent<ExplodeObject>();
            explodies.Add(explodBody);
        }

        explodies = explodies.OrderBy(child => (child.OriginalPosition - explodeOrigin).magnitude).ToList();

        int expandOrderIndex = 1;
        foreach (var explodi in explodies)
        {
            explodi.AttachToExploder(this, expandOrderIndex++);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Expand();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Collapse();
        }
    }

    public void Expand()
    {
        foreach (var explod in explodies)
        {
            explod.Expand();
        }
    }

    public void Collapse()
    {
        foreach (var explod in explodies)
        {
            explod.Collapse();
        }
    }

    private void OnDrawGizmos()
    {
        //if not set set manually for the gizmos, for easier debug
        if (explodeOrigin == Vector3.zero)
        {
            explodeOrigin = transform.position;
        }

        Gizmos.DrawRay(explodeOrigin, explodeDirection);
    }
}