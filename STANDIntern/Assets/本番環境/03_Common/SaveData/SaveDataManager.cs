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

public class SaveDataManager : SingletonMonoBehaviour<SaveDataManager>
{
    // セーブデータ用構造体
    public struct SaveDataInfo
    {
        // 正答率
        public int CorrectAnswerCount;
    }

    // ステージ数
    public static readonly int stageNumMax = 4;

    // セーブデータの中身
    private static SaveDataInfo[] saveDataInfos = new SaveDataInfo[stageNumMax];

    // セーブデータのファイル名
    private static string FileName = "SaveData";

    // セーブデータのフルパス
    private static string FullPath;

    public static SaveDataInfo[] SaveDataInfos
    {
        get { return saveDataInfos; }
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
    public static void Load()
    {
        // ファイル読み込み
        StreamReader reader = new StreamReader(FullPath);
        string fileData = reader.ReadToEnd();
        reader.Close();

        // 文字列をセーブデータ構造体配列に変換して格納
        saveDataInfos = JsonConvert.DeserializeObject<SaveDataInfo[]>(fileData);
    }

    /// <summary>
    /// セーブ処理
    /// </summary>
    public static void Save()
    {
        // jsonファイルの設定
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;  // 循環参照するオブジェクトの警告を無視する
        settings.Formatting = Formatting.Indented;                      // 改行あり
        settings.Converters.Add(new StringEnumConverter(new DefaultNamingStrategy()));  // enumで文字列を記載

        // データをjson形式に変換
        string data = JsonConvert.SerializeObject(saveDataInfos, settings);

        // ファイル書き込み
        StreamWriter writer = new StreamWriter(FullPath, false);
        writer.WriteLine(data);
        writer.Flush();
        writer.Close();
    }

    /// <summary>
    /// セーブデータ情報を初期化
    /// </summary>
    private static void InitInfos()
    {
        for (int i = 0; i < stageNumMax; i++)
        {
            saveDataInfos[i].CorrectAnswerCount = -1;
        }
    }

    /// <summary>
    /// セーブデータ生成予定パスまでのフォルダを生成
    /// </summary>
    private static void CreateDirectory()
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
