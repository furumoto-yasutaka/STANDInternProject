/*******************************************************************************
*
*	タイトル：	サウンド管理	[ AudioManager.cs ]
*
*	作成者：	古本 泰隆
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    // 起動中使用する
    [System.Serializable]
    public class AudioInfo
    {
        // オーディオファイル
        public AudioClip Clip = null;

        // ボリューム
        public float Volume = 1.0f;

        public AudioInfo(AudioClip clip, float volume)
        {
            Clip = clip;
            Volume = volume;
        }
    }

    // インスペクターからの入力専用
    [System.Serializable]
    public class AudioInfoInspector
    {
        // 音名(ファイル名とは別)
        public string Name = "";

        // オーディオファイル
        public AudioClip Clip = null;

        // ボリューム
        public float Volume = 1.0f;
    }

    [SerializeField, RenameField("BGM音量"), Range(0, 1)]
    private float BgmVolume = 1.0f;

    [SerializeField, RenameField("SE音量"), Range(0, 1)]
    public float SeVolume = 1.0f;

    [SerializeField, RenameField("ミュート")]
    private bool isMute = false;

    // スピーカー
    private AudioSource bgmSource;
    private AudioSource seSource;

    // オーディオデータ
    // キー   string型     音名(ファイル名とは別)
    // 要素   AudioInfo型  音情報
    private Dictionary<string, AudioInfo> bgmInfo = new Dictionary<string, AudioInfo>();
    private Dictionary<string, AudioInfo> seInfo = new Dictionary<string, AudioInfo>();

    // インスペクター入力用リスト
    [SerializeField] private List<AudioInfoInspector> bgmInfoInspector;
    [SerializeField] private List<AudioInfoInspector> seInfoInspector;

    protected override void Awake()
    {
        base.Awake();

        bgmSource = transform.GetChild(0).GetComponent<AudioSource>();
        seSource = transform.GetChild(1).GetComponent<AudioSource>();

        bgmSource.volume = BgmVolume;

        // インスペクターに入力された情報を参照に特化したDictionaryに加工する
        foreach (AudioInfoInspector info in bgmInfoInspector)
        {
            bgmInfo.Add(info.Name, new AudioInfo(info.Clip, info.Volume));
        }
        foreach (AudioInfoInspector info in seInfoInspector)
        {
            seInfo.Add(info.Name, new AudioInfo(info.Clip, info.Volume));
        }

        // リストはもう使わないので解放
        bgmInfoInspector.Clear();
        seInfoInspector.Clear();

        // ミュート設定の確認
        if (isMute)
        {
            bgmSource.mute = isMute;
            seSource.mute = isMute;
        }
    }

    public bool IsMute
    {
        get { return isMute; }

        set
        {
            isMute = value;

            bgmSource.mute = value;
            seSource.mute = value;
        }
    }

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="name">音名(Dictionaryのキー情報)</param>
    /// <param name="loop">ループするか</param>
    public void PlayBgm(string name, bool loop)
    {
        if (!bgmInfo.ContainsKey(name)) { return; }
        if (bgmSource.clip != null)
        {
            if (bgmSource.clip == bgmInfo[name].Clip && bgmSource.isPlaying) { return; }
        }

        AudioInfo info = bgmInfo[name];

        bgmSource.clip = info.Clip;
        bgmSource.loop = loop;
        bgmSource.volume = BgmVolume * info.Volume;
        bgmSource.Play();
    }

    /// <summary>
    /// BGM停止
    /// </summary>
    public void StopBgm()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    /// <summary>
    /// BGM一時停止
    /// </summary>
    public void PauseBgm()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Pause();
        }
    }

    /// <summary>
    /// BGM再生再開
    /// </summary>
    public void ResumeBgm()
    {
        if (bgmSource.clip != null)
        {
            bgmSource.Play();
        }
    }

    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="name">音名(Dictionaryのキー情報)</param>
    public void PlaySe(string name)
    {
        if (!seInfo.ContainsKey(name)) { return; }

        AudioInfo info = seInfo[name];

        seSource.PlayOneShot(info.Clip, SeVolume * info.Volume);
    }

    /// <summary>
    /// すべてのSEを停止
    /// </summary>
    public void StopSeAll()
    {
        if (seSource.isPlaying)
        {
            seSource.Stop();
        }
    }
}
