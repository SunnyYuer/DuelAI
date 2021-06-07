using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldHelper
{
    public static Vector3 GetClickInWorldPosition()
    {
        Vector3 mousePosition;
#if UNITY_ANDROID || UNITY_IOS
        mousePosition = Input.GetTouch(0).position;
#else
        mousePosition = Input.mousePosition;
#endif
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Physics.Raycast(ray, out RaycastHit hit);
        //Debug.Log(hit.point);
        return hit.point;
    }
}
