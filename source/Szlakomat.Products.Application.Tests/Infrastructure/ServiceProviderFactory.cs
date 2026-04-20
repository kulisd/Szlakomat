using Microsoft.Extensions.DependencyInjection;
using Szlakomat.Products.Infrastructure;

namespace Szlakomat.Products.Application.Tests.Infrastructure;

internal static class ServiceProviderFactory
{
    public static IServiceProvider Create()
    {
        var services = new ServiceCollection();
        services.AddProductModule();
        return services.BuildServiceProvider();
    }
}
