/*******************************************************************************
*
*	�^�C�g���F	�Z�[�u�f�[�^�Ǘ��N���X	[ SaveDataManager.cs ]
*
*	�쐬�ҁF	�Ö{ �ח�
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

public struct StageRecordInfo
{
    public bool IsClear;                    // �X�e�[�W�̃N���A��
    public bool[] IsGetCollectItems;        // ���W�v�f�̎擾��
    public uint MaxScore_ClearTimeMsec;      // �ō��X�R�A���̃N���A�^�C��
    public uint MaxScore_GetCollectItemNum;  // �ō��X�R�A���̎��W�A�C�e���擾��
    public uint MaxScore_DeathNum;           // �ō��X�R�A���̎��񂾉�
    public uint MaxScore;                    // �ō��X�R�A
    public uint MaxScore_Evaluation;         // �ō��X�R�A���̕]��
}

public class SaveDataManager : SingletonMonoBehaviour<SaveDataManager>
{
    // �Z�[�u�f�[�^�p�\����
    public struct SaveDataInfo
    {
        // 1�l�v���C
        public bool IsSinglePlayAllClear;
        public StageRecordInfo[] SinglePlayRecord;

        // 2�l���̓v���C
        public bool IsCoopPlayAllClear;
        public StageRecordInfo[] CoopPlayRecord;
    }

    // �Z�[�u�f�[�^�̒��g
    private SaveDataInfo saveData;

    // �Z�[�u�f�[�^�̃t�@�C����
    private string FileName = "SaveData";

    // �Z�[�u�f�[�^�̃t���p�X
    private string FullPath;

    // �X�e�[�W���
    [SerializeField]
    private StageDataBase singlePlayStageDataBase;
    [SerializeField]
    private StageDataBase coopPlayStageDataBase;

    public SaveDataInfo SaveData
    {
        get { return saveData; }
    }

    protected override void Awake()
    {
        base.Awake();

        FullPath = Application.persistentDataPath + "/" + FileName + ".json";

        // �Z�[�u�f�[�^�̗L��
        if (File.Exists(FullPath))
        {// ���݂����烍�[�h
            Load();
        }
        else
        {// ���݂��Ă��Ȃ�������f�t�H���g�l�ŏ��������ăf�B���N�g�����쐬
            InitInfos();
            CreateDirectory();
            Save();
        }
    }

    /// <summary>
    /// ���[�h����
    /// </summary>
    public void Load()
    {
        // �t�@�C���ǂݍ���
        StreamReader reader = new StreamReader(FullPath);
        string fileData = reader.ReadToEnd();
        reader.Close();

        // ��������Z�[�u�f�[�^�\���̔z��ɕϊ����Ċi�[
        saveData = JsonConvert.DeserializeObject<SaveDataInfo>(fileData);
    }

    /// <summary>
    /// �Z�[�u����
    /// </summary>
    public void Save()
    {
        // json�t�@�C���̐ݒ�
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;  // �z�Q�Ƃ���I�u�W�F�N�g�̌x���𖳎�����
        settings.Formatting = Formatting.Indented;                      // ���s����
        settings.Converters.Add(new StringEnumConverter(new DefaultNamingStrategy()));  // enum�ŕ�������L��

        // �f�[�^��json�`���ɕϊ�
        string data = JsonConvert.SerializeObject(saveData, settings);

        // �t�@�C����������
        StreamWriter writer = new StreamWriter(FullPath, false);
        writer.WriteLine(data);
        writer.Flush();
        writer.Close();
    }

    /// <summary>
    /// �Z�[�u�f�[�^����������
    /// </summary>
    private void InitInfos()
    {
        // 1�l�v���C
        saveData.IsSinglePlayAllClear = false;
        saveData.SinglePlayRecord = new StageRecordInfo[singlePlayStageDataBase.StageInfos.Length];
        for (int i = 0; i < singlePlayStageDataBase.StageInfos.Length; i++)
        {
            int collectItemNum = singlePlayStageDataBase.StageInfos[i].CollectItemNum;
            saveData.SinglePlayRecord[i].IsClear = false;
            saveData.SinglePlayRecord[i].IsGetCollectItems = new bool[collectItemNum];
            for (int j = 0; j < collectItemNum; j++)
            {
                saveData.SinglePlayRecord[i].IsGetCollectItems[j] = false;
            }
            saveData.SinglePlayRecord[i].MaxScore_ClearTimeMsec = 0;
            saveData.SinglePlayRecord[i].MaxScore_GetCollectItemNum = 0;
            saveData.SinglePlayRecord[i].MaxScore_DeathNum = 0;
            saveData.SinglePlayRecord[i].MaxScore = 0;
            saveData.SinglePlayRecord[i].MaxScore_Evaluation = 0;
        }

        // 2�l���̓v���C
        saveData.IsCoopPlayAllClear = false;
        saveData.CoopPlayRecord = new StageRecordInfo[coopPlayStageDataBase.StageInfos.Length];
        for (int i = 0; i < coopPlayStageDataBase.StageInfos.Length; i++)
        {
            int collectItemNum = coopPlayStageDataBase.StageInfos[i].CollectItemNum;
            saveData.CoopPlayRecord[i].IsClear = false;
            saveData.CoopPlayRecord[i].IsGetCollectItems = new bool[collectItemNum];
            for (int j = 0; j < collectItemNum; j++)
            {
                saveData.CoopPlayRecord[i].IsGetCollectItems[j] = false;
            }
            saveData.CoopPlayRecord[i].MaxScore_ClearTimeMsec = 0;
            saveData.CoopPlayRecord[i].MaxScore_GetCollectItemNum = 0;
            saveData.CoopPlayRecord[i].MaxScore_DeathNum = 0;
            saveData.CoopPlayRecord[i].MaxScore = 0;
            saveData.CoopPlayRecord[i].MaxScore_Evaluation = 0;
        }
    }

    /// <summary>
    /// �Z�[�u�f�[�^�����\��p�X�܂ł̃t�H���_�𐶐�
    /// </summary>
    private void CreateDirectory()
    {
        // �f�B���N�g��������ŏ�w�̃f�B���N�g�����𔲂����p�X���擾
        string remainPath = Path.GetDirectoryName(FullPath) + "/";
        // �m�F�Ώۂ̃t�H���_
        string searchDirectoryPath = "";
        // �����Ɏg�p����f�B���N�g����
        string directoryName;

        // �S�Ẵp�X���m�F����܂�
        while (remainPath != "")
        {
            // ���̃t�H���_�̃p�X��ݒ�
            int index = GetIndexOfSlash(remainPath);
            directoryName = remainPath.Remove(index);
            searchDirectoryPath += "/" + directoryName;
            remainPath = remainPath.Remove(0, index + 1);

            // �t�H���_�����݂��Ȃ�������V������������
            if (!Directory.Exists(searchDirectoryPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
                directoryInfo.Create();
            }
        }
    }

    /// <summary>
    /// �X���b�V����������̉������ڂɂ��邩�擾����
    /// </summary>
    /// <param name="path">�T���Ώۂ̃p�X</param>
    /// <returns>���������ꍇ�͉������ڂ���Ԃ��A������Ȃ������當����̕�������Ԃ�</returns>
    private int GetIndexOfSlash(string path)
    {
        int index = path.IndexOf("/");

        if (index != -1)
        {
            return index;
        }
        else
        {
            return path.Length;
        }
    }

    public void Delete()
    {
        InitInfos();
        Save();
    }
}
