# <!-- Try to use MCP tool when possible. Review Project Structure section to identify corresponding files when coding a module. -->

# Lucky Fantasy - Memory Bank

## Project Overview

**Game Title**: Lucky Fantasy

**Game Type**: Strategic turn-based card game (Mobile)

**Core Concept**: A strategic turn-based card game where players build and play decks to defeat increasingly challenging enemies. Starting as a humble Farmer and potentially evolving into specialized classes like Knights or Assassins, players must master the rhythm of drawing cards within limit mind power and employing tactical combinations.

## Project Structure

The codebase is organized into the following main modules:

### Game Cards Module (`Assets/Scripts/LuckyAdventure/GameCards/`)
- **CardConfigSO.cs**: Scriptable Object for defining individual card configurations
- **CardConfigsSO.cs**: Container for multiple card configurations, used for card collections
- **CardManager.cs**: Core manager that handles deck, hand, palette, and discard pile; manages drawing, playing, and resolving cards
- **DeckBuilder.cs**: Handles constructing and modifying player decks
- **InGameCard.cs**: Runtime representation of a card with all its properties and effects
- **MindPointsSystem.cs**: Manages the Mind Points resource system, including dice rolling mechanics

### Game Flow Module (`Assets/Scripts/LuckyAdventure/GameFlow/`)
- **InGameManager.cs**: Main game manager that orchestrates the entire game flow
- **IGameState.cs**: Interface for game state implementation
- **IState.cs**: Base interface for all state implementations
- **ITurnState.cs**: Interface for turn state implementation

#### Game States (`Assets/Scripts/LuckyAdventure/GameFlow/GameState/`)
- **GameInitState.cs**: Initializes the game components and settings
- **GameStartState.cs**: Starts a new game session
- **GamePlayerTurnState.cs**: Handles the player's turn sequence
- **GameEnemyTurnState.cs**: Manages enemy turn logic
- **GameEndState.cs**: Handles game end conditions and cleanup
- **GameResetState.cs**: Resets the game state for a new session

#### Turn States (`Assets/Scripts/LuckyAdventure/GameFlow/TurnState/`)
- **PlayerStandbyTurnState.cs**: Initial phase of player turn, rolls Mind Points and draws cards
- **PlayerMainPhaseTurnState.cs**: Main phase where player plays cards into the palette
- **PlayerResolvingTurnState.cs**: Resolves all cards played during the main phase
- **PlayerEndTurnState.cs**: Finalizes player turn and checks game conditions
- **EnemyMainPhaseTurnState.cs**: Handles enemy action execution and attacks
- **EnemyEndTurnState.cs**: Finalizes enemy turn and transitions to next state

### Game Unit Module (`Assets/Scripts/LuckyAdventure/GameUnit/`)
- **Unit.cs**: Base class for all game units (players and enemies)
- **PlayerUnit.cs**: Player-specific unit implementation with character class properties

## Game Design Pillars

1. **Strategic Card Management**: Players must carefully play cards to maximize their effectiveness within limitation in their mind conscious gauge bar. This creates meaningful decisions with each card play and encourages thoughtful planning rather than random play, making victories feel earned through clever thinking. Players also need to structure their deck effectively to maximize the good cards drawn.

2. **Character Progression & Customization**: Starting as a Farmer and evolving into specialized classes with unique abilities gives players agency in choosing their playstyle. The ability to upgrade cards and build personalized decks ensures players feel a sense of ownership and growth throughout their journey.

3. **Permadeath in Roguelike Gameplay**: Each run is a unique experience with permanent consequences, where death means starting over but with accumulated knowledge and potentially unlocked abilities. This creates tension in every decision, makes each victory meaningful, and encourages players to experiment with different strategies across multiple playthroughs.

## Core Gameplay

### Aim of the Game

Lucky Fantasy aims to deliver a strategic card experience that balances luck, skill, and player agency. The game should be approachable for casual players while offering enough depth and variety to engage dedicated players across multiple playthroughs, with each run telling a unique story of triumphs, setbacks, and strategic adaptation.

### Game Design Goals

- **Card-Based Strategic Depth with Controlled Randomness**
  - Design a card system where tactical play and thoughtful deck construction are the primary drivers of success
  - Implement the Mind Point mechanism to add a controlled element of chance that creates unique situations every turn without undermining strategy
  - Ensure that cards have clear, understandable effects with enough complexity to allow for creative combinations and synergies

- **Progressive Character Development**
  - Create a compelling progression system where players evolve from basic Farmer to specialized classes with distinct playstyles
  - Design class-specific cards and abilities that meaningfully impact gameplay and encourage experimentation
  - Balance character abilities so that all classes are viable while offering genuinely different tactical approaches

- **Engaging Roguelike Experience**
  - Craft a permadeath system that makes each defeat meaningful through persistent unlocks and knowledge gain
  - Design varied enemy encounters and boss battles that require adaptation and different strategies

- **Accessible Gameplay with Strategic Depth**
  - Ensure the core card mechanics are intuitive and easy to understand for new players
  - Include tutorials that teach fundamental concepts without overwhelming players
  - Layer in complexity gradually through enemy abilities and card synergies
  - Maintain a high skill ceiling through advanced card combinations and deck optimization

- **Satisfying Risk/Reward Balance**
  - Design the Mind Gauge system to create compelling choices between conservative and aggressive play
  - Implement bonus stages and treasure events that incentivize risk-taking
  - Ensure that bold strategies can lead to significant rewards while cautious play remains viable

- **Visually Clear Feedback**
  - Provide clear, immediate feedback for all player actions and enemy abilities
  - Develop distinctive visual language for card types, character abilities, and enemy intentions
  - Ensure UI elements clearly communicate critical information like Mind Points, health, and special effects

- **Compelling Progression and Balanced Game Economy**
  - Design a card unlocking system that keeps players motivated across multiple runs
  - Develop a rewarding in-game economy for card acquisition and upgrades
  - Balance card rarity and power to create exciting moments of discovery without power creep
  - Design monetization to enhance the experience without creating pay-to-win dynamics

## Game Mechanisms

### Turn Structure

1. **Start of Turn - Standby Phase**
   - Mind Gauge Determination: Roll 3 dice to establish the player's Mind Points (MP) for the turn
     - Standard Roll: Sum the values of all 3 dice (3-18 points)
     - Perfect Roll Bonus: If all 3 dice show the same value, multiply the total by 3 (9-54 points)
     - The resulting value determines the maximum MP the player can use during this turn
   - Apply buff/debuff on player
   - Players draw cards from their deck until their hand contains 5 cards (this limit will be extendable by meta features in the future)

2. **Action Phase - Player Main Phase**
   - Each card on player hand has an associated MP cost, Players may play cards as long as they have sufficient MP remaining in their Mind Gauge
   - Playing a card subtracts its MP cost from the Mind Gauge and that card will stack in the palette, waiting to be resolve (This allow combo)
   - Actions continue until the player chooses to end their turn by hit the End Button or cannot play any more cards

3. **Player Resolving Phase**
   - This phase resolves all cards in the palette. Any MP left will be converted into shields for defense.
   - If all enemies were killed in this phase, jump into End of Turn and check end game

4. **Enemy Action Phase - Enemy Main Phase**
   - Apply buff/debuff on enemy, status effects (poison, stun, etc.) take effect if applicable
   - Alive enemies execute their defined actions or abilities
   - Enemy attacks are applied to the player, reduced by any shield or defensive effects

5. **End of Turn**
   - Ongoing effects (buffs, debuffs) have their durations reduced by 1
   - Checking end wave / end game

### Mind Points System

Mind Points represent the mental energy required to play cards during combat. This system creates strategic depth through:

- **Resource Management**
  - Players must carefully consider which cards to play based on their Mind Gauge value
  - Higher MP rolls allow for more powerful card combinations
  - Lower MP rolls require conservative play and prioritization of efficient cards

- **Card MP Values**
  - Basic cards have lower MP costs (1-3 MP)
  - Powerful attack and utility cards have moderate costs (3-6 MP)
  - Special ability and character-specific cards have higher costs (6-10 MP)
  - Legendary/rare cards may cost 10+ MP with huge impact or powerful while having low cost, making them situational but impactful.

- **MP-Based Decision Making**
  - Strategic decisions about saving MP for defensive vs. spending MP on offense
  - Special cards may offer MP discounts under certain conditions (e.g., after defeating an enemy)

### Card Mechanism

- **Card Types and Effects**
  - **Attack Cards**: Deal damage to enemies (direct damage, area damage, conditional damage)
  - **Defense Cards**: Create shields or reduce incoming damage
  - **Healing Cards**: Restore player health points
  - **Utility Cards**: Provide special effects (manipulate MP, multiply damage, etc.)
  - **Status Effect Cards**: Apply debuffs to enemies or buffs to the player

- **Card Properties**
  - **MP Cost**: The Mind Points required to play the card
  - **Effect**: The primary function of the card
  - **Synergy Effects**: Bonus effects when played in sequence with certain other cards
  - **Level**: Cards can be upgraded to enhance their effects, player are allow to sync duplicate cards to level up it while reduce deck size to increase chance to draw good cards.
  - **Rarity**: Common, Uncommon, Rare, Epic, Legendary - determining base power level

- **Card Points and Progression**
  - Cards played during combat generate card points
  - Card points are used to upgrade existing cards or unlock new ones
  - Card progression follows defined paths based on card type and character class

### Character Classes and Progression

Characters in Lucky Fantasy begin as Farmers but can evolve into specialized classes through gameplay:

- **Farmer (Starting Class)**
  - Balanced statistics with no special abilities
  - Serves as a tutorial class to learn the game mechanics
  - HP: 70, Damage: 6, Heal: 3, Shield: 5

- **Specialized Classes**
  - **Knight**: Tank-focused with high HP and defensive capabilities
    - Male Knight: Higher shield values and defensive bonuses (HP: 115, Damage: 9, Heal: 4, Shield: 13)
    - Female Knight: Balance of offense and defense (HP: 105, Damage: 11, Heal: 6, Shield: 11)
  - **Priest**: Support-focused with healing and buffs
    - Male Priest: Strong area healing and defensive buffs (HP: 100, Damage: 7, Heal: 12, Shield: 9)
    - Female Priest: Single-target healing and debuff cleansing (HP: 100, Damage: 9, Heal: 8, Shield: 11)
  - **Duelist**: Combat-focused with parries and counterattacks
    - Male Duelist: Aggressive with high critical damage (HP: 88, Damage: 13, Heal: 5, Shield: 9)
    - Female Duelist: Control-oriented with precision strikes (HP: 92, Damage: 11, Heal: 3, Shield: 10)
  - **Assassin**: Stealth-focused with high burst damage
    - Male Assassin: Quick, lethal strikes and evasion (HP: 75, Damage: 16, Heal: 2, Shield: 6)
    - Female Assassin: Sustained damage and strategic attacks (HP: 85, Damage: 14, Heal: 4, Shield: 4)

- **Class Progression**
  - Characters level up by defeating enemies and completing stages
  - Each level provides stat improvements and access to class-specific cards
  - Special cards unlock at milestone levels, providing signature abilities
  - Players can purchase and unlock other classes in metagame, this allows monetization

### Combat Mechanics

- **Damage Calculation**
  - Base damage = Character's damage stat × Card damage multiplier
  - Critical hits = Base damage × Critical multiplier (typically 1.5x or 2x)
  - Damage reduction from enemy shield or resistance is subtracted
  - Final damage is applied to the target's HP

- **Shield Mechanics**
  - Shields absorb damage before it affects HP
  - Shields have a set value that decreases as damage is absorbed
  - Some abilities can pierce shields or gain bonuses against shielded targets
  - Unused shields may persist for a limited number of turns or until depleted

- **Special Combat States**
  - **Stunned**: Unable to act for a specified number of turns
  - **Poisoned**: Takes damage at the end of each turn
  - **Frozen**: Unable to use certain card types
  - **Vulnerable**: Takes increased damage (typically 25% more)
  - **Strengthened**: Deals increased damage (typically 25% more)

### Enemy Mechanics

- **Enemy Types**
  - **Standard Enemies**: Basic opponents with straightforward attacks
  - **Elite Enemies**: Stronger foes with special abilities
  - **Bosses**: Powerful adversaries with multiple phases or complex mechanics

- **Enemy Abilities**
  - **Basic Attack**: Direct damage to the player
  - **Special Abilities**: Unique effects such as summoning allies, applying status effects, or creating shields
  - **Reactive Abilities**: Triggered by specific player actions

- **Enemy Behavior Patterns**
  - Predictable attack patterns allow for strategic planning
  - Some enemies telegraph their next move, allowing players to counter
  - Boss fights feature phase changes at certain HP thresholds

### Bonus and Special Systems

This system helps players restructure and enhance their deck. The wave is placed following a common 10-stage structure: 4 battle waves - Bonus Stage - 4 battle waves - Boss wave.

- **Bonus Stages**
  - Special encounters offering new cards
- **Special Encounters - Merchant - Contract**
  - Merchants offering card trades or purchase by trading player MaxHP

### Roguelike Progression System

- **Run-Based Structure**
  - Each game attempt is a unique "run" through the game world
  - Death is permanent, requiring players to start a new run
  - Knowledge and upgrades carry over between runs

- **Persistent Upgrades**
  - Meta-progression systems provide permanent bonuses: Purchase cards, Unlock character, Upgrade talent tree skills, Equipment in metagate allow both monetization and progression.

- **Map Structure**
  - The game consists of multiple maps with increasing difficulty
  - Maps contain a mix of combat encounters stage, bonus stages, and boss fights stage
  - Map 1: Peaceful Forest (Introductory difficulty)
  - Map 2: Volcano (Intermediate difficulty)
  - Map 3 and beyond: Advanced environments with complex enemies

### Economy and Reward Systems

- **In-Game Currency**
  - Coins earned through combat and stage completion
  - Used for Meta-progression systems: Talent, Equipment, Characters…

## Development Notes

### TODO List
- Create detailed card database with all properties and effects
- Design UI mockups for the Mind Gauge and card palette system
- Implement first playable prototype focusing on the core card gameplay
- Develop character progression system and class specializations
- Balance MP costs and card effects
- Design first map (Peaceful Forest) with enemy encounters

### Questions for Decision
- Should unused MP convert to shield at a 1:1 ratio or different value?
- What should be the balance between randomness (dice rolls) and player agency?
- How frequently should players encounter merchants and bonus stages?
- What retention mechanics should be prioritized for F2P players?

### Implementation Priorities
1. Core card combat system with Mind Points
2. Basic character progression (Farmer → specialized class)
3. First map with standard enemies
4. Deck building and card upgrade mechanics
5. Meta-progression systems
6. Boss encounters and special stages
7. Economy and monetization features

## Reference Assets
- Mind Gauge UI design needed
- Card template designs needed
- Character progression chart needed
- Turn flowchart needed
- Enemy design table for Map 1 needed
- Starting deck compositions for each class needed