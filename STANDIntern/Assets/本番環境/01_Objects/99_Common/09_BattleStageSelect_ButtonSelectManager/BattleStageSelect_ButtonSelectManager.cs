using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStageSelect_ButtonSelectManager : ButtonSelectManager
{
    private Vector2 movePos = Vector2.zero;
    private Vector2 moveTarget;
    [SerializeField]
    private float moveSpeed = 0.1f;

    protected override void Update()
    {
        base.Update();

        Vector2 move = (moveTarget - movePos) * moveSpeed;
        transform.localPosition -= (Vector3)move;
        movePos += move;
    }

    public override void Select(int index)
    {
        base.Select(index);

        moveTarget = selecters[selectIndex].transform.GetComponent<RectTransform>().localPosition;
    }
}
