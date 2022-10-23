using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDataBase", menuName = "CreateStageDataBase")]
public class StageDataBase : ScriptableObject
{
    public StageInfo[] StageInfos;
}

[System.Serializable]
public struct StageInfo
{
    public string StageName;    // ステージ名
    public int CollectItemNum;  // 収集要素の数
    public Vector2 StageSelectDrawPos; // ステージセレクト画面での表示位置
}
