using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private PointSelectManager pointSelectManager;
    [System.NonSerialized]
    public int Index;

    void Start()
    {
        pointSelectManager = transform.parent.GetComponent<PointSelectManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (pointSelectManager.IsCanInput)
        //{
        //    pointSelectManager.Select(Index);
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (pointSelectManager.IsCanInput)
        //{
        //    pointSelectManager.Unselect(Index);
        //}
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (pointSelectManager.IsCanInput)
        {
            if (Index == pointSelectManager.SelectIndex)
            {
                pointSelectManager.Decition();
            }
            else
            {
                pointSelectManager.Select(Index);
            }
        }
    }
}
