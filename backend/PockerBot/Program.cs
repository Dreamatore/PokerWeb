using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PockerBot.Presentation;
using Telegram.Bot;

namespace PockerBot;

public class Program
{
    static void Main(string[] args)
    {
        var bld = WebApplication.CreateBuilder();
        bld.Services.AddControllers();
        bld.Services.AddSwaggerGen();
        
        bld.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
        bld.Services.AddSingleton<Config>();
        bld.Services.AddSingleton<TelegramBotClient>(x =>
        {
            var config = x.GetService<Config>();
            return new TelegramBotClient(config !.TelegramBotToken);
        });
        
        bld.Services.AddSingleton<SessionManager>();
        bld.Services.AddSingleton<TelegramBot>();
        var app = bld.Build();
        app.MapControllers();
        
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors("AllowAllOrigins");

        var bot = app.Services.GetRequiredService<TelegramBot>();
        bot.StartBot();
        
        app.Run();
    }
}