using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/Audio Library")]
public class AudioLibrary : ScriptableObject
{
    [Serializable]
    public class SoundDef
    {
        public string id;

        [Header("Clips")]
        public AudioClip[] clips;

        [Header("Routing")]
        public AudioMixerGroup outputGroup;

        [Header("Defaults")]
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0f, 0.5f)] public float volumeRandom = 0.1f;

        [Range(-3f, 3f)] public float pitch = 1f;
        [Range(0f, 0.5f)] public float pitchRandom = 0.05f;

        public bool loop = false;

        [Header("3D (SFX)")]
        public bool spatial = true;
        [Range(0f, 1f)] public float spatialBlend = 1f; // 1 = 3D, 0 = 2D
        public float minDistance = 1f;
        public float maxDistance = 15f;

        [Header("Anti-spam")]
        public float cooldown = 0f; // seconds
    }

    public List<SoundDef> sounds = new();

    private Dictionary<string, SoundDef> _map;

    public bool TryGet(string id, out SoundDef def)
    {
        if (_map == null)
        {
            _map = new Dictionary<string, SoundDef>(StringComparer.Ordinal);
            foreach (var s in sounds)
            {
                if (!string.IsNullOrWhiteSpace(s.id))
                    _map[s.id] = s;
            }
        }
        return _map.TryGetValue(id, out def);
    }
}
