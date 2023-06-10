using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InGameDiceRollingAnimator : MonoBehaviour {

    // Array of dice sides sprites to load from Resources folder
    [SerializeField] private Sprite[] diceSides;

    // Reference to sprite renderer to change sprites
    [SerializeField] private Image rend;

    public void RollingDice(int result, System.Action callback = null, float timeRollingAnim = 1f, float timeShowResult = 1f)
    {
        this.gameObject.SetActive(true);

        StartCoroutine(RollTheDice(result, callback, timeRollingAnim, timeShowResult));
    }

    // Coroutine that rolls the dice
    private IEnumerator RollTheDice(int result, System.Action callback = null, float timeRollingAnim = 1f, float timeShowResult = 1f)
    {
        YieldInstruction yield = new WaitForSeconds(0.05f);
        // Variable to contain random dice side number.
        // It needs to be assigned. Let it be 0 initially
        int randomDiceSide = 0;

        int totalIteration = (int)(timeRollingAnim / 0.05f);

        // Loop to switch dice sides ramdomly
        // before final side appears. 20 itterations here.
        for (int i = 0; i < totalIteration; i++)
        {
            // Pick up random value from 0 to 5 (All inclusive)
            randomDiceSide = Random.Range(0, 5);

            // Set sprite to upper face of dice from array according to random value
            rend.sprite = diceSides[randomDiceSide];

            // Pause before next itteration
            yield return yield;
        }

        rend.sprite = diceSides[result];

        yield return new WaitForSeconds(timeShowResult);

        callback?.Invoke();
        this.gameObject.SetActive(false);
    }
}
