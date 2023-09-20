using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FxBoosterParticle : MonoBehaviour
{
    public ParticleSystem par;
    public ParticleSystemForceField parForce;

    private UnityAction callback;

#if UNITY_EDITOR
    private void OnValidate()
    {
        this.par = this.GetComponentInChildren<ParticleSystem>();
        this.parForce = this.GetComponentInChildren<ParticleSystemForceField>();
    }
#endif

    private void OnDisable()
    {
        this.callback?.Invoke();
        this.callback = null;
    }

    /// <summary>
    /// Show fx booster particle
    /// </summary>
    /// <param name="posStart">vị trí bắt đầu</param>
    /// <param name="posEnd">vị trí kết thúc</param>
    /// <param name="time">thời gian bay</param>
    /// <param name="callback">callback lúc bay xong</param>
    public void ShowFx(Vector3 posStart, Vector3 posEnd, int num, float scale, float time, UnityAction callback)
    {
        
        this.callback = callback;
        this.transform.position = posStart;
        this.parForce.directionX = posEnd.x - posStart.x;
        this.parForce.directionY = posEnd.y - posStart.y;

        ParticleSystem.MinMaxCurve countTime = new ParticleSystem.MinMaxCurve();
        countTime.constant = time;
        var main = this.par.main;
        main.startLifetime = countTime;

        ParticleSystem.MinMaxCurve countBust = new ParticleSystem.MinMaxCurve();
        countBust.constant = num;
        this.par.emission.SetBurst(0, new ParticleSystem.Burst(0f, countBust));

        this.par.gameObject.SetActive(true);
    }

}
