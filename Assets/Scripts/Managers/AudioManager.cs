using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Library")]
    [SerializeField] private AudioLibrary library;

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string masterParam = "MasterVol";
    [SerializeField] private string musicParam = "MusicVol";
    [SerializeField] private string sfxParam = "SfxVol";
    [SerializeField] private string uiParam = "UiVol";

    [Header("SFX Pool")]
    [SerializeField, Min(4)] private int sfxPoolSize = 16;

    [Header("Music Source")]
    [SerializeField] private AudioSource musicSource;

    private readonly Queue<AudioSource> sfxPool = new();
    private readonly Dictionary<string, float> lastPlayTime = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.spatialBlend = 0f;
        }

        // build SFX pool
        for (int i = 0; i < sfxPoolSize; i++)
        {
            var go = new GameObject($"SFX_{i}");
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            sfxPool.Enqueue(src);
        }
    }

    // ---------- Public API ----------

    public void PlaySfx(string id, Vector3 position)
    {
        if (!TryGetDef(id, out var def)) return;
        if (IsCoolingDown(id, def.cooldown)) return;

        var src = GetSfxSource();
        ConfigureSource(src, def, isUi: false);
        src.transform.position = position;

        var clip = PickClip(def);
        if (clip == null) return;

        ApplyRandom(def, src);
        src.clip = clip;

        if (def.loop) src.Play();
        else src.PlayOneShot(clip);
    }

    public void PlayUi(string id)
    {
        if (!TryGetDef(id, out var def)) return;
        if (IsCoolingDown(id, def.cooldown)) return;

        var src = GetSfxSource();
        ConfigureSource(src, def, isUi: true);

        var clip = PickClip(def);
        if (clip == null) return;

        ApplyRandom(def, src);
        src.PlayOneShot(clip);
    }

    public void PlayMusic(string id, bool restartIfSame = false)
    {
        if (!TryGetDef(id, out var def)) return;

        var clip = PickClip(def);
        if (clip == null) return;

        if (!restartIfSame && musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.outputAudioMixerGroup = def.outputGroup;
        musicSource.volume = def.volume;
        musicSource.pitch = def.pitch;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();

    // Volume should be 0..1 from UI sliders
    public void SetMasterVolume(float v01) => SetMixerDb(masterParam, v01);
    public void SetMusicVolume(float v01) => SetMixerDb(musicParam, v01);
    public void SetSfxVolume(float v01) => SetMixerDb(sfxParam, v01);
    public void SetUiVolume(float v01) => SetMixerDb(uiParam, v01);

    // ---------- Internals ----------

    private bool TryGetDef(string id, out AudioLibrary.SoundDef def)
    {
        def = null;
        if (library == null) return false;
        return library.TryGet(id, out def);
    }

    private bool IsCoolingDown(string id, float cooldown)
    {
        if (cooldown <= 0f) return false;
        var t = Time.unscaledTime;
        if (lastPlayTime.TryGetValue(id, out var last) && (t - last) < cooldown)
            return true;
        lastPlayTime[id] = t;
        return false;
    }

    private AudioSource GetSfxSource()
    {
        // Round-robin pool
        var src = sfxPool.Dequeue();
        sfxPool.Enqueue(src);
        return src;
    }

    private static AudioClip PickClip(AudioLibrary.SoundDef def)
    {
        if (def.clips == null || def.clips.Length == 0) return null;
        if (def.clips.Length == 1) return def.clips[0];
        return def.clips[Random.Range(0, def.clips.Length)];
    }

    private static void ApplyRandom(AudioLibrary.SoundDef def, AudioSource src)
    {
        float vol = Mathf.Clamp01(def.volume + Random.Range(-def.volumeRandom, def.volumeRandom));
        float pit = def.pitch + Random.Range(-def.pitchRandom, def.pitchRandom);
        src.volume = vol;
        src.pitch = pit;
    }

    private static void ConfigureSource(AudioSource src, AudioLibrary.SoundDef def, bool isUi)
    {
        src.outputAudioMixerGroup = def.outputGroup;

        if (isUi)
        {
            src.spatialBlend = 0f;
            src.loop = false;
        }
        else
        {
            src.spatialBlend = def.spatial ? def.spatialBlend : 0f;
            src.minDistance = def.minDistance;
            src.maxDistance = def.maxDistance;
            src.loop = def.loop;
        }
    }

    private void SetMixerDb(string param, float v01)
    {
        if (mixer == null || string.IsNullOrWhiteSpace(param)) return;

        // Convert 0..1 to decibels. Clamp to avoid -inf.
        v01 = Mathf.Clamp(v01, 0.0001f, 1f);
        float db = Mathf.Log10(v01) * 20f;
        mixer.SetFloat(param, db);
    }
}
