using BackendRaith.Services;
using System.Timers;
using Timer = System.Threading.Timer;
using Microsoft.Extensions.DependencyInjection;

namespace BackendRaith.BackgroundTask
{
    public class TimedDatabaseUpdate : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer? _timer = null;

        public TimedDatabaseUpdate(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(6));
            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var shiftService = scope.ServiceProvider.GetRequiredService<ShiftService>();
                shiftService.LoadShiftsIntoDB();
                Console.WriteLine("Shifts loaded into DB");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
