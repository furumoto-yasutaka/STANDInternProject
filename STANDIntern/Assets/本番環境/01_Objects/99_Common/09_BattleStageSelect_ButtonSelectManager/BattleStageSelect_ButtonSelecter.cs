using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleStageSelect_ButtonSelecter : ButtonSelecter
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (buttonSelectManager.IsCanInput)
        {
            if (Index == buttonSelectManager.SelectCursorIndex)
            {
                buttonSelectManager.Decition();
            }
            else
            {
                buttonSelectManager.Select(Index);
            }
        }
    }
}
