using System;
using System.Collections.Generic;
using LuckyFantasy;
using UnityEngine;

namespace LuckyAdventure.GameCards
{
    /// <summary>
    /// Scriptable object that defines a card's data.
    /// This allows for card definitions to be created and managed in the Unity editor.
    /// </summary>
    [CreateAssetMenu(fileName = "New Card", menuName = "Lucky Fantasy/Card Config")]
    public class CardConfigSO : ScriptableObject
    {
        #region Fields

        [Header("Basic Card Information")]
        [SerializeField] private string uniqueCardId; // Unique identifier for this card definition
        [SerializeField] private string cardName;
        [SerializeField] [TextArea(2, 4)] private string description;
        [SerializeField] private Sprite artwork;
        [SerializeField] private int mpCost;
        [SerializeField] private InGameCard.CardType cardType;
        [SerializeField] private InGameCard.CardRarity rarity;

        [Header("Class Restrictions")]
        [SerializeField] private List<InGameCard.CharacterClass> availableForClasses = new List<InGameCard.CharacterClass>();

        [Header("Effects")]
        [SerializeField] private List<CardEffectData> effects = new List<CardEffectData>();
        [SerializeField] private List<CardSynergyData> synergies = new List<CardSynergyData>();

        #endregion

        #region Properties

        public string CardName => cardName;
        public string Description => description;
        public Sprite Artwork => artwork;
        public int MPCost => mpCost;
        public InGameCard.CardType CardType => cardType;
        public InGameCard.CardRarity Rarity => rarity;
        public string UniqueCardId => uniqueCardId;
        public bool IsClassSpecific => availableForClasses != null && availableForClasses.Count > 0;
        public List<InGameCard.CharacterClass> AvailableForClasses => availableForClasses;
        public List<CardEffectData> Effects => effects;
        public List<CardSynergyData> Synergies => synergies;

        #endregion

        #region Methods

        /// <summary>
        /// Check if this card is available for a specific character class
        /// </summary>
        /// <param name="characterClass">Character class to check</param>
        /// <returns>True if the card is available for this class</returns>
        public bool IsAvailableForClass(InGameCard.CharacterClass characterClass)
        {
            // If not class specific, it's available for all classes
            if (!IsClassSpecific)
            {
                return true;
            }

            // Otherwise, check if this class is in the available classes list
            return availableForClasses.Contains(characterClass);
        }

        /// <summary>
        /// Create an instance of InGameCard from this data
        /// </summary>
        /// <returns>A configured InGameCard component</returns>
        public InGameCard CreateCardInstance(GameObject cardPrefab)
        {
            // Instantiate the card prefab
            GameObject cardObject = Instantiate(cardPrefab);
            InGameCard card = cardObject.GetComponent<InGameCard>();

            if (card == null)
            {
                Debug.LogError("Card prefab does not have an InGameCard component");
                return null;
            }

            // Configure the card instance
            ConfigureCardInstance(card);

            return card;
        }

        /// <summary>
        /// Configure an existing InGameCard instance with this data
        /// </summary>
        /// <param name="cardInstance">The card instance to configure</param>
        public void ConfigureCardInstance(InGameCard cardInstance)
        {
            // TODO: Implement configuration logic
            // This would set up the card with this data's properties
            // Would need to handle serialization/deserialization between CardData and InGameCard
            
            Debug.Log($"Configured card instance with data for {cardName}");
        }

        /// <summary>
        /// Generate a unique ID for this card if one doesn't exist
        /// </summary>
        public void GenerateUniqueIdIfNeeded()
        {
            if (string.IsNullOrEmpty(uniqueCardId))
            {
                uniqueCardId = Guid.NewGuid().ToString();
            }
        }

        #endregion
    }

    /// <summary>
    /// Data structure for card effects that can be serialized in the Unity editor
    /// </summary>
    [Serializable]
    public class CardEffectData
    {
        [SerializeField] private CardEffect.EffectType effectType;
        [SerializeField] private float baseValue;
        [SerializeField] private int duration = 1; // Number of turns the effect lasts
        [SerializeField] private CardEffect.TargetType targetType;
        [SerializeField] [TextArea(1, 3)] private string customEffectDescription;

        public CardEffect.EffectType EffectType => effectType;
        public float BaseValue => baseValue;
        public int Duration => duration;
        public CardEffect.TargetType TargetType => targetType;
        public string CustomEffectDescription => customEffectDescription;

        /// <summary>
        /// Create a CardEffect instance from this data
        /// </summary>
        /// <returns>The created CardEffect</returns>
        public CardEffect CreateEffect()
        {
            return new CardEffect(effectType, baseValue, duration, targetType);
        }
    }

    /// <summary>
    /// Data structure for card synergies that can be serialized in the Unity editor
    /// </summary>
    [Serializable]
    public class CardSynergyData
    {
        [SerializeField] private string synergyName;
        [SerializeField] private string[] synergyCardIds; // IDs of cards that synergize with this one
        [SerializeField] private CardEffectData synergyEffect; // The effect applied when synergy is triggered
        [SerializeField] [TextArea(1, 3)] private string synergyDescription;

        public string SynergyName => synergyName;
        public string[] SynergyCardIds => synergyCardIds;
        public CardEffectData SynergyEffect => synergyEffect;
        public string SynergyDescription => synergyDescription;

        /// <summary>
        /// Create a CardSynergy instance from this data
        /// </summary>
        /// <returns>The created CardSynergy</returns>
        public CardSynergy CreateSynergy()
        {
            // This is a placeholder, as CardSynergy doesn't have a public constructor
            // In a real implementation, you'd need to create a factory method or extend CardSynergy
            return null;
        }
    }
}
