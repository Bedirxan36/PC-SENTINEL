using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SENTINEL.Services;

public class AutoScanService
{
    private DispatcherTimer? _timer;
    private Func<Task>? _scanAction;
    private bool _isEnabled;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            if (_isEnabled)
                Start();
            else
                Stop();
        }
    }

    public int IntervalMinutes { get; set; } = 30;

    public void Initialize(Func<Task> scanAction)
    {
        _scanAction = scanAction;
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(IntervalMinutes)
        };
        _timer.Tick += async (s, e) => await ExecuteScanAsync();
    }

    public void Start()
    {
        _timer?.Start();
    }

    public void Stop()
    {
        _timer?.Stop();
    }

    public void SetInterval(int minutes)
    {
        IntervalMinutes = minutes;
        if (_timer != null)
        {
            _timer.Interval = TimeSpan.FromMinutes(minutes);
        }
    }

    private async Task ExecuteScanAsync()
    {
        if (_scanAction != null)
        {
            await _scanAction();
        }
    }
}
