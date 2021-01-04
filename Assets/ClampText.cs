using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClampText : MonoBehaviour
{
    public Text label;

  // Update is called once per frame
    void Update()
    {
        Vector3 labelPos = Camera.main.WorldToScreenPoint(this.transform.position);
        label.transform.position = labelPos;
    }
}
