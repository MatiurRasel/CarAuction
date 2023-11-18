using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReserveProxy"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {
        options.Authority = builder.Configuration["IdentityServiceUrl"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience =  false;
        options.TokenValidationParameters.NameClaimType = "userName";
    });

var app = builder.Build();

app.MapReverseProxy();

app.UseAuthentication();

app.UseAuthorization();

app.Run();
