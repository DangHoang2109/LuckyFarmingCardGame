using System.Collections.Generic;
using UnityEngine;

namespace LuckyAdventure.GameCards
{
    /// <summary>
    /// Manages the collection of card config scriptable objects
    /// </summary>
    [CreateAssetMenu(fileName = "CardConfigsSO", menuName = "Lucky Fantasy/Card DB")]
    public class CardConfigsSO : ScriptableObject
    {
        private static CardConfigsSO _instance;

        public static CardConfigsSO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<CardConfigsSO>("Configs/CardConfigsSO");
                }
                return _instance;
            }
        }
        
        [SerializeField] private List<CardConfigSO> allCardData = new List<CardConfigSO>();
        [SerializeField] private GameObject cardPrefab; // Prefab with InGameCard component

        // Dictionary for faster lookup by ID
        private Dictionary<string, CardConfigSO> cardDataById = new Dictionary<string, CardConfigSO>();

        private void Awake()
        {
            // Build lookup dictionary
            foreach (var cardData in allCardData)
            {
                cardData.GenerateUniqueIdIfNeeded();
                cardDataById[cardData.UniqueCardId] = cardData;
            }
        }

        /// <summary>
        /// Get card data by its unique ID
        /// </summary>
        /// <param name="cardId">Unique ID of the card data</param>
        /// <returns>The card data, or null if not found</returns>
        public CardConfigSO GetCardDataById(string cardId)
        {
            if (cardDataById.TryGetValue(cardId, out CardConfigSO data))
            {
                return data;
            }
            return null;
        }

        /// <summary>
        /// Create an instance of InGameCard from card data
        /// </summary>
        /// <param name="cardId">Unique ID of the card data</param>
        /// <returns>The created InGameCard instance, or null if data not found</returns>
        public InGameCard CreateCardInstance(string cardId)
        {
            CardConfigSO configSo = GetCardDataById(cardId);
            if (configSo == null)
            {
                Debug.LogWarning($"Card data with ID {cardId} not found");
                return null;
            }

            return configSo.CreateCardInstance(cardPrefab);
        }

        /// <summary>
        /// Get all card data available for a specific character class
        /// </summary>
        /// <param name="characterClass">The character class to filter by</param>
        /// <returns>List of card data available for the class</returns>
        public List<CardConfigSO> GetCardsForClass(InGameCard.CharacterClass characterClass)
        {
            List<CardConfigSO> result = new List<CardConfigSO>();
            
            foreach (var cardData in allCardData)
            {
                if (cardData.IsAvailableForClass(characterClass))
                {
                    result.Add(cardData);
                }
            }
            
            return result;
        }

        /// <summary>
        /// Get all card data of a specific type
        /// </summary>
        /// <param name="cardType">The type of cards to filter by</param>
        /// <returns>List of card data of the specified type</returns>
        public List<CardConfigSO> GetCardsByType(InGameCard.CardType cardType)
        {
            return allCardData.FindAll(card => card.CardType == cardType);
        }

        /// <summary>
        /// Get all card data of a specific rarity
        /// </summary>
        /// <param name="rarity">The rarity to filter by</param>
        /// <returns>List of card data of the specified rarity</returns>
        public List<CardConfigSO> GetCardsByRarity(InGameCard.CardRarity rarity)
        {
            return allCardData.FindAll(card => card.Rarity == rarity);
        }
    }
}
