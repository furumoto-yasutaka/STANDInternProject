using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSelectManager : InputLockElement
{
    private PointSelector[] selectors;
    private int selectIndex = 0;
    [SerializeField]
    private RectTransform playerTrans;
    private RectTransform pointsParent;
    [SerializeField]
    private RectTransform linesParent;

    private Vector2 movePos = Vector2.zero;
    private Vector2 moveTarget = Vector2.zero;
    [SerializeField]
    private float moveSpeed = 0.2f;

    public int SelectIndex { get { return selectIndex; } }

    void Start()
    {
        selectors = new PointSelector[transform.childCount];
        pointsParent = GetComponent<RectTransform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform trans = transform.GetChild(i);
            selectors[i] = trans.GetComponent<PointSelector>();
            selectors[i].Index = i;
        }

        if (transform.childCount == 0)
        {
            LockInput();
        }
    }

    void Update()
    {
        if (IsCanInput)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Select((selectIndex - 1 + transform.childCount) % transform.childCount);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                int num = (selectIndex + 1) % transform.childCount;
                Select(num);
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Decition();
            }
        }

        Vector2 move = (moveTarget - movePos) * moveSpeed;
        pointsParent.localPosition -= (Vector3)move;
        linesParent.localPosition -= (Vector3)move;
        movePos += move;
    }

    public void Select(int index)
    {
        selectIndex = index;
        moveTarget = selectors[selectIndex].transform.GetComponent<RectTransform>().localPosition;
    }

    public void Unselect(int index)
    {
        if (index == selectIndex)
        {
            selectIndex = -1;
        }
    }

    public void Decition()
    {
        GetComponent<SceneChange_AtStage>().StartSceneChange(selectIndex + 1);
    }
}
