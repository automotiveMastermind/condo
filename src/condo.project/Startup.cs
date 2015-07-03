namespace Condo.Project
{
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Http;

    using Microsoft.Framework.DependencyInjection;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World");
            });
        }
    }
}