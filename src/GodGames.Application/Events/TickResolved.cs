using GodGames.Application.Models;
using GodGames.Domain.Entities;
using MediatR;

namespace GodGames.Application.Events;

public record TickResolved(
    Champion Champion,
    WorldEvent Event,
    TickOutcome Outcome,
    int TickNumber) : INotification;
