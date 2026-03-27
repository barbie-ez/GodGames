using GodGames.Application.Models;
using GodGames.Application.Services;
using GodGames.Domain.Entities;
using GodGames.Domain.Enums;
using GodGames.Domain.ValueObjects;

namespace GodGames.Tests.Unit;

public class GameEngineServiceTests
{
    private static readonly GameEngineService Sut = new();

    private static Champion MakeChampion(int hp = 80, int maxHp = 100, int xp = 0, int level = 1) => new()
    {
        Id = Guid.NewGuid(),
        GodId = Guid.NewGuid(),
        Name = "Testicus",
        Class = ChampionClass.Warrior,
        Stats = new Stats(10, 10, 10, 10, 10),
        HP = hp,
        MaxHP = maxHp,
        XP = xp,
        Level = level,
        Biome = Biome.Safe
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
        var outcome = Sut.ResolveEvent(champion, MakeEvent(), null);
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
            var outcome = Sut.ResolveEvent(champion, MakeEvent(), null);
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
            Sut.ResolveEvent(champion, MakeEvent(), null);
            Assert.True(champion.HP <= champion.MaxHP);
        }
    }

    [Fact]
    public void ResolveEvent_HP_NeverDropsBelowOne()
    {
        for (int i = 0; i < 50; i++)
        {
            var champion = MakeChampion(hp: 1, maxHp: 100);
            Sut.ResolveEvent(champion, MakeEvent(), null);
            Assert.True(champion.HP >= 1);
        }
    }

    [Fact]
    public void ResolveEvent_HPInterventionBonus_IsApplied()
    {
        // With HP: 50, MaxHP: 100 and +30 HP intervention, HP must go up by at least 1
        // (even if the roll causes damage, the intervention partially offsets it)
        var champion = MakeChampion(hp: 50, maxHp: 100);
        var hpBefore = champion.HP;
        var effect = new StatEffect(HP: 30);

        // Run multiple times to average out RNG — at least some runs should heal
        bool healOccurred = false;
        for (int i = 0; i < 100; i++)
        {
            champion = MakeChampion(hp: 50, maxHp: 100);
            Sut.ResolveEvent(champion, MakeEvent(), effect);
            if (champion.HP > hpBefore) { healOccurred = true; break; }
        }
        Assert.True(healOccurred, "HP intervention bonus should increase HP on at least one run");
    }

    // --- Level up ---

    [Fact]
    public void ResolveEvent_LevelsUp_WhenXPThresholdCrossed()
    {
        // At level 1, threshold is 100 XP. Start at 95 XP — any gain crosses it.
        var champion = MakeChampion(xp: 95, level: 1);
        var outcome = Sut.ResolveEvent(champion, MakeEvent(), null);

        Assert.True(outcome.LeveledUp);
        Assert.Equal(2, champion.Level);
        Assert.True(champion.XP < 100, "XP should have been reduced after level-up");
    }

    [Fact]
    public void ResolveEvent_DoesNotLevelUp_WhenXPBelowThreshold()
    {
        var champion = MakeChampion(xp: 0, level: 1);
        var outcome = Sut.ResolveEvent(champion, MakeEvent(), null);

        // Max XP gain is 50, so starting at 0 cannot cross the 100 threshold
        Assert.False(outcome.LeveledUp);
        Assert.Equal(1, champion.Level);
    }

    [Fact]
    public void ResolveEvent_OnLevelUp_StatsIncrease()
    {
        var champion = MakeChampion(xp: 95, level: 1);
        var statsBefore = champion.Stats;

        Sut.ResolveEvent(champion, MakeEvent(), null);

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

        Sut.ResolveEvent(champion, MakeEvent(), null);

        Assert.True(champion.MaxHP > maxHpBefore);
    }

    [Fact]
    public void ResolveEvent_MultiLevelUp_HandledCorrectly()
    {
        // Contrived: level 1, 0 XP, but we manually pump XP so we can test two level-ups
        // At level 1, threshold = 100. At level 2, threshold = 200. Start near 100 XP.
        // We can't force two level-ups in a single tick via RNG alone (max gain is 50),
        // so instead verify that already-high XP completes multiple level-ups correctly.
        var champion = MakeChampion(xp: 290, level: 1);
        // XP: 290 → after tick XP >= 295. Level 1 threshold = 100, so levels to 2 (XP -= 100, = ~195+)
        // Level 2 threshold = 200, so levels to 3 (XP -= 200, = ~0+)
        Sut.ResolveEvent(champion, MakeEvent(), null);

        Assert.True(champion.Level >= 3);
    }

    // --- LastTickAt ---

    [Fact]
    public void ResolveEvent_UpdatesLastTickAt()
    {
        var champion = MakeChampion();
        champion.LastTickAt = DateTimeOffset.UtcNow.AddHours(-6);
        var before = champion.LastTickAt;

        Sut.ResolveEvent(champion, MakeEvent(), null);

        Assert.True(champion.LastTickAt > before);
    }

    // --- Intervention stat effects ---

    [Fact]
    public void ResolveEvent_WithStrIntervention_BoostsSuccessChance()
    {
        // An event requiring STR 100 (impossible without boost) with STR +200 effect
        // should succeed at least sometimes vs never.
        var highReqEvent = MakeEvent(@"{""STR"":100}");
        var strEffect = new StatEffect(STR: 200);

        int successWithBoost = 0;
        int successWithout = 0;

        for (int i = 0; i < 100; i++)
        {
            var c1 = MakeChampion();
            var o1 = Sut.ResolveEvent(c1, highReqEvent, strEffect);
            if (o1.XpGained >= 20) successWithBoost++;

            var c2 = MakeChampion();
            var o2 = Sut.ResolveEvent(c2, highReqEvent, null);
            if (o2.XpGained >= 20) successWithout++;
        }

        Assert.True(successWithBoost > successWithout,
            "A large STR boost should produce more successes than no boost");
    }

    [Theory]
    [InlineData(4, Biome.Safe,      Biome.Safe)]       // level 4 → stays Safe
    [InlineData(5, Biome.Safe,      Biome.Normal)]     // level 5 → advances to Normal
    [InlineData(9, Biome.Normal,    Biome.Normal)]     // level 9 → stays Normal
    [InlineData(10, Biome.Normal,   Biome.Dangerous)]  // level 10 → advances to Dangerous
    [InlineData(15, Biome.Normal,   Biome.Dangerous)]  // already high level
    public void ResolveEvent_BiomeAdvances_AtCorrectLevelThreshold(
        int targetLevel, Biome startingBiome, Biome expectedBiome)
    {
        // Start one tick below the target level so one tick tips over
        var champion = MakeChampion(xp: (targetLevel - 1) * 100 - 1, level: targetLevel - 1);
        champion.Biome = startingBiome;

        // Force a success by using an event with no requirements and enough XP to level up
        // Run many ticks until the champion reaches the target level
        for (int i = 0; i < 20 && champion.Level < targetLevel; i++)
            Sut.ResolveEvent(champion, MakeEvent(), new StatEffect(STR: 100));

        Assert.Equal(expectedBiome, champion.Biome);
    }
}
