using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Master Contain the anim fill
/// </summary>
public class InGamePlayerUpgradePalletAnim : MonoBehaviour
{
    public Transform _tfPanel, _tfPanelHide;
    public Transform _tfFrom, _tfTo;

    public InGameBagCardProgressAnim _prefab;
    public Dictionary<int, InGameBagCardProgressAnim> _dicItem;

    public static InGamePlayerUpgradePalletAnim Instance => InGameManager.Instance.GameController._upgradeCollectorAnim;

    public void ShowCollectUpgrades(List<InGame_CardDataModel> cardFromPallet )
    {
        _dicItem ??= new Dictionary<int, InGameBagCardProgressAnim>();

        ////prepare the anim item
        //List<InGameBagCardProgressAnim> _items = new List<InGameBagCardProgressAnim>();
        //foreach (var card in cardFromPallet)
        //{
        //    if (!_dicItem.TryGetValue(card._id, out InGameBagCardProgressAnim anim))
        //    {
        //        anim = Instantiate(_prefab, _tfPanelHide);
        //        _dicItem.Add(card._id, anim);
        //        anim.SetCardInfo(card._id);
        //    }
        //        _items.Add(anim);
        //}

        ////when we ensure all the item is ready
        //DOTween.Kill(this.GetInstanceID());
        //Sequence seq = DOTween.Sequence();
        //seq.SetId(this.GetInstanceID());
        //foreach (var item in _items)
        //{
        //    item.SetData()
        //    seq.Join(item.DoAnim());
        //}
    }

    public void ShowCollectUpgrade(int cardID, int oldVal, int newVal, int goal)
    {
        _dicItem ??= new Dictionary<int, InGameBagCardProgressAnim>();
        //prepare the anim item
        if (!_dicItem.TryGetValue(cardID, out InGameBagCardProgressAnim anim))
        {
            anim = Instantiate(_prefab, _tfPanel);
            _dicItem.Add(cardID, anim);
            anim.SetCardInfo(cardID);
        }
        anim.SetData(oldVal, newVal, goal, _tfFrom, _tfTo);
        anim.transform.position = _tfFrom.transform.position;
        //create the collect action
        DoCollectUpgrade act = new DoCollectUpgrade(anim);
        CollectUpgradeActionManager.Instance.AddActionAndRun(act);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            ShowCollectUpgrade(0, 0, 1, 2);
            ShowCollectUpgrade(1, 0, 1, 2);
            ShowCollectUpgrade(2, 0, 1, 2);
        }
    }
}
