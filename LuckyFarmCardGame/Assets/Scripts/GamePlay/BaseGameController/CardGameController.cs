using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameController : MonoBehaviour
{
    //Khi user draw ra 1 card đã có trên pallet
    public System.Action _onPalletConflict;
    //Pallet conflict after: roll dice nhưng bị destroy
    public System.Action _onPalletDestroyed;
    //Pallet conflict after: roll dice và thắng roll
    public System.Action<List<InGame_CardDataModel>> _onRuleMakeUserPullCard;

    public List<InGame_CardDataModel> _cardsOnPallet;

    public void InitGame()
    {
        _cardsOnPallet = new List<InGame_CardDataModel>();
    }

    public void AddCallback_PalletConflict(System.Action cb)
    {
        _onPalletConflict -= cb;
        _onPalletConflict += cb;
    }
    public void AddCallback_PalletDestroyed(System.Action cb)
    {
        _onPalletDestroyed -= cb;
        _onPalletDestroyed += cb;
    }
    public void AddCallback_PalletPulledByRule(System.Action<List<InGame_CardDataModel>> cb)
    {
        _onRuleMakeUserPullCard -= cb;
        _onRuleMakeUserPullCard += cb;
    }


    #region Draw and Interacting with Pallet
    public void OnDrawACard()
    {
        //Get a card from deck
        InGame_CardDataModel topDeckCard = GetDeckTopCard(isWillPopThatCardOut: true);
        //put it to pallet
        PutACardToPallet(topDeckCard);
    }

    private InGame_CardDataModel GetDeckTopCard(bool isWillPopThatCardOut)
    {
        int newCardID = Random.Range(0, 6);
        return new InGame_CardDataModel().SetCardID(newCardID);
    }
    private void PutACardToPallet(InGame_CardDataModel card)
    {
        _cardsOnPallet ??= new List<InGame_CardDataModel>();

        //if in pallet already has this card
        int indexOfSameIDCardIfExistOnPallet = _cardsOnPallet.FindIndex(x => x._id == card._id);
        if (indexOfSameIDCardIfExistOnPallet >= 0 && indexOfSameIDCardIfExistOnPallet < _cardsOnPallet.Count)
        {
            Debug.Log($"RULE: EXISTED CARD {card._id} at index {indexOfSameIDCardIfExistOnPallet}");
            _onPalletConflict?.Invoke();
            OnPalletConflict();
        }
        //else: let it in
        else
        {
            _cardsOnPallet.Add(card);
            Debug.Log($"RULE: ADD CARD {card._id} at index {_cardsOnPallet.Count - 1}");
        }

        //active card effect

    }

    private void OnPalletConflict()
    {
        //ask the rule to see what to do?
        ThisFuncShouldOnRule_TellControllerToRollingDice(this._cardsOnPallet);

        void ThisFuncShouldOnRule_TellControllerToRollingDice(List<InGame_CardDataModel> cardsOnPallet)
        {
            //you may replace 'this' with the 'GameController'
            if (cardsOnPallet == null || cardsOnPallet.Count == 0)
                return;

            int diceResult = this.RollADice();
            Debug.Log($"RULE: Rolling dice {diceResult} compare to pallet {cardsOnPallet.Count}");
            //if dice result > pallet amount card => the pallet will be destroyed
            if(diceResult > cardsOnPallet.Count)
            {
                this.DestroyPallet();
            }
            else
            {
                this.RuleForce_PullCardFromPalletToUser();
            }
        }
    }
    public void DestroyPallet()
    {
        //play the destroying card animation
        if (this._cardsOnPallet == null || this._cardsOnPallet.Count == 0)
            return;

        Debug.Log("CONTROLER: DESTROY PALLET");
        _onPalletDestroyed?.Invoke();

        ClearPallet();
    }
    public void RuleForce_PullCardFromPalletToUser()
    {
        Debug.Log("CONTROLER: RULE PULL PALLET");

        List<InGame_CardDataModel> receiveCards = PullCardFromPalletToUser();
        _onRuleMakeUserPullCard?.Invoke(receiveCards);
    }
    public List<InGame_CardDataModel> PullCardFromPalletToUser()
    {
        Debug.Log("CONTROLER: PULL PALLET");

        List<InGame_CardDataModel> cacheCardsInPallet = new List<InGame_CardDataModel>(this._cardsOnPallet);

        ClearPallet();

        return cacheCardsInPallet;
    }
    private void ClearPallet()
    {
        this._cardsOnPallet ??= new List<InGame_CardDataModel>();
        this._cardsOnPallet.Clear();
    }
    private int RollADice()
    {
        return UnityEngine.Random.Range(0, 6);
    }
    #endregion Draw and Interacting with Pallet
}
