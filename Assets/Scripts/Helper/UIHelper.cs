using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHelper
{
    public static bool RaycastUICheck(GameObject raycastUI, GameObject parentUI = null)
    {
        //创建一个点击事件
        PointerEventData eventData = new PointerEventData(EventSystem.current);
#if UNITY_ANDROID || UNITY_IOS
        eventData.position = Input.GetTouch(0).position;
#else
        eventData.position = Input.mousePosition;
#endif
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        //向点击位置发射一条射线，检测是否点击UI
        EventSystem.current.RaycastAll(eventData, raycastResults);
        List<GameObject> uiObjects = new List<GameObject>();
        foreach (RaycastResult result in raycastResults)
        {
            //Debug.Log(result.gameObject.name);
            uiObjects.Add(result.gameObject);
        }
        if (uiObjects.Contains(raycastUI) || (parentUI != null && uiObjects.Contains(parentUI)))
        { // 点击到该ui或者该父ui
            return true;
        }
        // 点击到其余地方
        return false;
    }
}
