using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelectManager : InputLockElement
{
    public enum ButtonLine
    {
        Vertical = 0,
        Horizontal,
        Length,
    }

    private Button[] buttons;
    private ButtonSelecter[] selecters;

    [SerializeField]
    private ButtonLine buttonLine = ButtonLine.Vertical;
    private System.Action[] cursorMoveAction;
    private int selectIndex = -1;

    void Start()
    {
        buttons = new Button[transform.childCount];
        selecters = new ButtonSelecter[transform.childCount];
        cursorMoveAction = new System.Action[(int)ButtonLine.Length]
            { CursorMove_Vertical, CursorMove_Horizontal };

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform trans = transform.GetChild(i);
            buttons[i] = trans.GetComponent<Button>();
            selecters[i] = trans.GetComponent<ButtonSelecter>();
            selecters[i].Index = i;
        }

        if (transform.childCount == 0)
        {
            LockInput();
        }
        else
        {
            buttons[0].Select();
        }
    }

    void Update()
    {
        if (!IsCanInput) { return; }

        cursorMoveAction[(int)buttonLine]();
    }

    private void CursorMove_Vertical()
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

    private void CursorMove_Horizontal()
    {
        if (Input.GetKeyDown(KeyCode.A))
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

        if (Input.GetKeyDown(KeyCode.D))
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

    public void Unselect(int index)
    {
        if (index == selectIndex)
        {
            selectIndex = -1;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
