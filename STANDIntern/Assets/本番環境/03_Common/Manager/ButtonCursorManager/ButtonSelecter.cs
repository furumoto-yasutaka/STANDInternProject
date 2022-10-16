using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelecter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ButtonSelectManager buttonSelectManager;
    [System.NonSerialized]
    public int Index;

    void Start()
    {
        buttonSelectManager = transform.parent.GetComponent<ButtonSelectManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonSelectManager.IsCanInput)
        {
            buttonSelectManager.Select(Index);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (buttonSelectManager.IsCanInput)
        //{
        //    buttonSelectManager.Unselect(Index);
        //}
    }
}
