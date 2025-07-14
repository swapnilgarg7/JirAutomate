
using JirAutomate.Services;
using Microsoft.AspNetCore.Http.Features;

namespace JirAutomate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50 MB
            });
            builder.Services.AddControllers();          
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<JiraService>();
            builder.Services.AddScoped<GeminiService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors();
            app.UseAuthorization();
            app.MapControllers();                      

            app.Run();

        }
    }
}
