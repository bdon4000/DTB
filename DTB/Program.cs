using DTB.Components;
using DTB.Components.Account;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.ResponseCompression;
using DTB.Hubs;
using DTB.Data.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Authentication.Cookies;
using DTB.Data.Base;
using DTB.Data.BatteryData;
using DTB.Data;
using DTB.Global.Nav;
using Masa.Blazor;
using System.Text;
using DTB.Data.Devices;
using Microsoft.Extensions.Options;
using DTB.Services;
using Microsoft.AspNetCore.SignalR;
using DTB.Service;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls(builder.Configuration["BaseUrl"]);

// 注册 EFCore DbContext 服务
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DTBConnection")));

Action<DbContextOptionsBuilder> configureDbContext = options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DTBConnection"));

// 添加 DbContextFactory，使用池化配置
builder.Services.AddPooledDbContextFactory<BatteryDbContext>(configureDbContext);

// 添加认证和授权
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
}).AddIdentityCookies();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Cookie.Name = ".AspNetCore.Identity.Application";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.LoginPath = "/Account/Login";
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;
});

// 注册 Identity 服务
builder.Services.AddIdentityCore<AppUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// 添加基础服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
    ?? throw new Exception("Get the assembly root directory exception!");

var i18nPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "i18n");

// 添加 Blazor 相关服务
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

// 添加 Masa Blazor
builder.Services.AddMasaBlazor(builder =>
{
    builder.ConfigureTheme(theme =>
    {
        theme.Themes.Light.Primary = "#4318FF";
        theme.Themes.Light.Accent = "#4318FF";
    });
}).AddI18nForServer(i18nPath, System.Text.Encoding.UTF8);

// 添加导航服务
builder.Services.AddDynamicNav();
builder.Services.AddHostedService<NavUpdateHostedService>();

using (var scopeb = builder.Services.BuildServiceProvider().CreateScope())
{
    var navService = scopeb.ServiceProvider.GetRequiredService<DynamicNavService>();
    await navService.GenerateNavJsonAsync();
}

builder.Services.AddNav(Path.Combine(basePath, $"wwwroot/nav/nav.json"));

// 添加通用服务
builder.Services.AddScoped<CookieStorage>();
builder.Services.AddScoped<GlobalConfig>();

// 添加 SignalR HubConnection
builder.Services.AddSingleton<HubConnection>(serviceProvider =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    var urls = config["BaseUrl"] ?? "http://localhost:5000";
    var baseUrl = urls.Split(';')[0].TrimEnd('/').Replace("*", "localhost");
    var connection = new HubConnectionBuilder()
        .WithUrl(baseUrl + "/Devicehub")
        .Build();
    return connection;
});

// 添加内存缓存服务（ShiftService 依赖）
builder.Services.AddMemoryCache();

// 注册 ShiftService（DeviceStateService 依赖）
builder.Services.AddScoped<IShiftService, ShiftService>();

// 注册设备相关服务
var connectionString = builder.Configuration.GetConnectionString("DTBConnection");
var optionsBuilder = new DbContextOptionsBuilder<BatteryDbContext>();
optionsBuilder.UseSqlServer(connectionString);

using (var tempContext = new BatteryDbContext(optionsBuilder.Options))
{
    var devicesInfo = tempContext.Devices.ToList();

    // 注册 HeartbeatBackgroundService
    builder.Services.AddSingleton<HeartbeatBackgroundService>(sp => new HeartbeatBackgroundService(
        sp,
        sp.GetRequiredService<IDbContextFactory<BatteryDbContext>>()
    ));

    builder.Services.AddHostedService(provider =>
        provider.GetRequiredService<HeartbeatBackgroundService>());

    // 注册 DeviceStateService
    builder.Services.AddScoped<IDeviceStateService>(sp => new DeviceStateService(
        sp.GetRequiredService<IHubContext<DeviceHub>>(),
        sp.GetRequiredService<DeviceStatusCacheFactory>(),
        sp.GetRequiredService<IDbContextFactory<BatteryDbContext>>(),
        sp.GetRequiredService<UserManager<AppUser>>(),
        sp.GetRequiredService<HeartbeatBackgroundService>(),
        sp.GetRequiredService<IShiftService>()
    ));

    // 注册设备状态缓存工厂
    builder.Services.AddSingleton<DeviceStatusCacheFactory>(serviceProvider =>
        new DeviceStatusCacheFactory(devicesInfo));

    // 为每个设备注册缓存服务
    foreach (var device in devicesInfo)
    {
        builder.Services.AddSingleton<IDeviceStatusCache>(serviceProvider =>
        {
            var factory = serviceProvider.GetRequiredService<DeviceStatusCacheFactory>();
            return factory.GetCache(device.Id);
        });
    }
}

// 注册 ShiftBackgroundService
builder.Services.AddHostedService<ShiftBackgroundService>();

// 构建应用
var app = builder.Build();

// 初始化数据库
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

// 初始化班次缓存
using (var scope = app.Services.CreateScope())
{
    var shiftService = scope.ServiceProvider.GetRequiredService<IShiftService>();
    await shiftService.RefreshCache();
}

// 配置中间件管道
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseResponseCompression();
app.UseMiddleware<BlazorCookieLoginMiddleware>();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapAdditionalIdentityEndpoints();
app.MapHub<DeviceHub>("/Devicehub");

app.Run();