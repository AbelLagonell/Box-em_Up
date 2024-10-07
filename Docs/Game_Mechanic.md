# Phase 1: Mechanic Summary
    The game will revolve around trying to get the highest score. The mechanic that would help this is the upgrade and gadget system. This is of course split into two, gadgets and upgrades. The main difference is how they are used within the game, with upgrades being acquired within the shops in between rounds and gadgets being dropped or bought in between the rounds as well. Upgrades pertain to different stats of the character so like Attack Power, Attack Speed, Defense, and Health being upgradeable. Gadgets will be more as consumables and so will be ability based, more along the lines of throwable objects and abilities that can be used up. This hopefully gives the player enough of a driving force each game to hone their skills and experiment with different gadgets and upgrades each run.

# Phase 2: Game State Variables
- Total score
- Combo Count
- Combo Multiplier
- Count of Alive Enemies
- List of Enemy Waves
- Wave Count
- List of Available Upgrades
- List of Available Gadgets
- List of Current Upgrades
- List of Current Gadgets
- Location of player within arena
- Player Health
- Player Attack
- Player Attack Speed
- Player Defense

# Phase 3: Initial Feature Set
- Player Scores points & increases Multiplier on successful hit
- Player loses health & resets Multiplier on getting hit (DMG calculated = AP - DF)
- Player can buy upgrades with score accumulated
- Upgrades can augment the player's stats (Attack Power, Attack Speed, Defense, Health)
- Gadgets can be bought or found by killing Enemies
- Player can view currently bought upgrades and Gadgets
- player can block an incoming attack to keep health and score Multiplier
- Enemies will spawn in any location outside of the player's view
- Enemies will come toward the player and try to hit them
- Enemies if hit will stop animation for hit and be in a stunned animation
- Player can Move (2D)
- Player can pause into a pause menu (End run, Settings)
- Score is saved after each game
- Player can view previuos High Scores
- Player does inputs to initiate attacks
- Attacks can combo into each other
- Upgrades are randomly generated and obtained
- Upgrades can be bought after 5 rounds

# Phase 4: Board Game

## Description: 
    The board game consists of cards for all upgrades and gadgets. Upgrades are for the four core stats as described in the phase 3, and gadgets are shurikens which can attack any Enemy and a sweeping attack that can attack mulitple lanes of enemies. The game consists of lanes where the enemies can be 'spawned' in. The number of enemies is determined by the wave count plus a d6 in which the game master would place the enemies in which ever one of the 5 lanes. The player can attack once per turn unless given upgrades and once they get shurikens they can then also attack with one shuriken unless given an upgrade. Upgrades that augment attack speed increment how many times it can attack. Both gadgets can be upgraded in attack power along with the player as well. The player can also upgrade their health and defense.

## Findings: 
    After playing the board game I noticed that my initial time of 5 rounds per upgrade buy was too much, it gave the player too much time to accumulate wealth to buy mostly everything in the shop but the shop prices tied to the current wave count was a good idea. The cost allowed the player to make decisions on what they should be buying. The upgrade shop also needs to update with what they have because having the random pick of everything without any pickups of random item pieces. Another thing that needs to be addressed is the wave enemy spawn rate. The predetermined spawn rate was too fast, which led to mid board game changing the spawn rate to what felt doable for his current equipment. In the actual game this would probably mean is a slower enemy spawn rate curve. The other thing I noticed while playing is that one of the mechanics was not the best explained and thus when introducing new gadgets in the system there should be a description and an indication of how to use it. Another thing that was apperant was that the scores should not be on hit but on death and the multiplier is on hit so that way the player can build up combo and get that payoff. The score system being the way you buy stuff is an interesting concept that was meant to be explored but not enough time was given to it to make sure that it was expressed.
