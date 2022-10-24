using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStageSelectArrowManager : MonoBehaviour
{
    [SerializeField]
    private ButtonSelectManager buttonSelectManager;

    public void CheckArrowActive()
    {
        if (buttonSelectManager.SelectIndex == 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }

        if (buttonSelectManager.SelectIndex == buttonSelectManager.transform.childCount - 1)
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
