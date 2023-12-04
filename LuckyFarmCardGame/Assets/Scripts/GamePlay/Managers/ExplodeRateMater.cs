using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeRateMater : MonoBehaviour
{
    private CardGameController _controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Init(CardGameController contller)
    {
        this._controller = contller;
    }
    public void OnChangeDeckAmount(int deckAmount)
    {
        //ignore param, call to controller and get rate
        float explodeRate = _controller.GetPalletConflictChance();
        Debug.Log($"EXPLODE RATE: {explodeRate}");
    }
}
