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
    public string StageName;    // �X�e�[�W��
    public int CollectItemNum;  // ���W�v�f�̐�
    public Vector2 StageSelectDrawPos; // �X�e�[�W�Z���N�g��ʂł̕\���ʒu
}
