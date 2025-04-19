using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LuckyFantasy
{
    /// <summary>
    /// Manages the Mind Points (MP) system, which is the primary resource
    /// for playing cards during combat.
    /// </summary>
    public class MindPointsSystem : MonoBehaviour
    {
        public static MindPointsSystem Instance;
        
        #region Fields

        [Header("Mind Points Settings")]
        [SerializeField] private int minDiceValue = 1; // Minimum value on each dice
        [SerializeField] private int maxDiceValue = 6; // Maximum value on each dice
        [SerializeField] private int numberOfDice = 3; // Number of dice to roll for MP
        [SerializeField] private float perfectRollMultiplier = 3f; // Multiplier for rolling all the same value

        [Header("UI References")]
        [SerializeField] private Transform diceContainerTransform; // Parent for dice visuals
        [SerializeField] private GameObject dicePrefab; // Visual representation of dice

        // Events
        public UnityEvent<int> OnMindPointsChanged;
        public UnityEvent<int[]> OnDiceRolled;
        public UnityEvent<bool> OnPerfectRoll;

        // Current state
        private int currentMP = 0;
        private int[] currentDiceValues;
        private bool wasPerfectRoll = false;

        #endregion

        #region Properties

        /// <summary>
        /// Current Mind Points available to the player
        /// </summary>
        public int CurrentMP => currentMP;

        /// <summary>
        /// The values of the last dice roll
        /// </summary>
        public int[] CurrentDiceValues => currentDiceValues;

        /// <summary>
        /// Whether the last roll was a perfect roll (all dice showing the same value)
        /// </summary>
        public bool WasPerfectRoll => wasPerfectRoll;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            Instance = this;
            currentDiceValues = new int[numberOfDice];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Roll dice to determine Mind Points for this turn
        /// </summary>
        /// <returns>Total MP generated from the roll</returns>
        public int RollForMindPoints()
        {
            // Roll dice and record values
            int totalValue = 0;
            bool allSameValue = true;
            int firstValue = 0;

            currentDiceValues = new int[numberOfDice];

            for (int i = 0; i < numberOfDice; i++)
            {
                int diceValue = UnityEngine.Random.Range(minDiceValue, maxDiceValue + 1);
                currentDiceValues[i] = diceValue;
                totalValue += diceValue;

                // Check if this is part of a perfect roll
                if (i == 0)
                {
                    firstValue = diceValue;
                }
                else if (diceValue != firstValue)
                {
                    allSameValue = false;
                }

                // TODO: Instantiate or update dice visuals
            }

            // Apply perfect roll bonus if applicable
            wasPerfectRoll = allSameValue && numberOfDice > 1;
            if (wasPerfectRoll)
            {
                totalValue = Mathf.RoundToInt(totalValue * perfectRollMultiplier);
            }

            // Set the current MP
            currentMP = totalValue;

            // Trigger events
            OnDiceRolled?.Invoke(currentDiceValues);
            OnPerfectRoll?.Invoke(wasPerfectRoll);
            OnMindPointsChanged?.Invoke(currentMP);

            Debug.Log($"Rolled {numberOfDice} dice for {currentMP} Mind Points" + (wasPerfectRoll ? " (Perfect Roll!)" : ""));
            return currentMP;
        }

        /// <summary>
        /// Spend Mind Points to play a card
        /// </summary>
        /// <param name="amount">MP cost of the card</param>
        /// <returns>True if MP was successfully spent</returns>
        public bool SpendMP(int amount)
        {
            if (amount > currentMP)
            {
                Debug.LogWarning($"Cannot spend {amount} MP - only {currentMP} available");
                return false;
            }

            currentMP -= amount;
            OnMindPointsChanged?.Invoke(currentMP);
            Debug.Log($"Spent {amount} MP, {currentMP} remaining");
            return true;
        }

        /// <summary>
        /// Add MP during a turn (from card effects, etc.)
        /// </summary>
        /// <param name="amount">Amount of MP to add</param>
        public void AddMP(int amount)
        {
            if (amount <= 0) return;

            currentMP += amount;
            OnMindPointsChanged?.Invoke(currentMP);
            Debug.Log($"Added {amount} MP, now at {currentMP}");
        }

        /// <summary>
        /// Reset MP to zero at the end of turn
        /// </summary>
        public void ResetMP()
        {
            currentMP = 0;
            OnMindPointsChanged?.Invoke(currentMP);
            Debug.Log("Mind Points reset to 0");
        }

        /// <summary>
        /// Get the maximum possible MP from a perfect roll
        /// </summary>
        /// <returns>Maximum possible MP value</returns>
        public int GetMaxPossibleMP()
        {
            int maxRoll = numberOfDice * maxDiceValue;
            return Mathf.RoundToInt(maxRoll * perfectRollMultiplier);
        }

        /// <summary>
        /// Get the minimum possible MP from a roll
        /// </summary>
        /// <returns>Minimum possible MP value</returns>
        public int GetMinPossibleMP()
        {
            return numberOfDice * minDiceValue;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create visual representations of dice
        /// </summary>
        private void CreateDiceVisuals()
        {
            // TODO: Implement dice visual creation
            // This would create or update GameObjects representing the dice
            Debug.Log("Creating dice visuals");
        }

        /// <summary>
        /// Update dice visuals to show the current values
        /// </summary>
        private void UpdateDiceVisuals()
        {
            // TODO: Implement dice visual updates
            // This would animate or update the dice visuals to show the current roll
            Debug.Log("Updating dice visuals");
        }

        #endregion
    }
}
