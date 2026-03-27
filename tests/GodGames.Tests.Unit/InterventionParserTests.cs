using GodGames.Application.Services;

namespace GodGames.Tests.Unit;

public class InterventionParserTests
{
    private static readonly InterventionParser Sut = new();

    [Theory]
    [InlineData("grant fire resistance")]
    [InlineData("heat protection please")]
    public void Parse_FireResistance_ReturnsVitBoost(string command)
    {
        var effect = Sut.Parse(command);
        Assert.Equal(10, effect.VIT);
        Assert.Equal(3, effect.DurationTicks);
    }

    [Theory]
    [InlineData("blessed blade")]
    [InlineData("give me a holy sword")]
    public void Parse_BlessedBlade_ReturnsStrBoost(string command)
    {
        var effect = Sut.Parse(command);
        Assert.Equal(15, effect.STR);
        Assert.Equal(3, effect.DurationTicks);
    }

    [Theory]
    [InlineData("divine shield")]
    [InlineData("protect my champion")]
    public void Parse_DivineShield_ReturnsHPBoost(string command)
    {
        var effect = Sut.Parse(command);
        Assert.Equal(20, effect.HP);
        Assert.Equal(0, effect.DurationTicks);
    }

    [Theory]
    [InlineData("swift feet")]
    [InlineData("grant speed")]
    [InlineData("haste now")]
    public void Parse_SwiftFeet_ReturnsDexBoost(string command)
    {
        var effect = Sut.Parse(command);
        Assert.Equal(15, effect.DEX);
        Assert.Equal(3, effect.DurationTicks);
    }

    [Theory]
    [InlineData("arcane mind")]
    [InlineData("grant wisdom to my champion")]
    public void Parse_ArcaneMind_ReturnsWisBoost(string command)
    {
        var effect = Sut.Parse(command);
        Assert.Equal(15, effect.WIS);
        Assert.Equal(3, effect.DurationTicks);
    }

    [Theory]
    [InlineData("inner eye")]
    [InlineData("grant insight")]
    public void Parse_InnerEye_ReturnsIntBoost(string command)
    {
        var effect = Sut.Parse(command);
        Assert.Equal(15, effect.INT);
        Assert.Equal(3, effect.DurationTicks);
    }

    [Theory]
    [InlineData("battle rage")]
    [InlineData("go berserk")]
    public void Parse_BattleRage_ReturnsStrBoostWithWisPenalty(string command)
    {
        var effect = Sut.Parse(command);
        Assert.Equal(20, effect.STR);
        Assert.Equal(-5, effect.WIS);
        Assert.Equal(3, effect.DurationTicks);
    }

    [Theory]
    [InlineData("stealth mode")]
    [InlineData("use shadow")]
    public void Parse_Stealth_ReturnsDexBoost(string command)
    {
        var effect = Sut.Parse(command);
        Assert.Equal(10, effect.DEX);
        Assert.Equal(3, effect.DurationTicks);
    }

    [Theory]
    [InlineData("fortify the champion")]
    [InlineData("endure the pain")]
    public void Parse_Fortify_ReturnsVitAndHPBoost(string command)
    {
        var effect = Sut.Parse(command);
        Assert.Equal(10, effect.VIT);
        Assert.Equal(5, effect.HP);
        Assert.Equal(0, effect.DurationTicks);
    }

    [Fact]
    public void Parse_Smite_ReturnsStrAndIntBoost()
    {
        var effect = Sut.Parse("smite the enemy");
        Assert.Equal(10, effect.STR);
        Assert.Equal(5, effect.INT);
        Assert.Equal(3, effect.DurationTicks);
    }

    [Theory]
    [InlineData("rejuvenate")]
    [InlineData("heal my warrior")]
    [InlineData("mend his wounds")]
    public void Parse_Heal_ReturnsLargeHPBoost(string command)
    {
        var effect = Sut.Parse(command);
        Assert.Equal(30, effect.HP);
    }

    [Theory]
    [InlineData("do something random")]
    [InlineData("")]
    [InlineData("zzz")]
    public void Parse_UnknownCommand_ReturnsDefaultBlessing(string command)
    {
        var effect = Sut.Parse(command);
        Assert.Equal(5, effect.VIT);
        Assert.Equal(0, effect.STR);
        Assert.Equal(0, effect.HP);
    }

    [Fact]
    public void Parse_IsCaseInsensitive()
    {
        var lower = Sut.Parse("blessed blade");
        var upper = Sut.Parse("BLESSED BLADE");
        var mixed = Sut.Parse("Blessed Blade");

        Assert.Equal(lower, upper);
        Assert.Equal(lower, mixed);
    }
}
