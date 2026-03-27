namespace GodGames.Domain.Entities;

public class God
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;

    public Champion? Champion { get; set; }
}
