﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TestWebAPI.Config;
using TestWebAPI.Data;
using TestWebAPI.Helpers;
using TestWebAPI.Middlewares;
using TestWebAPI.Repositories;
using TestWebAPI.Repositories.Interfaces;
using TestWebAPI.Services;
using TestWebAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Set Cors
builder.Services.AddCors(opt => opt.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Connect DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookStore"));
});


// JWT config
var jwtSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<TokenSettings>(jwtSection);

var jwtSettings = jwtSection.Get<TokenSettings>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            RoleClaimType = "roleCode"
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                return Task.CompletedTask;
            }
        };
    });

//config permission 
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("add_role", policy =>
        policy.Requirements.Add(new AuthorizationConfig("add_role")));
    options.AddPolicy("get_role", policy =>
    policy.Requirements.Add(new AuthorizationConfig("get_role")));
});

// AutoMapper
#region Auto mapper
builder.Services.AddSingleton(provider => new MapperConfiguration(options =>
{
    options.AddProfile(new ApplicationMapper());
}).CreateMapper());
#endregion
builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));

// Add Repositories to the container.
builder.Services.AddScoped<IRoleRepositories, RoleRepositories>();
builder.Services.AddScoped<IAuthRepositories, AuthRepositories>();
builder.Services.AddScoped<IJwtRepositories, JwtRepositories>();
builder.Services.AddScoped<IPermisstionRepositories, PermisstionRepositories>();
builder.Services.AddScoped<IRoleHasPermissionRepositories, RoleHasPermissionRepositories>();
builder.Services.AddScoped<IUserRepositories, UserRepositories>();

// Add services to the container.
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPermissionServices, PermissionServices>();
builder.Services.AddScoped<ISendMailService, SendMailServices>();
builder.Services.AddScoped<IRoleHasPermissionServices, RoleHasPermissionServices>();
builder.Services.AddScoped<IUserServices, UserServices>();

// Register JWTHelper
builder.Services.AddScoped<IJWTHelper, JWTHelper>();

//Htttp cookie
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature.Error;

        // Xử lý ngoại lệ ở đây
        // Ví dụ:
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Internal Server Error");
    });
});

app.UseMiddleware<ErrorHandlingToken>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingToken>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
