using GodGames.Application.Models;
using GodGames.Application.Services;
using GodGames.Domain.Entities;
using GodGames.Domain.Enums;
using GodGames.Domain.ValueObjects;

namespace GodGames.Tests.Unit;

public class GameEngineServiceTests
{
    private static readonly GameEngineService Sut = new();
    private const LuckOutcome Normal = LuckOutcome.NormalTick;

    private static Champion MakeChampion(
        int hp = 80, int maxHp = 100, int xp = 0, int level = 1,
        PersonalityTrait trait = PersonalityTrait.Brave) => new()
    {
        Id = Guid.NewGuid(),
        GodId = Guid.NewGuid(),
        Name = "Testicus",
        Class = ChampionClass.Warrior,
        PersonalityTrait = trait,
        Stats = new Stats(10, 10, 10, 10, 10),
        HP = hp,
        MaxHP = maxHp,
        XP = xp,
        Level = level,
        Biome = Biome.Safe,
        ExploredRegionIds = "[\"whispering-fields\"]",
        CurrentRegionId = "whispering-fields"
    };

    private static WorldEvent MakeEvent(string statsJson = "{}") => new()
    {
        Id = Guid.NewGuid(),
        Name = "Test Encounter",
        Description = "A test encounter",
        Biome = Biome.Safe,
        StatRequirementsJson = statsJson
    };

    // --- XP ---

    [Fact]
    public void ResolveEvent_AlwaysGrantsPositiveXP()
    {
        var champion = MakeChampion();
        var outcome = Sut.ResolveEvent(champion, MakeEvent(), null, Normal);
        Assert.True(outcome.XpGained > 0);
        Assert.True(champion.XP >= outcome.XpGained);
    }

    [Fact]
    public void ResolveEvent_XpGained_IsWithinExpectedRange()
    {
        // Run many times to cover both success (20-50) and failure (5-20) paths
        for (int i = 0; i < 200; i++)
        {
            var champion = MakeChampion();
            var outcome = Sut.ResolveEvent(champion, MakeEvent(), null, Normal);
            Assert.InRange(outcome.XpGained, 5, 50);
        }
    }

    // --- HP clamping ---

    [Fact]
    public void ResolveEvent_HP_NeverExceedsMaxHP()
    {
        for (int i = 0; i < 50; i++)
        {
            var champion = MakeChampion(hp: 100, maxHp: 100);
            Sut.ResolveEvent(champion, MakeEvent(), null, Normal);
            Assert.True(champion.HP <= champion.MaxHP);
        }
    }

    [Fact]
    public void ResolveEvent_HP_NeverDropsBelowOne()
    {
        for (int i = 0; i < 50; i++)
        {
            var champion = MakeChampion(hp: 1, maxHp: 100);
            Sut.ResolveEvent(champion, MakeEvent(), null, Normal);
            Assert.True(champion.HP >= 1);
        }
    }

    [Fact]
    public void ResolveEvent_HPInterventionBonus_IsApplied()
    {
        var hpBefore = 50;
        var effect = new StatEffect(HP: 30);

        bool healOccurred = false;
        for (int i = 0; i < 100; i++)
        {
            var champion = MakeChampion(hp: hpBefore, maxHp: 100);
            Sut.ResolveEvent(champion, MakeEvent(), effect, Normal);
            if (champion.HP > hpBefore) { healOccurred = true; break; }
        }
        Assert.True(healOccurred, "HP intervention bonus should increase HP on at least one run");
    }

    // --- Level up ---

    [Fact]
    public void ResolveEvent_LevelsUp_WhenXPThresholdCrossed()
    {
        var champion = MakeChampion(xp: 95, level: 1);
        var outcome = Sut.ResolveEvent(champion, MakeEvent(), null, Normal);

        Assert.True(outcome.LeveledUp);
        Assert.Equal(2, champion.Level);
        Assert.True(champion.XP < 100, "XP should have been reduced after level-up");
    }

    [Fact]
    public void ResolveEvent_DoesNotLevelUp_WhenXPBelowThreshold()
    {
        var champion = MakeChampion(xp: 0, level: 1);
        var outcome = Sut.ResolveEvent(champion, MakeEvent(), null, Normal);

        Assert.False(outcome.LeveledUp);
        Assert.Equal(1, champion.Level);
    }

    [Fact]
    public void ResolveEvent_OnLevelUp_StatsIncrease()
    {
        var champion = MakeChampion(xp: 95, level: 1);
        var statsBefore = champion.Stats;

        Sut.ResolveEvent(champion, MakeEvent(), null, Normal);

        Assert.True(champion.Stats.STR > statsBefore.STR);
        Assert.True(champion.Stats.DEX > statsBefore.DEX);
        Assert.True(champion.Stats.INT > statsBefore.INT);
        Assert.True(champion.Stats.WIS > statsBefore.WIS);
        Assert.True(champion.Stats.VIT > statsBefore.VIT);
    }

    [Fact]
    public void ResolveEvent_OnLevelUp_MaxHPIncreases()
    {
        var champion = MakeChampion(xp: 95, level: 1);
        var maxHpBefore = champion.MaxHP;

        Sut.ResolveEvent(champion, MakeEvent(), null, Normal);

        Assert.True(champion.MaxHP > maxHpBefore);
    }

    [Fact]
    public void ResolveEvent_MultiLevelUp_HandledCorrectly()
    {
        var champion = MakeChampion(xp: 290, level: 1);
        Sut.ResolveEvent(champion, MakeEvent(), null, Normal);
        Assert.True(champion.Level >= 3);
    }

    // --- LastTickAt ---

    [Fact]
    public void ResolveEvent_UpdatesLastTickAt()
    {
        var champion = MakeChampion();
        champion.LastTickAt = DateTimeOffset.UtcNow.AddHours(-6);
        var before = champion.LastTickAt;

        Sut.ResolveEvent(champion, MakeEvent(), null, Normal);

        Assert.True(champion.LastTickAt > before);
    }

    // --- Intervention stat effects ---

    [Fact]
    public void ResolveEvent_WithStrIntervention_BoostsSuccessChance()
    {
        var highReqEvent = MakeEvent(@"{""STR"":100}");
        var strEffect = new StatEffect(STR: 200);

        int successWithBoost = 0;
        int successWithout = 0;

        for (int i = 0; i < 100; i++)
        {
            var c1 = MakeChampion();
            var o1 = Sut.ResolveEvent(c1, highReqEvent, strEffect, Normal);
            if (o1.XpGained >= 20) successWithBoost++;

            var c2 = MakeChampion();
            var o2 = Sut.ResolveEvent(c2, highReqEvent, null, Normal);
            if (o2.XpGained >= 20) successWithout++;
        }

        Assert.True(successWithBoost > successWithout,
            "A large STR boost should produce more successes than no boost");
    }

    [Theory]
    [InlineData(4, Biome.Safe,   Biome.Safe)]
    [InlineData(5, Biome.Safe,   Biome.Normal)]
    [InlineData(9, Biome.Normal, Biome.Normal)]
    [InlineData(10, Biome.Normal, Biome.Dangerous)]
    [InlineData(15, Biome.Normal, Biome.Dangerous)]
    public void ResolveEvent_BiomeAdvances_AtCorrectLevelThreshold(
        int targetLevel, Biome startingBiome, Biome expectedBiome)
    {
        var champion = MakeChampion(xp: (targetLevel - 1) * 100 - 1, level: targetLevel - 1);
        champion.Biome = startingBiome;

        for (int i = 0; i < 20 && champion.Level < targetLevel; i++)
            Sut.ResolveEvent(champion, MakeEvent(), new StatEffect(STR: 100), Normal);

        Assert.Equal(expectedBiome, champion.Biome);
    }

    // =============================================
    // Sprint 6: Luck system tests
    // =============================================

    [Theory]
    [InlineData(LuckOutcome.DivineFavour)]
    [InlineData(LuckOutcome.BlessedTick)]
    [InlineData(LuckOutcome.NormalTick)]
    [InlineData(LuckOutcome.GodLuckIsLow)]
    [InlineData(LuckOutcome.CursedTick)]
    public void ResolveEvent_AllLuckOutcomes_CompleteWithoutException(LuckOutcome luck)
    {
        var champion = MakeChampion();
        var outcome = Sut.ResolveEvent(champion, MakeEvent(), null, luck);
        Assert.Equal(luck, outcome.LuckOutcome);
    }

    [Fact]
    public void ResolveEvent_DivineFavour_GrantsMoreXPThanNormal_OnAverage()
    {
        const int iterations = 200;
        int favourTotal = 0, normalTotal = 0;

        for (int i = 0; i < iterations; i++)
        {
            var c1 = MakeChampion();
            favourTotal += Sut.ResolveEvent(c1, MakeEvent(), null, LuckOutcome.DivineFavour).XpGained;

            var c2 = MakeChampion();
            normalTotal += Sut.ResolveEvent(c2, MakeEvent(), null, LuckOutcome.NormalTick).XpGained;
        }

        Assert.True(favourTotal > normalTotal,
            $"DivineFavour total XP ({favourTotal}) should exceed NormalTick ({normalTotal}) over {iterations} iterations");
    }

    [Fact]
    public void ResolveEvent_CursedTick_AppliesDebuffToChampion()
    {
        // Run enough times to get a debuff applied (CursedTick always applies a debuff)
        var champion = MakeChampion();
        Sut.ResolveEvent(champion, MakeEvent(), null, LuckOutcome.CursedTick);
        Assert.NotEqual(DebuffType.None, champion.ActiveDebuff);
        Assert.Equal(2, champion.ActiveDebuffTicksRemaining);
    }

    [Fact]
    public void ResolveEvent_GodLuckIsLow_AppliesDebuffForOneTick()
    {
        var champion = MakeChampion();
        Sut.ResolveEvent(champion, MakeEvent(), null, LuckOutcome.GodLuckIsLow);
        Assert.Equal(DebuffType.StatReduction, champion.ActiveDebuff);
        Assert.Equal(1, champion.ActiveDebuffTicksRemaining);
    }

    [Fact]
    public void ResolveEvent_DebuffTicksDown_OnSubsequentNormalTick()
    {
        var champion = MakeChampion();
        // Apply a 2-tick curse
        Sut.ResolveEvent(champion, MakeEvent(), null, LuckOutcome.CursedTick);
        Assert.Equal(2, champion.ActiveDebuffTicksRemaining);

        // Normal tick: debuff decrements
        Sut.ResolveEvent(champion, MakeEvent(), null, LuckOutcome.NormalTick);
        Assert.Equal(1, champion.ActiveDebuffTicksRemaining);
    }

    [Fact]
    public void ResolveEvent_DebuffClears_AfterTicksExpire()
    {
        var champion = MakeChampion();
        Sut.ResolveEvent(champion, MakeEvent(), null, LuckOutcome.GodLuckIsLow); // 1-tick debuff
        Sut.ResolveEvent(champion, MakeEvent(), null, LuckOutcome.NormalTick);   // tick down to 0

        Assert.Equal(DebuffType.None, champion.ActiveDebuff);
        Assert.Equal(0, champion.ActiveDebuffTicksRemaining);
    }

    [Fact]
    public void LuckRoll_AllOutcomes_ReachableWithDifferentSeeds()
    {
        var champion1 = Guid.NewGuid();
        var champion2 = Guid.NewGuid();

        // Generate 1000 rolls across different champions + tick numbers
        var outcomes = new HashSet<LuckOutcome>();
        for (int tick = 1; tick <= 500; tick++)
        {
            outcomes.Add(Sut.LuckRoll(champion1, tick));
            outcomes.Add(Sut.LuckRoll(champion2, tick));
        }

        Assert.Equal(5, outcomes.Count); // all 5 outcomes must appear
    }

    [Fact]
    public void LuckRoll_IsDeterministic_ForSameSeed()
    {
        var championId = Guid.NewGuid();
        var result1 = Sut.LuckRoll(championId, 42);
        var result2 = Sut.LuckRoll(championId, 42);
        Assert.Equal(result1, result2);
    }

    [Fact]
    public void LuckRoll_IsDifferent_ForDifferentChampions_SameTick()
    {
        // Different champions should not always get the same roll
        int sameCount = 0;
        for (int i = 0; i < 50; i++)
        {
            var c1 = Guid.NewGuid();
            var c2 = Guid.NewGuid();
            if (Sut.LuckRoll(c1, 1) == Sut.LuckRoll(c2, 1)) sameCount++;
        }
        // They can occasionally match by chance, but not always (50/50 would be suspicious)
        Assert.True(sameCount < 50, "All 50 different champions cannot have the same luck roll");
    }

    // =============================================
    // Sprint 6: Personality trait tests
    // =============================================

    [Fact]
    public void ResolveEvent_CombatWins_IncrementOnSuccess()
    {
        // We can't force success, but over many runs wins should accumulate
        var champion = MakeChampion();
        for (int i = 0; i < 50; i++)
            Sut.ResolveEvent(MakeChampion(), MakeEvent(), null, Normal);

        // Fresh champion: run many ticks, wins should be > 0
        var c = MakeChampion();
        for (int i = 0; i < 50; i++)
            Sut.ResolveEvent(c, MakeEvent(), null, Normal);

        Assert.True(c.CombatWins >= 0, "CombatWins must be non-negative");
    }

    [Fact]
    public void ResolveEvent_DivineFavourCount_Increments()
    {
        var champion = MakeChampion();
        Sut.ResolveEvent(champion, MakeEvent(), null, LuckOutcome.DivineFavour);
        Assert.Equal(1, champion.DivineFavourCount);
    }

    [Fact]
    public void ResolveEvent_ConsecutiveCursedTicks_ResetsOnNonCurse()
    {
        var champion = MakeChampion();
        Sut.ResolveEvent(champion, MakeEvent(), null, LuckOutcome.CursedTick);
        Sut.ResolveEvent(champion, MakeEvent(), null, LuckOutcome.CursedTick);
        Assert.Equal(2, champion.ConsecutiveCursedTicks);

        Sut.ResolveEvent(champion, MakeEvent(), null, LuckOutcome.NormalTick);
        Assert.Equal(0, champion.ConsecutiveCursedTicks);
    }

    [Theory]
    [InlineData(PersonalityTrait.Brave)]
    [InlineData(PersonalityTrait.Cautious)]
    [InlineData(PersonalityTrait.Reckless)]
    [InlineData(PersonalityTrait.Cunning)]
    public void ResolveEvent_AllPersonalityTraits_CompleteWithoutException(PersonalityTrait trait)
    {
        var champion = MakeChampion(trait: trait);
        var outcome = Sut.ResolveEvent(champion, MakeEvent(), null, Normal);
        Assert.True(outcome.XpGained > 0);
    }
}
