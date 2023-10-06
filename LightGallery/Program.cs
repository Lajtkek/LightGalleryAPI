using System.Text;
using System.Text.Json.Serialization;
using LightGallery;
using LightGallery.Models;
using LightGallery.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure reading application configuration from Railway env variables
builder.Configuration.AddEnvironmentVariables(prefix: "Railway_");

// Configure the application web host to listen on PORT provided by Railway reverse proxy
// var port = Environment.GetEnvironmentVariable("PORT") ?? "8081";
// builder.WebHost.UseUrls($"http://*:{port}");

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IGalleryService, GalleryService>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<IFTPService, FTPService>();
builder.Services.AddScoped<SeedService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Insert JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<DefaultDatabaseContext>(o =>
      o.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        options.User.RequireUniqueEmail = false;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 1;
    })
    .AddEntityFrameworkStores<DefaultDatabaseContext>()
    .AddDefaultTokenProviders();

var key = builder.Configuration["JWT:Secret"];
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Internal";
    options.DefaultChallengeScheme = "Internal";
    options.DefaultScheme = "Internal";
}).AddJwtBearer("Internal", x =>
{
    x.Events = new JwtBearerEvents()
    {
        OnMessageReceived = (context) => Task.CompletedTask,
        OnChallenge = (context) => Task.CompletedTask
    };
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seedService = services.GetService<SeedService>();
    //await seedService.Seed();
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();