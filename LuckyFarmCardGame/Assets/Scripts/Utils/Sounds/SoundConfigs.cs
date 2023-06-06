using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Games/Sounds")]
public class SoundConfigs : ScriptableObject
{
    public List<AudioClip> sounds;
    private static SoundConfigs _isntance;
    public static SoundConfigs Instance
    {
        get
        {
            if (_isntance == null)
            {

                _isntance = LoaderUtility.Instance.GetAsset<SoundConfigs>("Home/Configs/SoundConfigs");

            }
            return _isntance;
        }

    }
    private Dictionary<string, AudioClip> temps;

#if UNITY_EDITOR

    private void OnValidate()
    {
        string path = string.Format("Assets/LocalDatas/Sounds/");

        List<AudioClip> audioClips = GameUtils.LoadAllAssetsInFolder<AudioClip>(path, new List<string> { "*.mp3", "*.ogg", "*.wav" });

        this.sounds = new List<AudioClip>(audioClips);
    }
#endif

    public AudioClip GetAudioByName(string key)
    {
        if (this.temps == null)
        {
            this.temps = new Dictionary<string, AudioClip>();
        }
        if (this.temps.ContainsKey(key))
        {
            return this.temps[key];
        }
        else
        {
            AudioClip clip = this.sounds.Find(x => x.name.Equals(key));
            if (clip != null)
            {
                this.temps.Add(key, clip);
                return clip;
            }
        }
        Debug.LogError("CLIP NULL: " + key);
        return null;
    }

    public List<string> GetSoundsContainsName(string nameSound)
    {
        List<string> snds = new List<string>();
        foreach(AudioClip clip in this.sounds)
        {
            if (clip.name.Contains(nameSound))
                snds.Add(clip.name);
        }
        return snds;
    }
}
