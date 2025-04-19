using System;
using System.Collections.Generic;
using LuckyAdventure.GameCards;
using UnityEngine;

namespace LuckyFantasy
{
    /// <summary>
    /// Manages the player's card collection and deck building outside of combat.
    /// </summary>
    public class DeckBuilder : MonoBehaviour
    {
        #region Fields

        [Header("Card Collection")]
        [SerializeField] private List<InGameCard> cardCollection = new List<InGameCard>(); // All cards the player owns
        [SerializeField] private List<InGameCard> currentDeck = new List<InGameCard>(); // Cards in the player's deck
        
        [Header("Deck Settings")]
        [SerializeField] private int minDeckSize = 15; // Minimum required cards in a deck
        [SerializeField] private int maxDeckSize = 30; // Maximum allowed cards in a deck

        [Header("Class Settings")]
        [SerializeField] private InGameCard.CharacterClass currentClass = InGameCard.CharacterClass.Farmer; // Player's selected class

        // Events
        public Action<InGameCard> OnCardAdded;
        public Action<InGameCard> OnCardRemoved;
        public Action<InGameCard.CharacterClass> OnClassChanged;

        #endregion

        #region Properties

        public int CurrentDeckSize => currentDeck.Count;
        public int CardCollectionSize => cardCollection.Count;
        public InGameCard.CharacterClass CurrentClass => currentClass;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            // Initialize with starter deck for current class
            InitializeStarterDeck();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize the starter deck for the current character class
        /// </summary>
        public void InitializeStarterDeck()
        {
            // Clear current deck
            currentDeck.Clear();
            
            // Add starter cards based on class
            switch (currentClass)
            {
                case InGameCard.CharacterClass.Farmer:
                    AddFarmerStarterCards();
                    break;
                case InGameCard.CharacterClass.MaleKnight:
                case InGameCard.CharacterClass.FemaleKnight:
                    AddKnightStarterCards();
                    break;
                case InGameCard.CharacterClass.MalePriest:
                case InGameCard.CharacterClass.FemalePriest:
                    AddPriestStarterCards();
                    break;
                case InGameCard.CharacterClass.MaleDuelist:
                case InGameCard.CharacterClass.FemaleDuelist:
                    AddDuelistStarterCards();
                    break;
                case InGameCard.CharacterClass.MaleAssassin:
                case InGameCard.CharacterClass.FemaleAssassin:
                    AddAssassinStarterCards();
                    break;
                default:
                    AddFarmerStarterCards(); // Default to farmer
                    break;
            }
            
            Debug.Log($"Initialized starter deck for {currentClass} with {currentDeck.Count} cards");
        }

        /// <summary>
        /// Change the character class and reset to appropriate starter deck
        /// </summary>
        /// <param name="newClass">The class to change to</param>
        public void ChangeClass(InGameCard.CharacterClass newClass)
        {
            currentClass = newClass;
            InitializeStarterDeck();
            OnClassChanged?.Invoke(currentClass);
            Debug.Log($"Changed class to {currentClass}");
        }

        /// <summary>
        /// Add a card from collection to the current deck
        /// </summary>
        /// <param name="cardId">ID of the card to add</param>
        /// <returns>True if card was added successfully</returns>
        public bool AddCardToDeck(string cardId)
        {
            // Check if deck is already at max capacity
            if (currentDeck.Count >= maxDeckSize)
            {
                Debug.LogWarning($"Cannot add card: Deck already at maximum size ({maxDeckSize})");
                return false;
            }
            
            // Find the card in collection
            InGameCard cardToAdd = cardCollection.Find(card => card.CardId == cardId);
            
            if (cardToAdd == null)
            {
                Debug.LogWarning($"Card with ID {cardId} not found in collection");
                return false;
            }
            
            // Add to deck
            currentDeck.Add(cardToAdd);
            OnCardAdded?.Invoke(cardToAdd);
            Debug.Log($"Added card {cardToAdd.CardName} to deck");
            return true;
        }

        /// <summary>
        /// Remove a card from the current deck
        /// </summary>
        /// <param name="cardIndex">Index of the card in the deck</param>
        /// <returns>True if card was removed successfully</returns>
        public bool RemoveCardFromDeck(int cardIndex)
        {
            // Check if index is valid
            if (cardIndex < 0 || cardIndex >= currentDeck.Count)
            {
                Debug.LogWarning($"Invalid card index: {cardIndex}");
                return false;
            }
            
            // Check if removing would make deck too small
            if (currentDeck.Count <= minDeckSize)
            {
                Debug.LogWarning($"Cannot remove card: Deck already at minimum size ({minDeckSize})");
                return false;
            }
            
            // Remove the card
            InGameCard removedCard = currentDeck[cardIndex];
            currentDeck.RemoveAt(cardIndex);
            OnCardRemoved?.Invoke(removedCard);
            Debug.Log($"Removed card {removedCard.CardName} from deck");
            return true;
        }

        /// <summary>
        /// Add a new card to the player's collection
        /// </summary>
        /// <param name="card">The card to add</param>
        public void AddCardToCollection(InGameCard card)
        {
            cardCollection.Add(card);
            Debug.Log($"Added card {card.CardName} to collection");
        }

        /// <summary>
        /// Check if the current deck is valid for play
        /// </summary>
        /// <returns>True if deck is valid</returns>
        public bool IsDeckValid()
        {
            // Check deck size
            if (currentDeck.Count < minDeckSize || currentDeck.Count > maxDeckSize)
            {
                Debug.LogWarning($"Deck size ({currentDeck.Count}) is outside valid range ({minDeckSize}-{maxDeckSize})");
                return false;
            }
            
            // TODO: Implement any additional validation rules
            
            return true;
        }

        /// <summary>
        /// Save the current deck configuration
        /// </summary>
        public void SaveDeck()
        {
            // TODO: Implement deck saving logic
            Debug.Log("Deck saved");
        }

        /// <summary>
        /// Load a previously saved deck configuration
        /// </summary>
        public void LoadDeck()
        {
            // TODO: Implement deck loading logic
            Debug.Log("Deck loaded");
        }

        /// <summary>
        /// Get a copy of the current deck list
        /// </summary>
        /// <returns>List of cards in the current deck</returns>
        public List<InGameCard> GetCurrentDeck()
        {
            return new List<InGameCard>(currentDeck); // Return a copy to prevent external modification
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Add starter cards for the Farmer class
        /// </summary>
        private void AddFarmerStarterCards()
        {
            // TODO: Implement with actual card data
            Debug.Log("Added Farmer starter cards");
        }

        /// <summary>
        /// Add starter cards for the Knight class
        /// </summary>
        private void AddKnightStarterCards()
        {
            // TODO: Implement with actual card data
            Debug.Log("Added Knight starter cards");
        }

        /// <summary>
        /// Add starter cards for the Priest class
        /// </summary>
        private void AddPriestStarterCards()
        {
            // TODO: Implement with actual card data
            Debug.Log("Added Priest starter cards");
        }

        /// <summary>
        /// Add starter cards for the Duelist class
        /// </summary>
        private void AddDuelistStarterCards()
        {
            // TODO: Implement with actual card data
            Debug.Log("Added Duelist starter cards");
        }

        /// <summary>
        /// Add starter cards for the Assassin class
        /// </summary>
        private void AddAssassinStarterCards()
        {
            // TODO: Implement with actual card data
            Debug.Log("Added Assassin starter cards");
        }

        #endregion
    }
}
