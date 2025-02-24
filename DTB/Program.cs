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

// ע�� EFCore DbContext ����
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DTBConnection")));

Action<DbContextOptionsBuilder> configureDbContext = options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DTBConnection"));

// ��� DbContextFactory��ʹ�óػ�����
builder.Services.AddPooledDbContextFactory<BatteryDbContext>(configureDbContext);

// �����֤����Ȩ
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

// ע�� Identity ����
builder.Services.AddIdentityCore<AppUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// ��ӻ�������
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

// ��� Blazor ��ط���
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

// ��� Masa Blazor
builder.Services.AddMasaBlazor(builder =>
{
    builder.ConfigureTheme(theme =>
    {
        theme.Themes.Light.Primary = "#4318FF";
        theme.Themes.Light.Accent = "#4318FF";
    });
}).AddI18nForServer(i18nPath, System.Text.Encoding.UTF8);

// ��ӵ�������
builder.Services.AddDynamicNav();
builder.Services.AddHostedService<NavUpdateHostedService>();

using (var scopeb = builder.Services.BuildServiceProvider().CreateScope())
{
    var navService = scopeb.ServiceProvider.GetRequiredService<DynamicNavService>();
    await navService.GenerateNavJsonAsync();
}

builder.Services.AddNav(Path.Combine(basePath, $"wwwroot/nav/nav.json"));

// ���ͨ�÷���
builder.Services.AddScoped<CookieStorage>();
builder.Services.AddScoped<GlobalConfig>();

// ��� SignalR HubConnection
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

// ����ڴ滺�����ShiftService ������
builder.Services.AddMemoryCache();

// ע�� ShiftService��DeviceStateService ������
builder.Services.AddScoped<IShiftService, ShiftService>();

// ע���豸��ط���
var connectionString = builder.Configuration.GetConnectionString("DTBConnection");
var optionsBuilder = new DbContextOptionsBuilder<BatteryDbContext>();
optionsBuilder.UseSqlServer(connectionString);

using (var tempContext = new BatteryDbContext(optionsBuilder.Options))
{
    var devicesInfo = tempContext.Devices.ToList();

    // ע�� HeartbeatBackgroundService
    builder.Services.AddSingleton<HeartbeatBackgroundService>(sp => new HeartbeatBackgroundService(
        sp,
        sp.GetRequiredService<IDbContextFactory<BatteryDbContext>>()
    ));

    builder.Services.AddHostedService(provider =>
        provider.GetRequiredService<HeartbeatBackgroundService>());

    // ע�� DeviceStateService
    builder.Services.AddScoped<IDeviceStateService>(sp => new DeviceStateService(
        sp.GetRequiredService<IHubContext<DeviceHub>>(),
        sp.GetRequiredService<DeviceStatusCacheFactory>(),
        sp.GetRequiredService<IDbContextFactory<BatteryDbContext>>(),
        sp.GetRequiredService<UserManager<AppUser>>(),
        sp.GetRequiredService<HeartbeatBackgroundService>(),
        sp.GetRequiredService<IShiftService>()
    ));

    // ע���豸״̬���湤��
    builder.Services.AddSingleton<DeviceStatusCacheFactory>(serviceProvider =>
        new DeviceStatusCacheFactory(devicesInfo));

    // Ϊÿ���豸ע�Ỻ�����
    foreach (var device in devicesInfo)
    {
        builder.Services.AddSingleton<IDeviceStatusCache>(serviceProvider =>
        {
            var factory = serviceProvider.GetRequiredService<DeviceStatusCacheFactory>();
            return factory.GetCache(device.Id);
        });
    }
}

// ע�� ShiftBackgroundService
builder.Services.AddHostedService<ShiftBackgroundService>();

// ����Ӧ��
var app = builder.Build();

// ��ʼ�����ݿ�
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

// ��ʼ����λ���
using (var scope = app.Services.CreateScope())
{
    var shiftService = scope.ServiceProvider.GetRequiredService<IShiftService>();
    await shiftService.RefreshCache();
}

// �����м���ܵ�
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