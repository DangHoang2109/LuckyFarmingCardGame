using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class InGameTurnNotification : MonoBehaviour
{
    #region Property in Inspector
    public Transform _playerNotifyBaseShowPos, _enemyNotifyBaseShowPos;
    public Queue<InGameTurnNotificationItem> items;
    public InGameTurnNotificationItem _prefab;
    public Transform _panel;
    #endregion Property in Inspector
    public InGameTurnNotificationItem GetItem()
    {
        items ??= new Queue<InGameTurnNotificationItem>();
        if (items.Count > 0)
            return items.Dequeue();
        else
        {
            InGameTurnNotificationItem i = Instantiate(this._prefab, this._panel);

            return i.SetUp();
        }
    }
    public void ShowText(List<string> contents, bool mainPlayerTurn, float timeStay = 1f)
    {
        for (int i = 0; i < contents.Count; i++)
        {
            InGameTurnNotificationItem activeItem = this.GetItem();
            activeItem.gameObject.SetActive(true);
            activeItem.ShowText(
                contents[i], mainPlayerTurn,
                playerShowPos: mainPlayerTurn ? _playerNotifyBaseShowPos.position : _enemyNotifyBaseShowPos.position
                , i*0.4f, timeStay, OnClear);
        }
    }
    public void ShowText(string content, bool mainPlayerTurn, float timeStay = 1f)
    {
        InGameTurnNotificationItem activeItem = this.GetItem();
        activeItem.gameObject.SetActive(true);
        activeItem.ShowText(content, mainPlayerTurn,
            playerShowPos: mainPlayerTurn ? _playerNotifyBaseShowPos.position : _enemyNotifyBaseShowPos.position,
            0f, timeStay, OnClear);
    }
    public void ShowText(string content, bool mainPlayerTurn, Vector3 casterPosition, float timeStay = 1f)
    {
        InGameTurnNotificationItem activeItem = this.GetItem();
        activeItem.gameObject.SetActive(true);
        activeItem.ShowText(content, mainPlayerTurn,
            playerShowPos: casterPosition,
            0f, timeStay, OnClear);
    }
    void OnClear(InGameTurnNotificationItem item)
    {
        item.SetUp();
        item.gameObject.SetActive(false);
        this.items.Enqueue(item);
    }
}
