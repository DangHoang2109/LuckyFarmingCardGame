using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameDeckUIAnimator : MonoBehaviour
{
    public Animator _animDeck;
    public System.Action _onClickDraw, _onShuffleDeckComplete;

    private const string KEY_ANIM_SHUFFLE = "PrepareDeckDisappear";

    public void PlayAnimationShuffleDeck(System.Action cb)
    {
        if (_animDeck == null)
            _animDeck = GetComponent<Animator>();

        _onShuffleDeckComplete = cb;
        _animDeck.gameObject.SetActive(true);
        _animDeck.Play(KEY_ANIM_SHUFFLE);
    }
    /// <summary>
    /// Assign on animator
    /// </summary>
    private void OnShuffleConplete()
    {
        _animDeck.gameObject.SetActive(false);
        _onShuffleDeckComplete?.Invoke();
    }
    private void PlaySfx()
    {

    }
    private void AddCardFinished()
    {

    }
}
