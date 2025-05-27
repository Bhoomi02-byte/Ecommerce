using Ecommerce.Data;
using Ecommerce.Middleware;
using Ecommerce.Services;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme  =   JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>  
 {
    options.TokenValidationParameters = new TokenValidationParameters
 {
     ValidateIssuer = true,
     ValidateAudience = true,
     ValidateLifetime = true,
     ValidateIssuerSigningKey = true,
     ValidIssuer = jwtSettings["Issuer"],
     ValidAudience = jwtSettings["Audience"],
     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
  };
  });

var redisConnectionString = builder.Configuration["Redis:ConnectionString"];
//Console.WriteLine($"Redis connection string RANDOM : '{redisConnectionString}'");

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = ConfigurationOptions.Parse(redisConnectionString, true);
    config.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
 options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<EmailService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<IVariantService, VariantService>();

builder.Services.AddScoped<GenerateJwtToken>();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new
            {
                Field = x.Key,
                Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
            }).ToList();

        var response = new ApiResponse( 400, false, JsonHelper.GetMessage(146), errors);

        return new BadRequestObjectResult(response);
    };
});


Log.Logger = new LoggerConfiguration()
   .WriteTo.Console()
   .WriteTo.File("Logs/api-log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
   .CreateLogger();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();  

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (!response.HasStarted)
    {
        response.ContentType = "application/json";

        var apiResponse = response.StatusCode switch
        {
            404 => new ApiResponse(404, false, JsonHelper.GetMessage(147), null),
            403 => new ApiResponse(403, false, JsonHelper.GetMessage(148), null),
            405 => new ApiResponse(405, false, JsonHelper.GetMessage(149), null),
            415 => new ApiResponse(415, false, JsonHelper.GetMessage(150), null),
            _ => new ApiResponse(response.StatusCode, false, JsonHelper.GetMessage(151), null)
        };

        var json = JsonSerializer.Serialize(apiResponse);
        await response.WriteAsync(json);
    }
});

app.MapControllers();
app.Run();
