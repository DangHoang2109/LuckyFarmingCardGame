using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBase : MonoBehaviour
{
    [SerializeField]
    private List<AudioSource> soundEffects;
    private Dictionary<string, List<AudioSource>> sounds = new Dictionary<string, List<AudioSource>>();
#if UNITY_EDITOR
    private void OnValidate()
    {
        AudioSource[] temps = this.gameObject.GetComponentsInChildren<AudioSource>();
        this.soundEffects = new List<AudioSource>(temps);
        
    }
#endif
    public void OnChangeMusic(object isMute)
    {
        bool isMusic = (bool)isMute;
        foreach(AudioSource source in this.soundEffects)
        {
            if(source != null)
            {
                source.mute = isMusic;
            }
            
        }
    }
    public void OnChangeSound(object isMute)
    {
        bool isSound = (bool)isMute;
        foreach (AudioSource source in this.soundEffects)
        {
            if(source != null)
            {
                source.mute = isSound;
            }
            
        }
    }
    public void Play(string key, Vector3 pos, float volume)
    {
        AudioClip clip = SoundConfigs.Instance.GetAudioByName(key);
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, volume);
        }
    }


    public AudioSource Play(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("KEY NULL: " + key);
            return null;
        }

        AudioSource soundEffect = this.GetSoundEffect(key);
        if (soundEffect != null)
        {
            soundEffect.clip = SoundConfigs.Instance.GetAudioByName(key); //LoaderUtility.Instance.GetAsset<AudioClip>(key);
            soundEffect.loop = false;
            soundEffect.Play();
            if (this.sounds.ContainsKey(key))
            {
                this.sounds[key].Add(soundEffect);
            }
            else
            {
                List<AudioSource> audioSources = new List<AudioSource>();
                audioSources.Add(soundEffect);
                this.sounds.Add(key, audioSources);
            }

            return soundEffect;
        }
        return null;
    }
    public AudioSource PlayLoop(string key, int loop = -1)
    {
        AudioSource audioSource = this.Play(key);
        audioSource.loop = true;

        return audioSource;
    }
    public AudioSource PlayLoop(string key, float loopTime)
    {
        return null;
    }
    public void StopSound(string key)
    {
        if(this.sounds == null)
        {
            return;
        }
        
        if(this.sounds.ContainsKey(key))
        { 
            foreach(AudioSource audio in this.sounds[key])
            {
                audio.Stop();
            }
        } 
    }
    public void StopSoundLoop(string key)
    {
        if (this.sounds == null)
        {
            return;
        }

        if (this.sounds.ContainsKey(key))
        {
            foreach (AudioSource audio in this.sounds[key])
            {
                audio.Stop();
            }
        }
    }

    private AudioSource GetSoundEffect(string key)
    {
        AudioSource audioSource = null;
        audioSource = soundEffects.Find(audio => audio.isPlaying == false && audio.loop == false);
        if (audioSource != null)
        {
            return audioSource;
        }

        if (this.IsCanCreateAudioSource(key, 5))
        {
            
            GameObject gameObject = new GameObject(key);
            gameObject.transform.SetParent(base.transform, false);
            audioSource = gameObject.AddComponent<AudioSource>();
            this.soundEffects.Add(audioSource);
            return audioSource;
        }
        else
        {
            Debug.LogError("FULL POOL: " + key);
        }

        return null;
    }
    private bool IsCanCreateAudioSource(string key, int poolCount)
    {
        if(this.sounds == null)
        {
            this.sounds = new Dictionary<string, List<AudioSource>>();
            return true;
        }

        int total = 0;
        if(this.sounds.ContainsKey(key))
        {
            total = this.sounds[key].FindAll(x => x.isPlaying).Count;
            
            return total <= poolCount;
        }
        return true;
    }
    public void Clear()
    {
        Debug.LogWarning("Cleared sound!");
        this.soundEffects.ForEach(x => x.Stop());
    }

}
