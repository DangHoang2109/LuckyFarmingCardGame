using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Master Contain the anim fill
/// </summary>
public class InGamePlayerUpgradePalletAnim : MonoBehaviour
{
    [Header("Slider Upgrade")]
    public Transform _tfPanel, _tfPanelHide;
    public Transform _tfFrom, _tfTo;

    public InGameBagCardProgressAnim _prefab;
    public Dictionary<int, InGameBagCardProgressAnim> _dicItem;

    public static InGamePlayerUpgradePalletAnim Instance => InGameManager.Instance.GameController._upgradeCollectorAnim;

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
    public void ClearItems()
    {
        if(_dicItem != null)
        {
            foreach (var item in _dicItem.Values)
            {
                item.SetData(0, 0, 1, _tfFrom, _tfTo);
            }
        }
    }
}
