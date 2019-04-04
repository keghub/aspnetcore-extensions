using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EMG.Extensions.AspNetCore
{
    public interface IJwtBuilder
    {
        IServiceCollection Services { get; }

        IConfiguration Configuration { get; }
    }

    public class JwtBuilder : IJwtBuilder
    {
        public JwtBuilder(IServiceCollection services, IConfiguration configuration)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IServiceCollection Services { get; }

        public IConfiguration Configuration { get; }
    }
}