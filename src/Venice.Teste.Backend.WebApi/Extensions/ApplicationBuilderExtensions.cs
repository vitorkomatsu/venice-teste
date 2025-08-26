using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

namespace Venice.Teste.Backend.WebApi.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("swagger/v1/swagger.json", "Venice.Teste.Backend.WebApi");
                options.RoutePrefix = "";
                options.EnableFilter();
                options.DisplayRequestDuration();
            });
        }
    }
}