using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace FelicityTweets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Configuration.AddJsonFile("appsettings.local.json", optional: true);
            builder.Configuration.AddEnvironmentVariables();

            builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<TweetRepository>();
            builder.Services.AddHostedService(provider => provider.GetService<TweetRepository>());

            builder.Services.AddCors();
            
            

            builder.Services.AddHttpClient("twitter", (sp, httpClient) =>
            {
                var configuration = sp.GetService<IConfiguration>() ?? throw new Exception("no IConfiguration");
                var token = configuration["TWITTER_TOKEN"];

                httpClient.BaseAddress = new Uri("https://api.twitter.com");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            });

            var app = builder.Build();

            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials()
            );

            app.UseRouting();
            app.UseEndpoints(app => app.MapControllers());
            app.Run();
        }
    }
}