using System.Text;
using Message_Backend.Application.Helpers;
using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Repository;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Application.Services;
using Message_Backend.Domain.Entities;
using Message_Backend.Domain.Exceptions;
using Message_Backend.Infrastructure.Data;
using Message_Backend.Infrastructure.Repository;
using Message_Backend.Presentation.AuthHandlers;
using Message_Backend.Presentation.AuthRequirements;
using Message_Backend.Presentation.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped(typeof(IRepository<,>),typeof(Repository<,>));
builder.Services.AddScoped(typeof(IBaseService<,>),typeof(BaseService<,>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFriendsService, FriendsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMessageService, MessageService>();
//Add authorization Services
builder.Services.AddScoped<IAuthorizationHandler,SameUserHandler>();
builder.Services.AddScoped<IAuthorizationHandler,RequireAdminOrOwnerRoleHandler>();
builder.Services.AddScoped<IAuthorizationHandler,GroupMemberRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, UserCreatesGroupForThemselvesHandler>();
builder.Services.AddScoped<IAuthorizationHandler, UserHasRequiredRoleInGroupHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CanCreateChatWithProvidedRoleHandler>();
builder.Services.AddScoped<IAuthorizationHandler,UserIsSenderHandler>();
builder.Services.AddScoped<IAuthorizationHandler, UserHasRequiredChatRoleHandler>();
builder.Services.AddScoped<IAuthorizationHandler, UserCanDeleteMessageHandler>();
builder.Services.AddScoped<IAuthorizationHandler,CanReadMessageHandler>();

builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
    o.MaximumReceiveMessageSize = 5000000;
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .WithMethods("GET", "POST","DELETE","PUT")
                .AllowCredentials();
        });
});
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler 
            = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter your JWT token in this field",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
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
            new string[] {}
        }
    };

    c.AddSecurityRequirement(securityRequirement);
    c.AddSignalRSwaggerGen();
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContextPool<MessageContext>(opt =>
    opt.
        UseNpgsql(builder
            .Configuration
            .GetConnectionString("MessageDb"), sqlBuilder =>
        {
            sqlBuilder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            sqlBuilder.MigrationsAssembly("Message-Backend");
        }));

builder.Services.AddIdentityCore<User>(options =>
    {
        var allowedCharacters="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ąĄćĆęĘłŁńŃóÓśŚźŹżŻ";
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.User.AllowedUserNameCharacters = allowedCharacters;
    })
    .AddEntityFrameworkStores<MessageContext>()
    .AddSignInManager();               


builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new RsaSecurityKey(RsaHelper.PublicKey),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("SameUser", policy => policy.Requirements
        .Add(new SameUserRequirement()))
    .AddPolicy("RequireAdminRoleInGroup", policy => policy.Requirements
        .Add(new AdminRoleRequirement()))
    .AddPolicy("GroupMember", policy =>
        policy.Requirements.Add(new GroupMemberRequirement()))
    .AddPolicy("UserCreatesGroupForThemselves", policy =>
        policy.Requirements.Add(new UserCreatesGroupForThemselves()))
    .AddPolicy("UserHasRequiredRoleInGroup", policy =>
        policy.Requirements.Add(new UserHasRequiredRoleInGroup()))
    .AddPolicy("CanCreateChatWithProvidedRole", policy =>
        policy.Requirements.Add(new CanCreateChatWithProvidedRole()))
    .AddPolicy("UserIsSender", policy =>
        policy.Requirements.Add(new UserIsSender()))
    .AddPolicy("UserHasRequiredChatRole", policy =>
        policy.Requirements.Add(new UserHasRequiredChatRole()))
    .AddPolicy("UserCanDelete", policy =>
        policy.Requirements.Add(new UserCanDeleteMessage()))
    .AddPolicy("CanReadMessage", policy =>
        policy.Requirements.Add(new CanReadMessage()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var messageContext = scope.ServiceProvider.GetRequiredService<MessageContext>();
    messageContext.Database.Migrate();
}
app.MapHub<ChatHub>("/ChatHub");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Define Error handling middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        context.Response.StatusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            UserManagerException => StatusCodes.Status403Forbidden,
            EntityAlreadyExistsException or UserAlreadyInGroupException => StatusCodes.Status409Conflict,
            UserNotInGroupException => StatusCodes.Status406NotAcceptable,
            InviteNotValidException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError,
        };
        await context.Response.WriteAsJsonAsync(new{error=exception?.Message});

    });
});
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
