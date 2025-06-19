using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : SingletonComponent<SoundManager>
{
    #region Classes

    [System.Serializable]
    private class SoundInfo
    {
        public string id = string.Empty;
        public AudioClip audioClip;
        public SoundType type = SoundType.SoundEffect;
        public bool playAndLoopOnStart;

        [Range(0, 1)] public float clipVolume = 1;
    }

    private class PlayingSound
    {
        public SoundInfo SoundInfo;
        public AudioSource AudioSource;
    }

    #endregion

    #region Enums

    internal enum SoundType
    {
        SoundEffect,
        Music
    }

    #endregion

    #region Inspector Variables

    [SerializeField] private List<SoundInfo> soundInfos;

    #endregion

    #region Member Variables

    private readonly List<PlayingSound> _playingAudioSources = new();
    private readonly List<PlayingSound> _loopingAudioSources = new();

    #endregion

    #region Properties

    internal static bool IsMusicOn
    {
        get => PlayerPrefs.GetInt("Music", 1) == 1;
        set => PlayerPrefs.SetInt("Music", value ? 1 : 0);
    }

    internal static bool IsSoundOn
    {
        get => PlayerPrefs.GetInt("Sound", 1) == 1;
        set => PlayerPrefs.SetInt("Sound", value ? 1 : 0);
    }

    internal static bool IsVibrationOn
    {
        get => PlayerPrefs.GetInt("Vibration", 1) == 1;
        set => PlayerPrefs.SetInt("Vibration", value ? 1 : 0);
    }

    #endregion

    #region Unity Methods

    private void Start()
    {
        foreach (var soundInfo in soundInfos.Where(soundInfo => soundInfo.playAndLoopOnStart))
        {
            Play(soundInfo.id, true);
        }
    }

    private void Update()
    {
        for (var i = 0; i < _playingAudioSources.Count; i++)
        {
            var audioSource = _playingAudioSources[i].AudioSource;
            if (audioSource.isPlaying) continue;
            Destroy(audioSource.gameObject);
            _playingAudioSources.RemoveAt(i);
            i--;
        }
    }

    #endregion

    #region Public Methods

    public void Play(string id, bool loop = false, float playDelay = 0)
    {
        if (id == string.Empty) return;

        var soundInfo = GetSoundInfo(id);
        if (soundInfo == null) return;
        if ((soundInfo.type == SoundType.Music && !IsMusicOn) ||
            (soundInfo.type == SoundType.SoundEffect && !IsSoundOn)) return;

        var audioSource = CreateAudioSource(id);

        audioSource.clip = soundInfo.audioClip;
        audioSource.loop = loop;
        audioSource.time = 0;
        audioSource.volume = soundInfo.clipVolume;

        if (playDelay > 0) audioSource.PlayDelayed(playDelay);
        else audioSource.Play();

        var playingSound = new PlayingSound
        {
            SoundInfo = soundInfo,
            AudioSource = audioSource
        };

        if (loop) _loopingAudioSources.Add(playingSound);
        else _playingAudioSources.Add(playingSound);
    }

    internal void Stop(string id)
    {
        StopAllSounds(id, _playingAudioSources);
        StopAllSounds(id, _loopingAudioSources);
    }

    internal void Stop(SoundType type)
    {
        StopAllSounds(type, _playingAudioSources);
        StopAllSounds(type, _loopingAudioSources);
    }

    internal void SetSoundTypeOnOff(SoundType type, bool isOn)
    {
        switch (type)
        {
            case SoundType.SoundEffect:

                if (isOn == IsSoundOn)
                {
                    return;
                }

                IsSoundOn = isOn;

                break;
            case SoundType.Music:

                if (isOn == IsMusicOn)
                {
                    return;
                }

                IsMusicOn = isOn;

                break;
            default:
                return;
        }

        if (!isOn) Stop(type);
        else PlayAtStart(type);
    }

    internal void PlayMusic(bool play)
    {
        if (!play)
        {
            if (IsMusicOn) Stop(SoundType.Music);
        }
        else if (IsMusicOn && _loopingAudioSources.Count == 0) PlayAtStart(SoundType.Music);
    }

    internal void LightVibrate()
    {
#if UNITY_ANDROID
        if (IsVibrationOn)
            Vibration.VibrateAndroid(20);
#else
        if (IsVibrationOn)
            Vibration.VibrateIOS(ImpactFeedbackStyle.Soft);
#endif
    }
    #endregion

    #region Private Methods

    private void PlayAtStart(SoundType type)
    {
        foreach (var soundInfo in soundInfos.Where(soundInfo => soundInfo.type == type && soundInfo.playAndLoopOnStart))
        {
            Play(soundInfo.id, true);
        }
    }

    private static void StopAllSounds(string id, List<PlayingSound> playingSounds)
    {
        for (var i = 0; i < playingSounds.Count; i++)
        {
            var playingSound = playingSounds[i];
            if (id != playingSound.SoundInfo.id) continue;
            playingSound.AudioSource.Stop();
            Destroy(playingSound.AudioSource.gameObject);
            playingSounds.RemoveAt(i);
            i--;
        }
    }

    private static void StopAllSounds(SoundType type, List<PlayingSound> playingSounds)
    {
        for (var i = 0; i < playingSounds.Count; i++)
        {
            var playingSound = playingSounds[i];

            if (type != playingSound.SoundInfo.type) continue;
            playingSound.AudioSource.Stop();
            Destroy(playingSound.AudioSource.gameObject);
            playingSounds.RemoveAt(i);
            i--;
        }
    }

    private SoundInfo GetSoundInfo(string id)
    {
        return soundInfos.FirstOrDefault(t => id == t.id);
    }

    private AudioSource CreateAudioSource(string id)
    {
        var obj = new GameObject("sound_" + id);
        obj.transform.SetParent(transform);
        return obj.AddComponent<AudioSource>();
    }

    internal bool Is_Playing_Music(string soundName)
    {
        return _loopingAudioSources.Find(x => x.SoundInfo.id == soundName).AudioSource.isPlaying;
    }

    #endregion
}