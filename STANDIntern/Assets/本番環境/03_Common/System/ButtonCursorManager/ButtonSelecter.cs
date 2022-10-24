using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ButtonSelecter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    protected ButtonSelectManager buttonSelectManager;
    [System.NonSerialized]
    public int Index;
    public UnityEvent OnClickCallBack;
    public UnityEvent OnSelectCallBack;
    public UnityEvent OnUnselectCallBack;

    protected void Start()
    {
        buttonSelectManager = transform.parent.GetComponent<ButtonSelectManager>();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonSelectManager.IsCanInput)
        {
            buttonSelectManager.Select(Index);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        //if (buttonSelectManager.IsCanInput)
        //{
        //    buttonSelectManager.Unselect(Index);
        //}
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (buttonSelectManager.IsCanInput)
        {
            buttonSelectManager.Decition();
        }
    }

    public void MySelect()
    {
        buttonSelectManager.Select(Index);
    }
}
