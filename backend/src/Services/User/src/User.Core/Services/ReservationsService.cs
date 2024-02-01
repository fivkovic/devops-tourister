using FluentResults;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace User.Core.Services;

public class ReservationsServiceNotAvailable() : Error("Reservations service is currently not available.");

public class ReservationsService(HttpClient httpClient, ILogger<ReservationsService> logger)
{
    public async Task<Result<bool>> HasActiveReservations(string accessToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/reservations/active");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        logger.LogError("Failed to check for active reservations: {StatusCode}", response.StatusCode);
        return new ReservationsServiceNotAvailable();
    }
}