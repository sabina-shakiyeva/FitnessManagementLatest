using Fitness.Business.Abstract;
using Fitness.Business.Concrete;
using Fitness.Business.Hubs;
using Fitness.Business.Mappers;
using Fitness.Business.Middlewares;
using Fitness.DataAccess.Abstract;
using Fitness.DataAccess.Concrete.EfEntityFramework;
using Fitness.Entities.Concrete;
using FitnessManagement.Data;
using FitnessManagement.Entities;
using FitnessManagement.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAutoMapper(typeof(UserMapper));
builder.Services.AddAutoMapper(typeof(TrainerMapper));
builder.Services.AddAutoMapper(typeof(Mapper));
builder.Services.AddHttpContextAccessor();

//builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));


//builder.Services.AddSingleton<IStripeService, StripeService>();

builder.Services.AddControllers();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:3001") // Frontend URL
              .AllowAnyMethod()
              .AllowAnyHeader()
         .AllowCredentials();
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<DelayedPaymentBackgroundService>();

builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IAdminDal, EfAdminDal>();
builder.Services.AddScoped<ITrainerDal, EfTrainerDal>();
builder.Services.AddScoped<IUserDal, EfUserDal>();
builder.Services.AddScoped<IEquipmentDal, EfEquipmentDal>();
builder.Services.AddScoped<IAttendanceDal, EfAttendanceDal>();
builder.Services.AddScoped<IPackageDal, EfPackageDal>();
builder.Services.AddScoped<IUserEquipmentUsageDal, EfUserEquipmentUsageDal>();
builder.Services.AddScoped<IPaymentDal,EfPaymentDal>();
builder.Services.AddScoped <IGroupDal, EfGroupDal>();
builder.Services.AddScoped<ITrainerScheduleDal,EfTrainerScheduleDal>();
builder.Services.AddScoped<IGroupUserDal, EfGroupUserDal>();
builder.Services.AddScoped<IMessageDal, EfMessageDal>();
builder.Services.AddScoped<IProductDal, EfProductDal>();
builder.Services.AddScoped<ICartItemDal, EfCartItemDal>();
builder.Services.AddScoped<IPurchaseHistoryDal, EfPurchaseHistoryDal>();
builder.Services.AddScoped<IPurchaseRequestDal, EfPurchaseRequestDal>();
builder.Services.AddScoped<IWorkoutPlanDal,EfWorkoutPlanDal>();
builder.Services.AddScoped<IWorkoutDayDal, EfWorkoutDayDal>();
builder.Services.AddScoped<IWorkoutExerciseDal, EfWorkoutExerciseDal>();
builder.Services.AddScoped <IPackageWorkoutDal, EfPackageWorkoutDal>();
builder.Services.AddScoped<IFeedbackDal, EfFeedbackDal>();  
builder.Services.AddScoped<INotificationDal,EfNotificationDal>();
builder.Services.AddScoped<IGlobalNotificationDal, EfGlobalNotificationDal>();
builder.Services.AddScoped<IBalanceTopUp, EfBalanceTopUp>();
builder.Services.AddScoped <IGroupService,GroupService>();
builder.Services.AddScoped<ITrainerScheduleService,TrainerScheduleService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IPurchaseHistoryService,PurchaseHistoryService>();
builder.Services.AddScoped<IWorkoutPlanService, WorkoutPlanService>();
builder.Services.AddScoped<ITrainerFeedbackService,TrainerFeedbackService>();
builder.Services.AddScoped<IUserFeedbackService,UserFeedbackService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IGlobalNotificationService, GlobalNotificationService>();

builder.Services.AddScoped<IUserEquipmentUsageService, UserEquipmentUsageService>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<GymDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };

        // ===>>> Əlavə et bunları:
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                // SignalR bağlantısı üçün xüsusi yoxlama
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notification"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Issuer"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//    };
//});
var conn =builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<GymDbContext>(options => options.UseSqlServer(conn));


var app = builder.Build();
app.UseRouting();

app.UseCors("AllowLocalhost");




if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseStaticFiles();
app.UseHttpsRedirection();



app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chathub");
app.MapHub<NotificationHub>("/hubs/notification");


app.MapControllers();

app.Run();
