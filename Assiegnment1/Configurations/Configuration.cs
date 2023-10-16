using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.Practices.ServiceLocation;
using ServerSide.Repositories;

namespace ServerSide.Configurations;


public static class AppConfiguration
{
    public static IConfiguration Configuration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        return builder.Build();
    }

    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton(DepartmentRepository.Instance);
        services.AddSingleton(UserRepository.Instance);
        services.AddSingleton(ReportingLineRepository.Instance);
        services.AddSingleton(TaskToDoRepository.Instance);

        return services.BuildServiceProvider();
    }

}

