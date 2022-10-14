using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelecter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ButtonSelectManager buttonSelectManager;
    private Button button;
    public int Index;

    void Start()
    {
        buttonSelectManager = transform.parent.GetComponent<ButtonSelectManager>();
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonSelectManager.Select(Index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonSelectManager.UnSelect(Index);
    }

    //public void OnClick()
    //{

    //}
}
