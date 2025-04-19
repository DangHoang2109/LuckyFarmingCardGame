using System;
using System.Collections.Generic;
using LuckyAdventure.GameFlow;
using UnityEngine;

namespace LuckyAdventure.GameCards
{
    /// <summary>
    /// Represents a card in the Lucky Fantasy game.
    /// Cards are the main gameplay element used during combat.
    /// </summary>
    public class InGameCard : MonoBehaviour
    {
        #region Card Properties

        [Header("Basic Card Information")]
        [SerializeField] private string cardName;
        [SerializeField] private string description;
        [SerializeField] private Sprite cardArtwork;
        [SerializeField] private int mpCost; // Mind Points cost to play this card
        [SerializeField] private CardType cardType;
        [SerializeField] private CardRarity rarity;
        [SerializeField] private int cardLevel = 1; // Cards start at level 1 and can be upgraded
        [SerializeField] private string cardId; // Unique identifier for this card

        [Header("Card Effects")]
        [SerializeField] private List<CardEffect> cardEffects = new List<CardEffect>();
        [SerializeField] private List<CardSynergy> synergies = new List<CardSynergy>();

        // Runtime properties
        private bool isPlayable = true;
        private CharacterClass ownerClass; // The class this card belongs to

        #endregion

        #region Enums

        /// <summary>
        /// Types of cards as defined in the GDD
        /// </summary>
        public enum CardType
        {
            Attack,     // Deal damage to enemies
            Defense,    // Create shields or reduce incoming damage
            Healing,    // Restore player health points
            Utility,    // Provide special effects
            StatusEffect // Apply debuffs to enemies or buffs to the player
        }

        /// <summary>
        /// Card rarities, determining base power level
        /// </summary>
        public enum CardRarity
        {
            Common,
            Uncommon,
            Rare,
            Epic,
            Legendary
        }

        /// <summary>
        /// Character classes that can use specific cards
        /// </summary>
        public enum CharacterClass
        {
            Farmer,     // Starting class
            MaleKnight,
            FemaleKnight,
            MalePriest,
            FemalePriest,
            MaleDuelist,
            FemaleDuelist,
            MaleAssassin,
            FemaleAssassin
        }

        #endregion

        #region Properties (Getters/Setters)

        public string CardName => cardName;
        public string Description => description;
        public Sprite CardArtwork => cardArtwork;
        public int MPCost => mpCost;
        public CardType Type => cardType;
        public CardRarity Rarity => rarity;
        public int CardLevel => cardLevel;
        public string CardId => cardId;
        public bool IsPlayable => isPlayable;
        public CharacterClass OwnerClass => ownerClass;
        public List<CardEffect> CardEffects => cardEffects;
        public List<CardSynergy> Synergies => synergies;

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines if this card can be played with the available MP
        /// </summary>
        /// <param name="availableMp">Current Mind Points available to the player</param>
        /// <returns>True if the card can be played, false otherwise</returns>
        public bool CanPlay(int availableMp)
        {
            return isPlayable && availableMp >= mpCost;
        }

        /// <summary>
        /// Play this card and apply its effects
        /// </summary>
        /// <param name="InGameManager">Reference to the game manager to apply effects</param>
        /// <returns>The MP cost of playing this card</returns>
        public int Play(InGameManager InGameManager)
        {
            // Apply card effects
            foreach (var effect in cardEffects)
            {
                effect.ApplyEffect(InGameManager);
            }

            // Generate card points when played
            GenerateCardPoints();

            // Return the MP cost for deduction from player's MP
            return mpCost;
        }

        /// <summary>
        /// Add this card to the palette (waiting area before resolution)
        /// </summary>
        public void AddToPalette()
        {
            // TODO: Implement palette adding logic
            Debug.Log($"Card {cardName} added to palette, waiting for resolution");
        }

        /// <summary>
        /// Upgrade the card by increasing its level
        /// </summary>
        /// <param name="levels">Number of levels to increase</param>
        public void UpgradeCard(int levels = 1)
        {
            cardLevel += levels;
            
            // Recalculate card effects based on new level
            foreach (var effect in cardEffects)
            {
                effect.ScaleWithLevel(cardLevel);
            }

            Debug.Log($"Card {cardName} upgraded to level {cardLevel}");
        }

        /// <summary>
        /// Sync with a duplicate card to level up and reduce deck size
        /// </summary>
        /// <param name="duplicateCard">The duplicate card to sync with</param>
        public void SyncWithDuplicate(InGameCard duplicateCard)
        {
            // TODO: Implement duplicate syncing logic
            // This would increase the card's level and possibly improve effects
            if (duplicateCard.CardId == this.cardId)
            {
                UpgradeCard();
                // Logic to remove the duplicate from the deck would be handled elsewhere
            }
        }

        /// <summary>
        /// Check if this card has synergy with another card
        /// </summary>
        /// <param name="otherCard">The other card to check synergy with</param>
        /// <returns>True if there's synergy, false otherwise</returns>
        public bool HasSynergyWith(InGameCard otherCard)
        {
            foreach (var synergy in synergies)
            {
                if (synergy.CheckSynergy(otherCard))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Apply synergy effects when cards are played in sequence
        /// </summary>
        /// <param name="otherCard">The card played in sequence with this one</param>
        /// <param name="InGameManager">Reference to the game manager</param>
        public void ApplySynergyEffects(InGameCard otherCard, InGameManager InGameManager)
        {
            foreach (var synergy in synergies)
            {
                if (synergy.CheckSynergy(otherCard))
                {
                    synergy.ApplySynergyEffect(InGameManager);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generate card points when the card is played
        /// </summary>
        private void GenerateCardPoints()
        {
            // TODO: Implement card points generation logic
            // This would be used for meta-progression systems
            int pointsGenerated = CalculateCardPoints();
            Debug.Log($"Card {cardName} generated {pointsGenerated} card points");
            
            // Points would be added to a player's total elsewhere
        }

        /// <summary>
        /// Calculate the card points generated based on card properties
        /// </summary>
        /// <returns>The number of card points generated</returns>
        private int CalculateCardPoints()
        {
            // TODO: Implement actual calculation
            // Basic calculation based on rarity and level
            int basePoints = (int)rarity + 1;
            return basePoints * cardLevel;
        }

        #endregion
    }

    /// <summary>
    /// Represents an effect that a card can have during gameplay
    /// </summary>
    [Serializable]
    public class CardEffect
    {
        [SerializeField] private EffectType effectType;
        [SerializeField] private float baseValue;
        [SerializeField] private int duration = 1; // Number of turns the effect lasts (1 for immediate effects)
        [SerializeField] private TargetType targetType;

        private float scaledValue; // Value after scaling with card level

        public enum EffectType
        {
            Damage,         // Deal damage to enemies
            Shield,         // Create shield for defense
            Healing,        // Restore health
            ModifyMP,       // Change MP (positive or negative)
            ApplyStatus,    // Apply a status effect
            DrawCard,       // Draw additional cards
            DiscardCard,    // Force discard
            MultiplyDamage, // Multiply subsequent damage
            AreaEffect,     // Affect multiple targets
            Other           // Custom effects
        }

        public enum TargetType
        {
            Self,           // Target the player
            SingleEnemy,    // Target a single enemy
            AllEnemies,     // Target all enemies
            Random,         // Target randomly
            Custom          // Custom targeting logic
        }

        public EffectType Type => effectType;
        public float BaseValue => baseValue;
        public float ScaledValue => scaledValue;
        public int Duration => duration;
        public TargetType Target => targetType;

        public CardEffect(EffectType type, float value, int duration = 1, TargetType target = TargetType.SingleEnemy)
        {
            this.effectType = type;
            this.baseValue = value;
            this.duration = duration;
            this.targetType = target;
            this.scaledValue = value; // Initially scaled value equals base value
        }

        /// <summary>
        /// Apply this effect in the game
        /// </summary>
        /// <param name="InGameManager">Reference to the game manager</param>
        public void ApplyEffect(InGameManager InGameManager)
        {
            // TODO: Implement actual effect application logic
            switch (effectType)
            {
                case EffectType.Damage:
                    // Apply damage to targets
                    Debug.Log($"Applying {scaledValue} damage to {targetType}");
                    break;
                case EffectType.Shield:
                    // Create shields
                    Debug.Log($"Creating {scaledValue} shield for {targetType}");
                    break;
                case EffectType.Healing:
                    // Heal targets
                    Debug.Log($"Healing {scaledValue} HP for {targetType}");
                    break;
                case EffectType.ApplyStatus:
                    // Apply status effects
                    Debug.Log($"Applying status effect to {targetType} for {duration} turns");
                    break;
                default:
                    Debug.Log($"Applying effect type {effectType}");
                    break;
            }
        }

        /// <summary>
        /// Scale the effect value based on card level
        /// </summary>
        /// <param name="cardLevel">Current level of the card</param>
        public void ScaleWithLevel(int cardLevel)
        {
            // Basic scaling formula - can be adjusted for balance
            scaledValue = baseValue * (1 + (cardLevel - 1) * 0.2f);
        }
    }

    /// <summary>
    /// Represents a synergy between cards that can trigger special effects
    /// </summary>
    [Serializable]
    public class CardSynergy
    {
        [SerializeField] private string synergyName;
        [SerializeField] private string[] synergyCardIds; // IDs of cards that synergize with this one
        [SerializeField] private CardEffect synergyEffect; // The effect applied when synergy is triggered

        public string SynergyName => synergyName;
        public string[] SynergyCardIds => synergyCardIds;
        public CardEffect SynergyEffect => synergyEffect;

        /// <summary>
        /// Check if this card has synergy with another card
        /// </summary>
        /// <param name="otherCard">The other card to check synergy with</param>
        /// <returns>True if there's synergy, false otherwise</returns>
        public bool CheckSynergy(InGameCard otherCard)
        {
            foreach (var cardId in synergyCardIds)
            {
                if (otherCard.CardId == cardId)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Apply the synergy effect
        /// </summary>
        /// <param name="InGameManager">Reference to the game manager</param>
        public void ApplySynergyEffect(InGameManager InGameManager)
        {
            Debug.Log($"Applying synergy effect {synergyName}");
            synergyEffect.ApplyEffect(InGameManager);
        }
    }
}
