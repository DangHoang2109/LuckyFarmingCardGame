using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class SoundManager : MonoSingleton<SoundManager>
{
    public bool isMusic;
    public bool isSound;
    public bool isVibrate;
    public SoundBase soundEffect;
    public SoundBase soundBg;

    public const float VOLUME_SOUND = 0.7f;// 1f;
    public const float VOLUME_MUSIC = 0.3f;// 0.4f;

    public override void Init()
    {
        this.isMusic = true;
        this.isSound = true;
        base.Init();
    }
    private void Start()
    {

        this.AddCallbackMusic(this.soundBg.OnChangeMusic);
        this.AddCallbackSounds(this.soundEffect.OnChangeSound);
    }
    private void OnDestroy()
    {
        this.RemoveCallbackMusic(this.soundBg.OnChangeMusic);

        this.RemoveCallbackSounds(this.soundEffect.OnChangeSound);
    }
    #region EFFECT
    public void Play(string key, Vector3 pos, float volume)
    {
        if(!this.isSound)
        {
            return;
        }
        this.soundEffect.Play(key, pos, volume);
    }
    public void Play(string key, float volumne)
    {
        if (!this.isSound)
        {
            return;

        }
        this.soundEffect.Play(key, Vector3.zero, volumne);
    }
    public void Play(string key)
    {
        if (!this.isSound)
        {
            return;
        }
        this.soundEffect.Play(key, Vector3.zero, VOLUME_SOUND);
    }
    public void PlayLoop(string key, int loop = -1)
    {
        if (!this.isSound)
        {
            return;

        }
        AudioSource sound = this.soundEffect.PlayLoop(key, loop);
        if (sound != null)
        {
            sound.volume = VOLUME_SOUND;
            sound.mute = false;
        }
    }
    public void PlayLoop(string key, float loopTime)
    {
        if (!this.isSound)
        {
            return;

        }
        AudioSource sound =  this.soundEffect.PlayLoop(key, loopTime);
        if (sound != null)
        {
            sound.volume = VOLUME_SOUND;
            sound.mute = false;
        }
    }
    public void StopSound(string key)
    {
        this.soundEffect.StopSound(key);
    }
    public void StopSoundLoop(string key)
    {
        this.soundEffect.StopSoundLoop(key);
    }
    public void PlayButtonClick()
    {
        this.Play("snd_button_click");
    }


    //volume tương ứng với tốc độ banh
    public void BallHitRail(Vector3 velocity)
    {
        /*float v0 = Mathf.Max(Mathf.Abs(velocity.x), Mathf.Abs(velocity.y), Mathf.Abs(velocity.z));
        v0 = Mathf.Clamp(v0, 0, 5) * 0.2f;
        v0 = (float)(System.Math.Round(v0, 2));
        this.Play("RailHit", v0);*/
    }
    public void BallhitBall(Vector3 velocity, Vector3 pos)
    {
        float v0 = Mathf.Max(Mathf.Abs(velocity.x), Mathf.Abs(velocity.y), Mathf.Abs(velocity.z));
        v0 = Mathf.Clamp(v0, 0.1f, 10f);
        v0 = (float)(v0 / 10.0f);
        this.Play("game_ball_collision_strong", Vector3.zero, v0);
    }

    #endregion
    #region BG
    public void PlayMusic(string key)
    {
        if(!this.isMusic)
        {
            return;
        }
        AudioSource music = this.soundBg.PlayLoop(key);
        music.volume = VOLUME_MUSIC;
        music.mute = false;
    }
    public void StopMusic(string key)
    {
        this.soundBg.StopSound(key);
    }
    #endregion

    
    #region Callback
    private CallbackEventObject callbackMusics = new CallbackEventObject();
    private CallbackEventObject callbackSounds = new CallbackEventObject();
    public void AddCallbackMusic(UnityAction<object> callback)
    {
        if(this.callbackMusics == null)
        {
            this.callbackMusics = new CallbackEventObject();
        }
        this.callbackMusics.AddListener(callback);
        callback.Invoke(this.isMusic);
        

    }
    public void RemoveCallbackMusic(UnityAction<object> callback)
    {
        if (this.callbackMusics == null)
        {
            this.callbackMusics = new CallbackEventObject();
        }
        this.callbackMusics.RemoveListener(callback);
    }
    public void OnChangeMusic(bool isMusic)
    {
        this.isMusic = isMusic;
        this.callbackMusics.Invoke(this.isMusic);
    }

    public void AddCallbackSounds(UnityAction<object> callback)
    {
        if (this.callbackSounds == null)
        {
            this.callbackSounds = new CallbackEventObject();
        }
        this.callbackSounds.AddListener(callback);
        callback.Invoke(this.isSound);

    }
    public void RemoveCallbackSounds(UnityAction<object> callback)
    {
        if (this.callbackSounds == null)
        {
            this.callbackSounds = new CallbackEventObject();
        }
        this.callbackSounds.RemoveListener(callback);
    }
    public void OnChangeSounds(bool isSound)
    {
        this.isSound = isSound;
        this.callbackSounds.Invoke(this.isSound);
    }
    #endregion
}
