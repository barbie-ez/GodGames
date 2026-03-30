namespace GodGames.Domain.Enums;

public enum PersonalityTrait
{
    Brave = 0,    // +10% combat event selection, +5% combat outcome bonus
    Cautious = 1, // +10% exploration/rest events, -5% damage taken
    Reckless = 2, // +20% combat, higher variance on outcomes
    Cunning = 3,  // +15% loot events, better intervention parse results
}
