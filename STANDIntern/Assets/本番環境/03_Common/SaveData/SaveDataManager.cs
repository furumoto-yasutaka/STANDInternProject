/*******************************************************************************
*
*	タイトル：	セーブデータ管理クラス	[ SaveDataManager.cs ]
*
*	作成者：	古本 泰隆
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
    public bool IsClear;                    // ステージのクリア状況
    public bool[] IsGetCollectItems;        // 収集要素の取得状況
    public uint MaxScore_ClearTimeMsec;      // 最高スコア時のクリアタイム
    public uint MaxScore_GetCollectItemNum;  // 最高スコア時の収集アイテム取得数
    public uint MaxScore_DeathNum;           // 最高スコア時の死んだ回数
    public uint MaxScore;                    // 最高スコア
    public uint MaxScore_Evaluation;         // 最高スコア時の評価
}

public class SaveDataManager : SingletonMonoBehaviour<SaveDataManager>
{
    // セーブデータ用構造体
    public struct SaveDataInfo
    {
        // 1人プレイ
        public bool IsSinglePlayAllClear;
        public StageRecordInfo[] SinglePlayRecord;

        // 2人協力プレイ
        public bool IsCoopPlayAllClear;
        public StageRecordInfo[] CoopPlayRecord;
    }

    // セーブデータの中身
    private SaveDataInfo saveData;

    // セーブデータのファイル名
    private string FileName = "SaveData";

    // セーブデータのフルパス
    private string FullPath;

    // ステージ情報
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

        // セーブデータの有無
        if (File.Exists(FullPath))
        {// 存在したらロード
            Load();
        }
        else
        {// 存在していなかったらデフォルト値で初期化してディレクトリを作成
            InitInfos();
            CreateDirectory();
            Save();
        }
    }

    /// <summary>
    /// ロード処理
    /// </summary>
    public void Load()
    {
        // ファイル読み込み
        StreamReader reader = new StreamReader(FullPath);
        string fileData = reader.ReadToEnd();
        reader.Close();

        // 文字列をセーブデータ構造体配列に変換して格納
        saveData = JsonConvert.DeserializeObject<SaveDataInfo>(fileData);
    }

    /// <summary>
    /// セーブ処理
    /// </summary>
    public void Save()
    {
        // jsonファイルの設定
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;  // 循環参照するオブジェクトの警告を無視する
        settings.Formatting = Formatting.Indented;                      // 改行あり
        settings.Converters.Add(new StringEnumConverter(new DefaultNamingStrategy()));  // enumで文字列を記載

        // データをjson形式に変換
        string data = JsonConvert.SerializeObject(saveData, settings);

        // ファイル書き込み
        StreamWriter writer = new StreamWriter(FullPath, false);
        writer.WriteLine(data);
        writer.Flush();
        writer.Close();
    }

    /// <summary>
    /// セーブデータ情報を初期化
    /// </summary>
    private void InitInfos()
    {
        // 1人プレイ
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

        // 2人協力プレイ
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
    /// セーブデータ生成予定パスまでのフォルダを生成
    /// </summary>
    private void CreateDirectory()
    {
        // ディレクトリ名から最上層のディレクトリ名を抜いたパスを取得
        string remainPath = Path.GetDirectoryName(FullPath) + "/";
        // 確認対象のフォルダ
        string searchDirectoryPath = "";
        // 生成に使用するディレクトリ名
        string directoryName;

        // 全てのパスを確認するまで
        while (remainPath != "")
        {
            // 次のフォルダのパスを設定
            int index = GetIndexOfSlash(remainPath);
            directoryName = remainPath.Remove(index);
            searchDirectoryPath += "/" + directoryName;
            remainPath = remainPath.Remove(0, index + 1);

            // フォルダが存在しなかったら新しく生成する
            if (!Directory.Exists(searchDirectoryPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
                directoryInfo.Create();
            }
        }
    }

    /// <summary>
    /// スラッシュが文字列の何文字目にあるか取得する
    /// </summary>
    /// <param name="path">探索対象のパス</param>
    /// <returns>見つかった場合は何文字目かを返し、見つからなかったら文字列の文字数を返す</returns>
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
