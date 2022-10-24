using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleStageSelectButtonDataBase", menuName = "CreateBattleStageSelectButtonDataBase")]
public class BattleStageSelectButtonDataBase : ScriptableObject
{
    public Vector2 ButtonSpace;
    public string StageTextInitial;
    public BattleStageSelectButtonInfo[] ButtonInfo;
}

[System.Serializable]
public struct BattleStageSelectButtonInfo
{
    public string Comment;
    public Sprite ButtonBackSprite;
}
