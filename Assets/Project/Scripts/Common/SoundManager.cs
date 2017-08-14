using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class SoundVolume
{
    public float bgm = 0.5f;
    public float se = 0.4f;
    public float voice = 1.0f;

    public bool mute = false;

    public void Reset()
    {
        bgm = 0.5f;
        se = 0.4f;
        voice = 1.0f;
        mute = false;
    }

    public float defaultVolume_Bgm{
        get { return 0.5f; }
    }
}

public class SoundManager : SingletonBehaviour<SoundManager> {

    public SoundVolume volume = new SoundVolume();

    private AudioClip[] m_seClips;
    private AudioClip[] m_bgmClips;
    private AudioClip[] m_voiceClips;

    private Dictionary<string, int> m_seIndexes = new Dictionary<string, int>();
    private Dictionary<string, int> m_bgmIndexes = new Dictionary<string, int>();
    private Dictionary<string, int> m_voiceIndexes = new Dictionary<string, int>();

    const int m_cNumChannel = 16;
    private AudioSource m_voiceSource;
    private AudioSource m_bgmSource;
    private AudioSource[] m_seSources = new AudioSource[m_cNumChannel];

    Queue<int> m_seRequestQueue = new Queue<int>();

    //------------------------------------------------------------------------------
    private void Start()
    {
        // Debug.logger.logEnabled = false;

        m_bgmSource = gameObject.AddComponent<AudioSource>();
        m_bgmSource.loop = true;

        m_voiceSource = gameObject.AddComponent<AudioSource>();
        m_voiceSource.loop = false;

        for (int i = 0; i < m_seSources.Length; i++)
        {
            m_seSources[i] = gameObject.AddComponent<AudioSource>();
        }

        m_seClips = Resources.LoadAll<AudioClip>("Audio/SE");
        m_bgmClips = Resources.LoadAll<AudioClip>("Audio/BGM");
        m_voiceClips = Resources.LoadAll<AudioClip>("Audio/VOICE");

        for (int i = 0; i < m_seClips.Length; ++i)
        {
            m_seIndexes[m_seClips[i].name] = i;
        }

        for (int i = 0; i < m_bgmClips.Length; ++i)
        {
            m_bgmIndexes[m_bgmClips[i].name] = i;
        }

        for (int i = 0; i < m_voiceClips.Length; i++)
        {
            m_voiceIndexes[m_voiceClips[i].name] = i;
        }


        foreach (var source in m_seSources)
        {
            source.volume = volume.se;
        }
    }

    //------------------------------------------------------------------------------
    void Update()
    {
        m_bgmSource.mute = volume.mute;
        m_voiceSource.mute = volume.mute;
        foreach (var source in m_seSources)
        {
            source.mute = volume.mute;
        }

        m_bgmSource.volume = volume.bgm;
        m_voiceSource.volume = volume.voice;

        int count = m_seRequestQueue.Count;
        if (count != 0)
        {
            int sound_index = m_seRequestQueue.Dequeue();
            playSeImpl(sound_index);
        }
    }

    //------------------------------------------------------------------------------
    private void playSeImpl(int index)
    {
        if (0 > index || m_seClips.Length <= index)
        {
            return;
        }

        foreach (AudioSource source in m_seSources)
        {
            if (false == source.isPlaying)
            {
                source.clip = m_seClips[index];
                source.Play();
                return;
            }
        }
    }

    //------------------------------------------------------------------------------
    public void ChangeSeVolume(string name, float volume)
    {
        int index = GetSeIndex(name);

        foreach (AudioSource source in m_seSources)
        {
            if (source.clip = m_seClips[index])
            {
                source.volume = volume;
            }
        }

    }

    //------------------------------------------------------------------------------
    public int GetSeIndex(string name)
    {
        return m_seIndexes[name];
    }

    //------------------------------------------------------------------------------
    public int GetBgmIndex(string name)
    {
        return m_bgmIndexes[name];
    }

    //------------------------------------------------------------------------------
    public int GetVoiceIndex(string name)
    {
        return m_voiceIndexes[name];
    }

    //------------------------------------------------------------------------------
    public void PlayBgm(string name)
    {
        int index = m_bgmIndexes[name];
        PlayBgm(index);
    }

    //------------------------------------------------------------------------------
    public void PlayBgm(int index)
    {
        if (0 > index || m_bgmClips.Length <= index)
        {
            return;
        }

        if (m_bgmSource.clip == m_bgmClips[index])
        {
            return;
        }

        m_bgmSource.Stop();
        m_bgmSource.clip = m_bgmClips[index];
        m_bgmSource.Play();
    }

    //------------------------------------------------------------------------------
    public void StopBgm()
    {
        m_bgmSource.Stop();
        m_bgmSource.clip = null;
    }

    //------------------------------------------------------------------------------
    public bool IsPlayBgm()
    {
        return m_bgmSource.isPlaying;
    }

    //------------------------------------------------------------------------------
    public bool IsPlaySe(string name)
    {
        bool isplaying = false;
        int index = GetSeIndex(name);

        foreach (AudioSource source in m_seSources)
        {
            if (source.clip == m_seClips[index])
            {
                if (source.isPlaying)
                {
                    isplaying = true;
                }
            }
        }

        return isplaying;
    }


    //------------------------------------------------------------------------------
    public void PlayVoice(string name)
    {
        int index = m_voiceIndexes[name];
        PlayVoice(index);
    }

    //------------------------------------------------------------------------------
    public void PlayVoice(int index)
    {
        if (0 > index || m_voiceClips.Length <= index)
        {
            return;
        }

        if (m_voiceSource.clip == m_voiceClips[index])
        {
            return;
        }

        m_voiceSource.Stop();
        m_voiceSource.clip = m_voiceClips[index];
        m_voiceSource.Play();
    }

    //------------------------------------------------------------------------------
    public void StopVoice()
    {
        m_voiceSource.Stop();
        m_voiceSource.clip = null;
    }

    //------------------------------------------------------------------------------
    public bool IsPlayVoice()
    {
        return m_voiceSource.isPlaying;
    }
    //------------------------------------------------------------------------------
    private int _voicesLength = 0;
    private int _voicesIndex = 0;
    private string[] _voices;
    private Coroutine _coroutine = null;
    public void PlayVoices(string[] voices, bool reset = true)
    {
        IsPlayVoices = true;            // 再生中フラグを立てる
        if (reset)
        {
            _voicesIndex = 0;
            _voices = voices;               // 一時保存
            _voicesLength = _voices.Length; // 長さの保存       
        }
        _coroutine = StartCoroutine(playVoices(_voices[_voicesIndex]));
    }
    private IEnumerator playVoices(string voice)
    {
        PlayVoice(voice);
        yield return new WaitWhile(() => IsPlayVoice()); // ボイスの再生が終わるまで待機
        _voicesIndex++;                            // 終わったら番号をインクリメント
        if (_voicesLength <= _voicesIndex)          // 最後まで再生していたら
        {
            IsPlayVoices = false;                        // 再生終了フラグを立てる
            _voicesIndex = 0;
            _voices = null;
            yield break;
        }
        else
        {
            _coroutine = StartCoroutine(playVoices(_voices[_voicesIndex]));    // 終わってない場合は次のボイス再生
        }
    }
    public void StopVoices()
    {
        StopCoroutine(_coroutine);
        StopVoice();
    }
    public bool IsPlayVoices { get; private set; }

    //------------------------------------------------------------------------------
    public void PlaySe(string name)
    {
        PlaySe(name, volume.se);
    }
    public void PlaySe(string name, float volume)
    {
        PlaySe(GetSeIndex(name));
        ChangeSeVolume(name, volume);
    }

    //一旦queueに溜め込んで重複を回避しているので
    //再生が1frame遅れる時がある
    //------------------------------------------------------------------------------
    public void PlaySe(int index)
    {
        if (!m_seRequestQueue.Contains(index))
        {
            m_seRequestQueue.Enqueue(index);
        }
    }

    //------------------------------------------------------------------------------
    public void StopSe()
    {
        ClearAllSeRequest();
        foreach (AudioSource source in m_seSources)
        {
            source.Stop();
            source.clip = null;
        }
    }
    public void StopSe(string name)
    {
        StopSe(GetSeIndex(name));
    }

    public void StopSe(int index)
    {
        if (0 > index || m_seClips.Length <= index)
        {
            return;
        }

        foreach (AudioSource source in m_seSources)
        {
            if (source.clip == m_seClips[index])
            {
                source.Stop();
            }
        }
    }
    //------------------------------------------------------------------------------
    public void ClearAllSeRequest()
    {
        m_seRequestQueue.Clear();
    }

    public void FadeBgm(float volume, float duration)
    {
        DOTween.To(() => this.volume.bgm, Volume => this.volume.bgm = Volume, volume, duration);
    }
}
