using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Dapper;

namespace Bookify.Application.Bookings.GetBooking;

internal sealed class GetBookingQueryHandler : IQueryHandler<GetBookingQuery, BookingResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetBookingQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<BookingResponse>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = @"
            SELECT
                [Id],
                [ApartmentId],
                [UserId],
                [Status],
                [PriceForPeriodAmount] as PriceAmount,
                [PriceForPeriodCurrency] as PriceCurrency,
                [CleaningFeeAmount],
                [CleaningFeeCurrency],
                [AmenitiesUpChargeAmount],
                [AmenitiesUpChargeCurrency],
                [TotalPriceAmount],
                [TotalPriceCurrency],
                [DurationStart],
                [DurationEnd],
                [CreatedOnUtc]
            FROM Bookings
            WHERE Id = @BookingId";

        
        var booking = await connection.QueryFirstOrDefaultAsync<BookingResponse>(
            sql: sql,
            param: new { request.BookingId },
            commandTimeout: 15);

        return booking;
    }
}