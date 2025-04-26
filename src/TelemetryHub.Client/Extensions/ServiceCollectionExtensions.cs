using Microsoft.Extensions.DependencyInjection;

namespace TelemetryHub.Client.Extensions
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddTelemetryClient(
        this IServiceCollection services,
        string address)
    {
      services.AddSingleton(_ =>
          TelemetryClientFactory.Create(address));
      return services;
    }
  }
}
