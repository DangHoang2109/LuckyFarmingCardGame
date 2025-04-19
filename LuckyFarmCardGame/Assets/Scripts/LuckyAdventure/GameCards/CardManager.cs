using System;
using System.Collections.Generic;
using LuckyAdventure.GameCards;
using LuckyAdventure.GameFlow;
using UnityEngine;

namespace LuckyFantasy
{
    /// <summary>
    /// Manages the player's deck, hand, and card interactions during gameplay.
    /// Handles card drawing, playing, and resolving according to the turn structure.
    /// </summary>
    public class CardManager : MonoBehaviour
    {
        public static CardManager Instance;

        #region Fields

        [Header("Card References")]
        [SerializeField] private Transform cardHandParent; // UI parent for hand cards
        [SerializeField] private Transform cardPaletteParent; // UI parent for palette cards

        [Header("Settings")]
        [SerializeField] private int maxHandSize = 5; // Maximum number of cards in hand
        [SerializeField] private float cardPlayDelay = 0.2f; // Delay between playing cards
        
        // Card Collections
        private List<InGameCard> deckCards = new List<InGameCard>(); // Cards in the deck
        private List<InGameCard> handCards = new List<InGameCard>(); // Cards in hand
        private List<InGameCard> paletteCards = new List<InGameCard>(); // Cards waiting to be resolved
        private List<InGameCard> discardPile = new List<InGameCard>(); // Cards in discard pile
        
        // References
        private MindPointsSystem mpSystem => MindPointsSystem.Instance;

        // State
        private bool isPlayerTurn = false;
        private bool isResolvingCards = false;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            InitializeDeck();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize the player's deck at the start of a run
        /// </summary>
        public void InitializeDeck()
        {
            // TODO: Load deck from player's collection or character class
            Debug.Log("Initializing player deck");
            ShuffleDeck();
        }

        /// <summary>
        /// Called at the start of player's turn to draw cards and prepare
        /// </summary>
        public void StartPlayerTurn()
        {
            isPlayerTurn = true;
            
            // Draw cards until hand is full
            DrawCardsToHandLimit();
            
            // TODO: Apply any start-of-turn effects
            
            Debug.Log("Player turn started");
        }

        /// <summary>
        /// Draw cards until the hand is at maximum capacity
        /// </summary>
        public void DrawCardsToHandLimit()
        {
            int cardsToDraw = maxHandSize - handCards.Count;
            
            for (int i = 0; i < cardsToDraw; i++)
            {
                DrawCard();
            }
        }

        /// <summary>
        /// Draw a single card from the deck
        /// </summary>
        /// <returns>The drawn card, or null if deck is empty</returns>
        public InGameCard DrawCard()
        {
            if (deckCards.Count == 0)
            {
                // If deck is empty, shuffle discard pile into deck
                if (discardPile.Count > 0)
                {
                    ShuffleDiscardIntoDeck();
                }
                else
                {
                    Debug.LogWarning("Cannot draw card: Deck and discard pile are empty");
                    return null;
                }
            }
            
            // Draw the top card
            InGameCard drawnCard = deckCards[0];
            deckCards.RemoveAt(0);
            
            // Add to hand
            handCards.Add(drawnCard);
            
            // TODO: Position the card in the UI
            
            Debug.Log($"Drew card: {drawnCard.CardName}");
            return drawnCard;
        }

        /// <summary>
        /// Play a card from the hand to the palette
        /// </summary>
        /// <param name="cardIndex">Index of the card in the hand</param>
        /// <returns>True if the card was played successfully</returns>
        public bool PlayCard(int cardIndex)
        {
            if (!isPlayerTurn)
            {
                Debug.LogWarning("Cannot play card: Not player's turn");
                return false;
            }
            
            if (cardIndex < 0 || cardIndex >= handCards.Count)
            {
                Debug.LogWarning($"Invalid card index: {cardIndex}");
                return false;
            }
            
            InGameCard card = handCards[cardIndex];
            
            // Check if player has enough MP to play the card
            if (!card.CanPlay(mpSystem.CurrentMP))
            {
                Debug.LogWarning($"Not enough MP to play card: {card.CardName}");
                return false;
            }
            
            // Remove from hand
            handCards.RemoveAt(cardIndex);
            
            // Add to palette
            paletteCards.Add(card);
            card.AddToPalette();
            
            // Deduct MP cost
            mpSystem.SpendMP(card.MPCost);
            
            // TODO: Add card to palette UI
            
            Debug.Log($"Played card: {card.CardName}");
            return true;
        }

        /// <summary>
        /// End the player's turn and resolve all cards in the palette
        /// </summary>
        public void EndPlayerTurn()
        {
            if (!isPlayerTurn)
            {
                Debug.LogWarning("Cannot end turn: Not player's turn");
                return;
            }
            
            isPlayerTurn = false;
            
            // Start resolving cards
            StartResolveCards();
        }

        /// <summary>
        /// Start the card resolution process
        /// </summary>
        public void StartResolveCards()
        {
            if (isResolvingCards)
            {
                Debug.LogWarning("Already resolving cards");
                return;
            }
            
            isResolvingCards = true;
            
            // TODO: Implement coroutine for card resolution with delay
            ResolveAllCards();
        }

        /// <summary>
        /// Resolve all cards in the palette
        /// </summary>
        public void ResolveAllCards()
        {
            Debug.Log($"Resolving {paletteCards.Count} cards in palette");
            var gameManager = InGameManager.Instance;
            // Process all cards in the palette
            foreach (var card in paletteCards)
            {
                // Apply card effects
                card.Play(gameManager);
                
                // Move to discard pile
                discardPile.Add(card);
            }
            
            // Clear the palette
            paletteCards.Clear();
            
            // Convert any remaining MP to shields
            ConvertRemainingMPToShields();
            
            isResolvingCards = false;
        }

        /// <summary>
        /// Convert any remaining MP to shields at the end of turn
        /// </summary>
        public void ConvertRemainingMPToShields()
        {
            int remainingMP = mpSystem.CurrentMP;
            if (remainingMP > 0)
            {
                // Use InGameManager to apply shields to the player
                InGameManager.Instance.AddPlayerShields(remainingMP);
                mpSystem.ResetMP();
            }
        }

        /// <summary>
        /// Shuffle the discard pile back into the deck
        /// </summary>
        public void ShuffleDiscardIntoDeck()
        {
            deckCards.AddRange(discardPile);
            discardPile.Clear();
            ShuffleDeck();
            Debug.Log("Shuffled discard pile into deck");
        }

        /// <summary>
        /// Shuffle the deck
        /// </summary>
        public void ShuffleDeck()
        {
            // Fisher-Yates shuffle algorithm
            System.Random rng = new System.Random();
            int n = deckCards.Count;
            
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                InGameCard temp = deckCards[k];
                deckCards[k] = deckCards[n];
                deckCards[n] = temp;
            }
            
            Debug.Log("Deck shuffled");
        }

        /// <summary>
        /// Add a card to the player's deck
        /// </summary>
        /// <param name="card">The card to add</param>
        public void AddCardToDeck(InGameCard card)
        {
            deckCards.Add(card);
            ShuffleDeck();
            Debug.Log($"Added card to deck: {card.CardName}");
        }

        /// <summary>
        /// Remove a card from the player's deck
        /// </summary>
        /// <param name="cardId">The ID of the card to remove</param>
        /// <returns>True if the card was removed successfully</returns>
        public bool RemoveCardFromDeck(string cardId)
        {
            for (int i = 0; i < deckCards.Count; i++)
            {
                if (deckCards[i].CardId == cardId)
                {
                    deckCards.RemoveAt(i);
                    Debug.Log($"Removed card with ID {cardId} from deck");
                    return true;
                }
            }
            
            Debug.LogWarning($"Card with ID {cardId} not found in deck");
            return false;
        }

        /// <summary>
        /// Get the current count of cards in the player's deck
        /// </summary>
        /// <returns>Number of cards in the deck</returns>
        public int GetDeckCount()
        {
            return deckCards.Count;
        }

        /// <summary>
        /// Get the current count of cards in the player's hand
        /// </summary>
        /// <returns>Number of cards in hand</returns>
        public int GetHandCount()
        {
            return handCards.Count;
        }
        
        /// <summary>
        /// Check if a specific card in hand can be played
        /// </summary>
        /// <param name="cardIndex">Index of the card in the hand</param>
        /// <returns>True if the card can be played</returns>
        public bool CanPlayCardAt(int cardIndex)
        {
            if (cardIndex < 0 || cardIndex >= handCards.Count)
            {
                return false;
            }
            
            // Check if player has enough MP to play this card
            InGameCard card = handCards[cardIndex];
            return card.CanPlay(mpSystem.CurrentMP);
        }
        
        /// <summary>
        /// Get the count of cards in hand that can currently be played
        /// </summary>
        /// <returns>Number of playable cards</returns>
        public int GetPlayableCardsCount()
        {
            int playableCount = 0;
            int availableMP = mpSystem.CurrentMP;
            
            foreach (var card in handCards)
            {
                if (card.CanPlay(availableMP))
                {
                    playableCount++;
                }
            }
            
            return playableCount;
        }

        #endregion
    }
}
