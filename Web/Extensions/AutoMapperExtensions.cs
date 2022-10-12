using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Web.Types;

namespace Web.Extensions;

public static class AutoMapperExtensions
{
    public static IServiceCollection AddAutoMapperConfig(this IServiceCollection services)
    {
        var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new AutoMapping()));
        IMapper mapper = mapperConfig.CreateMapper();

        services.AddSingleton(mapper);
        return services;
    }
}
