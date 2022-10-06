using System;
using System.IO;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// シーン名を管理する Enum を作成するエディター拡張
/// </summary>
public static class SceneNameEnumCreator
{
    // 無効な文字を管理する配列
    private static readonly string[] INVALUD_CHARS =
    {
        " ", "!", "\"", "#", "$",
        "%", "&", "\'", "(", ")",
        "-", "=", "^",  "~", "\\",
        "|", "[", "{",  "@", "`",
        "]", "}", ":",  "*", ";",
        "+", "/", "?",  ".", ">",
        "<", ",",
    };

    // コマンド名
    private const string ITEM_NAME = "Tools/Create/Scene Name Enum";

    // ファイルパス
    private const string PATH = "Assets/90_Other/SceneName/SceneNameEnum.cs";

    // ファイル名(拡張子あり)
    private static readonly string FILENAME = Path.GetFileName(PATH);

    // ファイル名(拡張子なし)
    private static readonly string FILENAME_WITHOUT_EXTENSION
        = Path.GetFileNameWithoutExtension(PATH);
#if UNITY_EDITOR
    /// <summary>
    /// シーン名を管理する Enum を作成します
    /// </summary>
    [MenuItem(ITEM_NAME)]
    public static void Create()
    {
        if (!CanCreate()) return;

        CreateScript();

        EditorUtility.DisplayDialog(FILENAME, "作成が完了しました", "OK");
    }

    /// <summary>
    /// スクリプトを作成します
    /// </summary>
    public static void CreateScript()
    {
        var builder = new StringBuilder();

        builder
            .AppendLine("/// <summary>")
            .AppendLine("/// シーン名を管理する Enum")
            .AppendLine("/// </summary>")
            .AppendLine($"public enum {FILENAME_WITHOUT_EXTENSION}")
            .AppendLine("{");

        foreach (var n in EditorBuildSettings.scenes
            .Select(c => Path.GetFileNameWithoutExtension(c.path))
            .Distinct()
            .Select(c => new { var = RemoveInvalidChars(c), val = c }))
        {
            builder.AppendLine($"\t{n.var},");
        }

        builder.AppendLine("}");

        var directoryName = Path.GetDirectoryName(PATH);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        File.WriteAllText(PATH, builder.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
    }

    /// <summary>
    /// シーン名を管理する Enum を作成できるかどうかを取得します
    /// </summary>
    [MenuItem(ITEM_NAME, true)]
    public static bool CanCreate()
        => !EditorApplication.isPlaying
            && !Application.isPlaying
            && !EditorApplication.isCompiling;

    /// <summary>
    /// 無効な文字を削除します
    /// </summary>
    public static string RemoveInvalidChars(string str)
    {
        Array.ForEach(INVALUD_CHARS, c => str = str.Replace(c, string.Empty));
        return str;
    }
#endif
}