using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ButtonIndex))]
public class ClassTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    int index = -1;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (index == -1)
        {
            index = GetComponent<ButtonIndex>().GetIndex();
        }
        GameManager.instance.ShowClassTooltip(index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.instance.HideClassTooltip();
    }
}
