using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Property.Core.Database;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Model = Property.Core.Model;

namespace IntegrationTest;

public class PropertiesTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
      .WithImage("postgres:15-alpine")
      .WithUsername("postgres")
      .WithPassword("postgres")
      .WithExposedPort(5432)
      .WithPortBinding(5432)
      .WithEnvironment("POSTGRES_EXTENSIONS", "pg_trgm")
      .Build();

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
      .WithUsername("guest")
      .WithPassword("guest")
      .WithExposedPort(5672)
      .WithPortBinding(5672)
      .Build();

    //https://dinochiesa.github.io/jwt/
    // Symmetric key: /wMLPRs1zvFxIsIztaakWJq7TWF+Lg==
    // UserId: ac6ec7e3-3795-4aac-93d3-32089373798e
    // Role: Host
    private const string HostJwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhYzZlYzdlMy0zNzk1LTRh" +
                                "YWMtOTNkMy0zMjA4OTM3Mzc5OGUiLCJyb2xlIjoiSG9zdCIsIm5iZiI6MTcwNjYyMTk1NC" +
                                "wiZXhwIjoxOTUwMDAwMDAwLCJpYXQiOjE3MDY2MjE5NTQsImlzcyI6Imh0dHBzOi8vYXBp" +
                                "LnRvdXJpc3Rlci5jb20iLCJhdWQiOiJodHRwczovL3RvdXJpc3Rlci5jb20ifQ.R5N17RpS" +
                                "25Sfr6eE9r5nzdUBQ9yAUZkpYtEeIuZfAjE";

    public Task InitializeAsync()
    {
        Console.WriteLine(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

        return Task.WhenAll(
          _postgresContainer.StartAsync(),
          _rabbitMqContainer.StartAsync()
        );
    }

    public Task DisposeAsync()
    {
        return Task.WhenAll(
          _postgresContainer.DisposeAsync().AsTask(),
          _rabbitMqContainer.DisposeAsync().AsTask()
        );
    }

    public sealed class Api : IClassFixture<PropertiesTest>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        private readonly IServiceScope _serviceScope;
        private readonly HttpClient _httpClient;

        public Api(PropertiesTest propertiesTest)
        {
            // Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", propertiesTest._sqlEdgeContainer.GetConnectionString());
            _webApplicationFactory = new WebApplicationFactory<Program>();
            _serviceScope = _webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            _httpClient = _webApplicationFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HostJwt);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _serviceScope.Dispose();
            _webApplicationFactory.Dispose();
        }

        public async Task<Model.User> GetOrCreateHost()
        {
            var context = _serviceScope.ServiceProvider.GetRequiredService<PropertyContext>();

            var hostId = new Guid("ac6ec7e3-3795-4aac-93d3-32089373798e"); // Same is UserId in JWT above
            var host = await context.Users.FirstOrDefaultAsync(u => u.Id == hostId);

            if (host is not null) return host;

            host = new Model.User()
            {
                Id = hostId,
                Role = "Host",
                Email = "host@example.com",
                FirstName = "John",
                LastName = "Boss"
            };

            context.Add(host);
            await context.SaveChangesAsync();

            return host;
        }

        [Fact]
        [Trait("Category", nameof(Api))]
        public async Task Get_Properties_ReturnsEmpty()
        {
            // Given
            const string path = "/properties";

            // When
            var response = await _httpClient.GetAsync(path);
            var properties = await response.Content.ReadFromJsonAsync<Model.Property[]>();

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Empty(properties!);
        }

        [Fact]
        [Trait("Category", nameof(Api))]
        public async Task HowToCreateDataBeforeRunningATestForExampleCreatingAUserSoYouCanCreateAPropertyUsingHttpClient()
        {
            var host = await GetOrCreateHost();
            Assert.NotNull(host);
        }
    }
}