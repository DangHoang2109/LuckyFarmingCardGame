using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardAnimationItem : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public Animator Anim
    {
        get { 
            if (animator == null)
                animator = GetComponent<Animator>();
            return animator; 
        }
    }
    public Image _imgFace;

    #region Anim Key
    private string animDraw = "CardPlayHeroB";

    #endregion Anim Key

    #region Callback
    public System.Action onCompleteDraw;

    #endregion Callback


    public void PlayDraw(int cardID)
    {
        InGameCardConfig cardConfig = InGameCardConfigs.Instance.GetCardConfig(cardID);
        if (cardConfig != null)
        {
            this._imgFace.sprite = cardConfig._sprCardArtwork;
            this.gameObject.SetActive(true);
            this.Anim?.Play(animDraw);
        }

    }
    public void OnCompleteDraw()
    {
        onCompleteDraw?.Invoke();
        this.gameObject.SetActive(false);
    }
}
