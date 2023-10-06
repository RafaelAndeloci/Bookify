using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Dapper;

namespace Bookify.Application.Apartments.SearchApartments;

public class SearchApartmentQueryHandler 
    : IQueryHandler<SearchApartmentsQuery, IReadOnlyList<ApartmentResponse>>
{
    private static readonly int[] ActiveBookingStatuses =
    {
        (int)BookingStatus.Reserved,
        (int)BookingStatus.Confirmed,
        (int)BookingStatus.Completed
    };
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public SearchApartmentQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IReadOnlyList<ApartmentResponse>>> Handle(SearchApartmentsQuery request, CancellationToken cancellationToken)
    {
        if (request.StartDate > request.EndDate) return new List<ApartmentResponse>();

        using var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = @"
            SELECT
                a.Id,
                a.Name,
                a.Description,
                a.PriceAmount as Price,
                a.PriceCurrency as Currency,
                a.AddressCountry as Country,
                a.AddressState as State,
                a.AddressZipCode as ZipCode,
                a.AddressCity as City,
                a.AddressStreet as Street
            FROM Apartments a
            WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM Bookings b
                    WHERE
                        b.ApartmentId = a.Id AND
                        b.DurationStart <= @EndDate AND
                        b.DurationEnd >= @StartDate AND
                        b.Status = ANY(@ActiveBookingStatuses)
                )";

        var apartments = await connection
            .QueryAsync<ApartmentResponse, AddressResponse, ApartmentResponse>(
                sql: sql,
                param: new { request.StartDate, request.EndDate, ActiveBookingStatuses },
                splitOn: "Country",
                map: (apartment, address) =>
                {
                    apartment.Address = address;
                    return apartment;
                });

        return apartments.ToList();
    }
}