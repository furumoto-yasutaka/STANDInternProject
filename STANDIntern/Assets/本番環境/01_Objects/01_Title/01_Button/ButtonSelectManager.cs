using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelectManager : MonoBehaviour
{
    private Button[] buttons;
    private ButtonSelecter[] selecters;
    private int selectIndex = -1;

    void Start()
    {
        buttons = new Button[transform.childCount];
        selecters = new ButtonSelecter[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform trans = transform.GetChild(i);
            buttons[i] = trans.GetComponent<Button>();
            selecters[i] = trans.GetComponent<ButtonSelecter>();
            selecters[i].Index = i;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (selectIndex == -1)
            {
                Select(0);
            }
            else
            {
                Select((selectIndex - 1 + transform.childCount) % transform.childCount);
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (selectIndex == -1)
            {
                Select(0);
            }
            else
            {
                int num = (selectIndex + 1) % transform.childCount;
                Select(num);
            }
        }
    }

    public void Select(int index)
    {
        selectIndex = index;
        buttons[selectIndex].Select();
    }

    public void UnSelect(int index)
    {
        if (index == selectIndex)
        {
            selectIndex = -1;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
