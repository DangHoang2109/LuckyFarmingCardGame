using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEditor
{
    [UnityEditor.MenuItem("Cosinas/Game/TestInCreaseMaxHP")]
    public static void TestInCreaseMaxHP()
    {
        InGameManager.Instance.OnPlayerIncreaseMaxHP(idHealReceiver: 0, hpAdd: 10);
    }
    [UnityEditor.MenuItem("Cosinas/Game/Reveal100card")]
    public static void Reveal100cards()
    {
        List<InGame_CardDataModel> topCards = InGameManager.Instance.GameController.GetDeckTopCards(100, isWillPopThatCardOut: false);
        Debug.Log(topCards.Count);
    }
}
