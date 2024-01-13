using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameDialog : BaseDialog
{
    public TMPro.TextMeshProUGUI _tmpTitle;
    public Button _btnContinue, _btnQuit, _btnReplay;
    //private JoinGameDataNormal joinGameData
    //{
    //    get
    //    {
    //        return JoinGameManager.Instance.GetJoinGameData<JoinGameDataNormal>();
    //    }
    //}
    private bool isWin = false, isContinue = false;

    public void ParseData(bool isWin, bool isContinued) //, List<RewardConfig> rewards
    {
        //this.rewardLayout.ParseData(rewards);
        this.isWin = isWin; this.isContinue = isContinued;
        ShowButton();
        this.DoAnimation();

        //SoundManager.Instance.Play(isWin ? SoundKey.WinGame : SoundKey.LoseGame);
    }
    private void ShowButton()
    {
        _btnContinue.gameObject.SetActive(!isWin && !isContinue);
        _btnQuit.gameObject.SetActive(!isWin);
        _btnReplay.gameObject.SetActive(isWin);
    }
    private void DoAnimation()
    {
        //Sequence seq = DOTween.Sequence();
        //seq.SetId(this.GetInstanceID());
        //if (this.isWin)
        //{
        //    seq.Join(this.player.DoWin());
        //}
        //else
        //{
        //    seq.Join(this.opponent.DoWin());
        //}

        //seq.AppendInterval(1f);
        //seq.Append(this.canvasGroupReward.DOFade(1, 0.2f));

        //seq.AppendCallback(() =>
        //{
        //    this.CheckShowAnimCollectBag();
        //});
        //seq.AppendInterval(0.5f);
        //seq.OnComplete(() =>
        //{
        //    this.panelToContinue.gameObject.SetActive(true);
        //});
    }

    public void OnContinue()
    {
        //watch add recover 50%, pay gem recover 100%
        InGameManager.Instance.OnContinueGame(0.5f);
        ClickCloseDialog();
    }
    public void OnQuit()
    {
        _tmpTitle.text = "CHƯA LÀM HOMESCENE, REOPEN APP NHA!";
    }
    public void Dev_OnReplay()
    {
        InGameManager.Instance.PrepareGame();
    }

    public static EndGameDialog ShowEndGameDialog()
    {
        return GameManager.Instance.OnShowDialog<EndGameDialog>("Dialogs/EndGameDialog");
    }
}
