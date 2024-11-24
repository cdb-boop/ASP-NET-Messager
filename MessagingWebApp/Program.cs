using MessagingWebApp;
using MessagingWebApp.Controllers;
using MessagingWebApp.Data;
using MessagingWebApp.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Twilio.AspNet.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MessagingWebAppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("MessagingWebAppDbConnectionString"))
);

builder.Services.AddScoped<IChatGroupRepository, ChatGroupRepository>();
builder.Services.AddScoped<ICommunicationRepository, TwilioCommunicationRepository>();

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.MapPost("broadcast", async (string message, IHubContext<ChatHub, IChatClient> context) =>
//{
//    //await context.Clients.Clients(); // specific client
//    await context.Clients.All.ReceiveMessage(message); // all clients
//
//    return Results.NoContent();
//});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapHub<ChatHub>("chat-hub");

app.Run();
