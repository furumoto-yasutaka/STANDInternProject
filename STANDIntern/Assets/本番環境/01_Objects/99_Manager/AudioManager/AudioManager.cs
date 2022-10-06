/*******************************************************************************
*
*	�^�C�g���F	�T�E���h�Ǘ�	[ AudioManager.cs ]
*
*	�쐬�ҁF	�Ö{ �ח�
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    // �N�����g�p����
    [System.Serializable]
    public class AudioInfo
    {
        // �I�[�f�B�I�t�@�C��
        public AudioClip Clip = null;

        // �{�����[��
        public float Volume = 1.0f;

        public AudioInfo(AudioClip clip, float volume)
        {
            Clip = clip;
            Volume = volume;
        }
    }

    // �C���X�y�N�^�[����̓��͐�p
    [System.Serializable]
    public class AudioInfoInspector
    {
        // ����(�t�@�C�����Ƃ͕�)
        public string Name = "";

        // �I�[�f�B�I�t�@�C��
        public AudioClip Clip = null;

        // �{�����[��
        public float Volume = 1.0f;
    }

    [SerializeField, RenameField("BGM����"), Range(0, 1)]
    private float BgmVolume = 1.0f;

    [SerializeField, RenameField("SE����"), Range(0, 1)]
    public float SeVolume = 1.0f;

    [SerializeField, RenameField("�~���[�g")]
    private bool isMute = false;

    // �X�s�[�J�[
    private AudioSource bgmSource;
    private AudioSource seSource;

    // �I�[�f�B�I�f�[�^
    // �L�[   string�^     ����(�t�@�C�����Ƃ͕�)
    // �v�f   AudioInfo�^  �����
    private Dictionary<string, AudioInfo> bgmInfo = new Dictionary<string, AudioInfo>();
    private Dictionary<string, AudioInfo> seInfo = new Dictionary<string, AudioInfo>();

    // �C���X�y�N�^�[���͗p���X�g
    [SerializeField] private List<AudioInfoInspector> bgmInfoInspector;
    [SerializeField] private List<AudioInfoInspector> seInfoInspector;

    protected override void Awake()
    {
        base.Awake();

        bgmSource = transform.GetChild(0).GetComponent<AudioSource>();
        seSource = transform.GetChild(1).GetComponent<AudioSource>();

        bgmSource.volume = BgmVolume;

        // �C���X�y�N�^�[�ɓ��͂��ꂽ�����Q�Ƃɓ�������Dictionary�ɉ��H����
        foreach (AudioInfoInspector info in bgmInfoInspector)
        {
            bgmInfo.Add(info.Name, new AudioInfo(info.Clip, info.Volume));
        }
        foreach (AudioInfoInspector info in seInfoInspector)
        {
            seInfo.Add(info.Name, new AudioInfo(info.Clip, info.Volume));
        }

        // ���X�g�͂����g��Ȃ��̂ŉ��
        bgmInfoInspector.Clear();
        seInfoInspector.Clear();

        // �~���[�g�ݒ�̊m�F
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
    /// BGM�Đ�
    /// </summary>
    /// <param name="name">����(Dictionary�̃L�[���)</param>
    /// <param name="loop">���[�v���邩</param>
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
    /// BGM��~
    /// </summary>
    public void StopBgm()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    /// <summary>
    /// BGM�ꎞ��~
    /// </summary>
    public void PauseBgm()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Pause();
        }
    }

    /// <summary>
    /// BGM�Đ��ĊJ
    /// </summary>
    public void ResumeBgm()
    {
        if (bgmSource.clip != null)
        {
            bgmSource.Play();
        }
    }

    /// <summary>
    /// SE�Đ�
    /// </summary>
    /// <param name="name">����(Dictionary�̃L�[���)</param>
    public void PlaySe(string name)
    {
        if (!seInfo.ContainsKey(name)) { return; }

        AudioInfo info = seInfo[name];

        seSource.PlayOneShot(info.Clip, SeVolume * info.Volume);
    }

    /// <summary>
    /// ���ׂĂ�SE���~
    /// </summary>
    public void StopSeAll()
    {
        if (seSource.isPlaying)
        {
            seSource.Stop();
        }
    }
}
