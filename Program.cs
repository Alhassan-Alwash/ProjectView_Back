using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using ProjectView.Data;
using ProjectView.Interfaces;
using ProjectView.Models;
using ProjectView.Repository;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers().AddJsonOptions(options =>
{

    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;

});

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{

    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();


});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });

    // Add JWT Bearer authentication to Swagger
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
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
            new List<string>()
        }
    });

});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

/*builder.Services.AddTransient<Seed>();*/
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IRoleRepo, RoleRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IProjectMemberRepo, ProjectMemberRepo>();
builder.Services.AddScoped<IProjectRepo, ProjectRepo>();
builder.Services.AddScoped<ISubProjectRepo, SubProjectRepo>();
builder.Services.AddScoped<IMemberRepo, MemberRepo>();

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

/*if (args.Length == 1 && args[0].ToLower() == "seeddata")
    SeedData(app);

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<Seed>();
        service.SeedDataContext();
    }
}*/

/*if (app.Environment.IsDevelopment())
{*/
app.UseSwagger();
app.UseSwaggerUI();


var rootFolder = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");
if (!Directory.Exists(rootFolder))
{
    Directory.CreateDirectory(rootFolder);
}

app.MapGet("/ProjImg/{ProjectId}/{imageName}", (string ProjectId, string imageName) =>
{

    var contentRoot = app.Environment.ContentRootPath;
    var imagePath = Path.Combine(contentRoot, "wwwroot", "Files", "ProjectImages", ProjectId, imageName);
    if (!System.IO.File.Exists(imagePath))
    {
        return Results.NotFound("Image not found.");
    }

    var imageExtension = Path.GetExtension(imageName).TrimStart('.');
    var contentType = $"image/{imageExtension}";

    return Results.File(imagePath, contentType);
}).WithDisplayName("ShowImage");

/*app.UseHttpsRedirection();*/

app.UseCors(options =>
{
    options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
});



app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
EnsureAdminUserCreated(app.Services).Wait();

if (app.Environment.IsDevelopment())
{
    app.Run();
}
else
{
    app.Run("http://localhost:5000");
}
//app.Run();

static async Task EnsureAdminUserCreated(IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Check if users already exist in the database
        if (!context.Users.Any(u => u.UserName == "admin"))
        {
            // If no users exist, create an admin user
            var adminUser = new User
            {
                UserName = "admin",
                FullName = "Admin",
                Role = "Admin", // Assuming "Admin" is the role for admin users
                Password = BCrypt.Net.BCrypt.HashPassword("admin") // Set the admin password
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }
    }
}
