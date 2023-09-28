using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Bookings.Events;

public record BookingReservedDomainEvent(Guid BookingId) : IDomainEvent;
public record BookingCancelledDomainEvent(Guid BookingId) : IDomainEvent;
public record BookingConfirmedDomainEvent(Guid BookingId) : IDomainEvent;
public record BookingRejectedDomainEvent(Guid BookingId) : IDomainEvent;
public record BookingCompletedDomainEvent(Guid BookingId) : IDomainEvent;
