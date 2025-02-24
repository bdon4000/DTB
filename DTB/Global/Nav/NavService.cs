using DTB.Data;
using Microsoft.EntityFrameworkCore;

namespace DTB.Global.Nav
{
    public class NavMenuItem
    {
        // NavMenuItem 类保持不变
        public int Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Href { get; set; }
        public string Target { get; set; }
        public bool Hide { get; set; }
        public List<NavMenuItem> Children { get; set; }
    }

    public class DynamicNavService
    {
        private readonly IDbContextFactory<BatteryDbContext> _contextFactory;
        private readonly string _navFilePath;
        private static int _currentId = 100;

        public DynamicNavService(
            IDbContextFactory<BatteryDbContext> contextFactory,
            IConfiguration configuration)
        {
            _contextFactory = contextFactory;
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _navFilePath = Path.Combine(basePath, "wwwroot", "nav", "nav.json");
        }

        private int GetNextId()
        {
            return Interlocked.Increment(ref _currentId);
        }

        public async Task GenerateNavJsonAsync()
        {
            try
            {
                // 读取现有的导航配置
                var existingNav = await ReadExistingNavAsync();
                existingNav.RemoveAll(n => n.Title == "DeviceSetting");
                existingNav.RemoveAll(n => n.Title == "Devices");
                existingNav.RemoveAll(n => n.Title == "DeviceEdit");
                existingNav.RemoveAll(n => n.Title == "DeviceStatus");


                // 使用 await using 来确保正确释放 context
                await using var context = await _contextFactory.CreateDbContextAsync();

                // 获取所有设备
                var devices = await context.Devices
                    .OrderBy(d => d.Id)
                    .Select(d => new { d.DeviceCode, d.DeviceName })
                    .ToListAsync();

                // 创建设备菜单项
                var deviceMenu = new NavMenuItem
                {
                    Id = GetNextId(),
                    Title = "DeviceSetting",
                    Icon = "mdi-cog-outline",
                    Children = devices.Select(device => new NavMenuItem
                    {
                        Id = GetNextId(),
                        Title = (string.IsNullOrEmpty(device.DeviceName) ? device.DeviceCode : device.DeviceName) + "Setting",
                        Icon = "mdi-circle",
                        Href = $"Device/Edit/{device.DeviceCode}",
                        Hide = false
                    }).ToList()
                };

                var devicestatusMenu = new NavMenuItem
                {
                    Id = GetNextId(),
                    Title = "DeviceStatus",
                    Icon = "mdi-factory",
                    Children = devices.Select(device => new NavMenuItem
                    {
                        Id = GetNextId(),
                        Title = (string.IsNullOrEmpty(device.DeviceName) ? device.DeviceCode : device.DeviceName) + "Status",
                        Icon = "mdi-circle",
                        Href = $"/dashboard/equipstate/{device.DeviceCode}",
                        Hide = false
                    }).ToList()
                };
                existingNav.Insert(0, devicestatusMenu);
                existingNav.Add(deviceMenu);

                

                // 保存更新后的导航配置
                await SaveNavConfigurationAsync(existingNav);
            }
            catch (Exception ex)
            {
                throw new Exception($"生成导航配置时出错: {ex.Message}", ex);
            }
        }

        private async Task<List<NavMenuItem>> ReadExistingNavAsync()
        {
            try
            {
                if (File.Exists(_navFilePath))
                {
                    var jsonContent = await File.ReadAllTextAsync(_navFilePath);
                    return JsonSerializer.Deserialize<List<NavMenuItem>>(jsonContent) ?? new List<NavMenuItem>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取现有导航配置失败: {ex.Message}");
            }
            return new List<NavMenuItem>();
        }

        private async Task SaveNavConfigurationAsync(List<NavMenuItem> navConfig)
        {
            var navDir = Path.GetDirectoryName(_navFilePath);
            if (!Directory.Exists(navDir))
            {
                Directory.CreateDirectory(navDir);
            }

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = null
            };

            await File.WriteAllTextAsync(
                _navFilePath,
                JsonSerializer.Serialize(navConfig, jsonOptions)
            );
        }
    }

    public class NavUpdateHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public NavUpdateHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            // 使用 CreateScope 来确保服务的正确生命周期管理
            using var scope = _serviceProvider.CreateScope();
            var navService = scope.ServiceProvider.GetRequiredService<DynamicNavService>();
            await navService.GenerateNavJsonAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

    public static class NavServiceExtensions
    {
        public static IServiceCollection AddDynamicNav(this IServiceCollection services)
        {
            // 修改为 Scoped 生命周期，因为它会被 hosted service 使用
            services.AddScoped<DynamicNavService>();
            return services;
        }
    }
}