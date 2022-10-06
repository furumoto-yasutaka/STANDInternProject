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

public class SaveDataManager : SingletonMonoBehaviour<SaveDataManager>
{
    // �Z�[�u�f�[�^�p�\����
    public struct SaveDataInfo
    {
        // ������
        public int CorrectAnswerCount;
    }

    // �X�e�[�W��
    public static readonly int stageNumMax = 4;

    // �Z�[�u�f�[�^�̒��g
    private static SaveDataInfo[] saveDataInfos = new SaveDataInfo[stageNumMax];

    // �Z�[�u�f�[�^�̃t�@�C����
    private static string FileName = "SaveData";

    // �Z�[�u�f�[�^�̃t���p�X
    private static string FullPath;

    public static SaveDataInfo[] SaveDataInfos
    {
        get { return saveDataInfos; }
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
    public static void Load()
    {
        // �t�@�C���ǂݍ���
        StreamReader reader = new StreamReader(FullPath);
        string fileData = reader.ReadToEnd();
        reader.Close();

        // ��������Z�[�u�f�[�^�\���̔z��ɕϊ����Ċi�[
        saveDataInfos = JsonConvert.DeserializeObject<SaveDataInfo[]>(fileData);
    }

    /// <summary>
    /// �Z�[�u����
    /// </summary>
    public static void Save()
    {
        // json�t�@�C���̐ݒ�
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;  // �z�Q�Ƃ���I�u�W�F�N�g�̌x���𖳎�����
        settings.Formatting = Formatting.Indented;                      // ���s����
        settings.Converters.Add(new StringEnumConverter(new DefaultNamingStrategy()));  // enum�ŕ�������L��

        // �f�[�^��json�`���ɕϊ�
        string data = JsonConvert.SerializeObject(saveDataInfos, settings);

        // �t�@�C����������
        StreamWriter writer = new StreamWriter(FullPath, false);
        writer.WriteLine(data);
        writer.Flush();
        writer.Close();
    }

    /// <summary>
    /// �Z�[�u�f�[�^����������
    /// </summary>
    private static void InitInfos()
    {
        for (int i = 0; i < stageNumMax; i++)
        {
            saveDataInfos[i].CorrectAnswerCount = -1;
        }
    }

    /// <summary>
    /// �Z�[�u�f�[�^�����\��p�X�܂ł̃t�H���_�𐶐�
    /// </summary>
    private static void CreateDirectory()
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
    private static int GetIndexOfSlash(string path)
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
}
