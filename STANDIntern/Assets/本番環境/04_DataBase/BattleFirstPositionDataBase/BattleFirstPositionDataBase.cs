using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleFirstPositionDataBase", menuName = "CreateBattleFirstPositionDataBase")]
public class BattleFirstPositionDataBase : ScriptableObject
{
    public BattleStageInfo[] BattleStageInfos;
}

[System.Serializable]
public class BattleStageInfo
{
    public FirstPosition[] FirstPositions;
    public float JumpRerativeHeight;
}

[System.Serializable]
public struct FirstPosition
{
    public Vector2[] Position;
}
