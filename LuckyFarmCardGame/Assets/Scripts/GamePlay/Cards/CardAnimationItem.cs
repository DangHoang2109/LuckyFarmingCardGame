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
    /// <summary>
    /// Callback được giữ và luôn gọi nên là nhớ - trước khi + để tránh duplicate
    /// </summary>
    public System.Action onCompleteDrawPersistent;
    /// <summary>
    /// Callback bị clear đi mỗi khi card hide
    /// </summary>
    public System.Action onCompleteDrawInTurn;

    #endregion Callback

    public void AssignCallbackPersistent(System.Action cb)
    {
        onCompleteDrawPersistent -= cb;
        onCompleteDrawPersistent += cb;
    }
    public void UnAssignCallbackPersistent(System.Action cb)
    {
        onCompleteDrawPersistent -= cb;
    }
    public void PlayDraw(int cardID, System.Action cb = null)
    {
        onCompleteDrawInTurn = cb;

        InGameCardConfig cardConfig = InGameCardConfigs.Instance.GetCardConfig(cardID);
        if (cardConfig != null)
        {
            this._imgFace.sprite = cardConfig._sprCardArtwork;
            this.gameObject.SetActive(true);
            this.Anim?.Play(animDraw);
        }

    }

    /// <summary>
    /// Calling from the animation events
    /// </summary>
    public void OnCompleteDraw()
    {
        //Debug.Log("OnCompleteDraw, me hide");
        onCompleteDrawPersistent?.Invoke();
        onCompleteDrawInTurn?.Invoke();

        this.gameObject.SetActive(false);
    }
}
