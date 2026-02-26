using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using SENTINEL.Models;
using SENTINEL.Services;

namespace SENTINEL.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly PasswordAnalyzerService _passwordAnalyzer = new();
    private readonly PortScannerService _portScanner = new();
    private readonly SystemSecurityService _systemSecurity = new();
    private readonly SecurityScoreService _scoreService = new();
    private readonly SystemMonitorService _systemMonitor = new();
    private readonly NetworkMonitorService _networkMonitor = new();
    private readonly RecommendationService _recommendationService = new();
    private readonly SystemInfoService _systemInfoService = new();
    private readonly AutoScanService _autoScanService = new();
    private readonly QuickFixService _quickFixService = new();
    private readonly ReportExportService _reportExportService = new();
    private readonly NotificationService _notificationService = new();
    private readonly DispatcherTimer _monitorTimer;

    private string _passwordInput = string.Empty;
    private int _securityScore;
    private string _riskLevel = "Bilinmiyor";
    private bool _isScanning;
    private string _scanStatus = "Tarama başlatılmadı";
    private float _cpuUsage;
    private float _ramUsage;
    private float _diskUsage;
    private int _activeConnections;
    private string _lastScanTime = "Henüz tarama yapılmadı";
    private SystemInfo? _systemInfo;
    private bool _isLoadingSystemInfo;
    private bool _autoScanEnabled;
    private int _autoScanInterval = 30;
    private SecurityReport? _lastReport;

    public string PasswordInput
    {
        get => _passwordInput;
        set { _passwordInput = value; OnPropertyChanged(); }
    }

    public int SecurityScore
    {
        get => _securityScore;
        set { _securityScore = value; OnPropertyChanged(); }
    }

    public string RiskLevel
    {
        get => _riskLevel;
        set { _riskLevel = value; OnPropertyChanged(); }
    }

    public bool IsScanning
    {
        get => _isScanning;
        set { _isScanning = value; OnPropertyChanged(); }
    }

    public string ScanStatus
    {
        get => _scanStatus;
        set { _scanStatus = value; OnPropertyChanged(); }
    }

    public float CpuUsage
    {
        get => _cpuUsage;
        set { _cpuUsage = value; OnPropertyChanged(); }
    }

    public float RamUsage
    {
        get => _ramUsage;
        set { _ramUsage = value; OnPropertyChanged(); }
    }

    public float DiskUsage
    {
        get => _diskUsage;
        set { _diskUsage = value; OnPropertyChanged(); }
    }

    public int ActiveConnections
    {
        get => _activeConnections;
        set { _activeConnections = value; OnPropertyChanged(); }
    }

    public string LastScanTime
    {
        get => _lastScanTime;
        set { _lastScanTime = value; OnPropertyChanged(); }
    }

    public SystemInfo? SystemInfo
    {
        get => _systemInfo;
        set { _systemInfo = value; OnPropertyChanged(); }
    }

    public bool IsLoadingSystemInfo
    {
        get => _isLoadingSystemInfo;
        set { _isLoadingSystemInfo = value; OnPropertyChanged(); }
    }

    public bool AutoScanEnabled
    {
        get => _autoScanEnabled;
        set 
        { 
            _autoScanEnabled = value; 
            _autoScanService.IsEnabled = value;
            OnPropertyChanged(); 
        }
    }

    public int AutoScanInterval
    {
        get => _autoScanInterval;
        set 
        { 
            _autoScanInterval = value;
            _autoScanService.SetInterval(value);
            OnPropertyChanged(); 
        }
    }

    public ObservableCollection<SecurityHistory> SecurityHistories { get; } = new();

    public ObservableCollection<PortScanResult> OpenPorts { get; } = new();
    public ObservableCollection<string> SecurityIssues { get; } = new();
    public ObservableCollection<SecurityRecommendation> Recommendations { get; } = new();
    public ObservableCollection<NetworkConnection> NetworkConnections { get; } = new();
    public ObservableCollection<PasswordHistory> PasswordHistories { get; } = new();

    public ICommand StartScanCommand { get; }
    public ICommand AnalyzePasswordCommand { get; }
    public ICommand ViewRecommendationsCommand { get; }
    public ICommand ViewNetworkCommand { get; }
    public ICommand LoadSystemInfoCommand { get; }
    public ICommand EnableDefenderCommand { get; }
    public ICommand EnableFirewallCommand { get; }
    public ICommand CheckUpdatesCommand { get; }
    public ICommand ExportReportCommand { get; }

    public MainViewModel()
    {
        StartScanCommand = new RelayCommand(async () => await StartFullScanAsync(), () => !IsScanning);
        AnalyzePasswordCommand = new RelayCommand(async () => await AnalyzePasswordAsync(), () => !string.IsNullOrEmpty(PasswordInput));
        ViewRecommendationsCommand = new RelayCommand(async () => await LoadRecommendationsAsync());
        ViewNetworkCommand = new RelayCommand(async () => await LoadNetworkConnectionsAsync());
        LoadSystemInfoCommand = new RelayCommand(async () => await LoadSystemInfoAsync());
        EnableDefenderCommand = new RelayCommand(async () => await EnableDefenderAsync());
        EnableFirewallCommand = new RelayCommand(async () => await EnableFirewallAsync());
        CheckUpdatesCommand = new RelayCommand(async () => await CheckUpdatesAsync());
        ExportReportCommand = new RelayCommand(async () => await ExportReportAsync());

        // Otomatik tarama servisini başlat
        _autoScanService.Initialize(async () => await StartFullScanAsync());

        // Gerçek zamanlı sistem izleme
        _monitorTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2) // 2 saniyede bir güncelle
        };
        _monitorTimer.Tick += async (s, e) => await UpdateSystemMetricsAsync();
        _monitorTimer.Start();

        // İlk yükleme - 500ms bekle
        Task.Run(async () => 
        {
            await Task.Delay(500);
            await UpdateSystemMetricsAsync();
        });
    }

    // Public metodlar UI'dan çağrılabilir
    public async Task AnalyzePasswordManualAsync()
    {
        await AnalyzePasswordAsync();
    }

    public async Task LoadNetworkConnectionsManualAsync()
    {
        await LoadNetworkConnectionsAsync();
    }

    private async Task LoadSystemInfoAsync()
    {
        if (IsLoadingSystemInfo) return;
        
        IsLoadingSystemInfo = true;
        SystemInfo = await _systemInfoService.GetSystemInfoAsync();
        IsLoadingSystemInfo = false;
    }

    private async Task EnableDefenderAsync()
    {
        var result = await _quickFixService.EnableWindowsDefenderAsync();
        if (result)
        {
            _notificationService.ShowSuccess("Başarılı", "Windows Defender etkinleştirildi!");
            await Task.Delay(1000);
            await StartFullScanAsync();
        }
        else
        {
            _notificationService.ShowError("Hata", "Windows Defender etkinleştirilemedi. Yönetici hakları gerekebilir.");
        }
    }

    private async Task EnableFirewallAsync()
    {
        var result = await _quickFixService.EnableFirewallAsync();
        if (result)
        {
            _notificationService.ShowSuccess("Başarılı", "Firewall etkinleştirildi!");
            await Task.Delay(1000);
            await StartFullScanAsync();
        }
        else
        {
            _notificationService.ShowError("Hata", "Firewall etkinleştirilemedi. Yönetici hakları gerekebilir.");
        }
    }

    private async Task CheckUpdatesAsync()
    {
        var result = await _quickFixService.CheckWindowsUpdatesAsync();
        if (result)
        {
            _notificationService.ShowInfo("Bilgi", "Windows Update ayarları açıldı.");
        }
    }

    private async Task ExportReportAsync()
    {
        if (_lastReport == null)
        {
            _notificationService.ShowWarning("Uyarı", "Önce bir tarama yapmalısınız!");
            return;
        }

        try
        {
            var json = await _reportExportService.ExportToJsonAsync(_lastReport, SystemInfo);
            await _reportExportService.SaveReportAsync(json, 
                $"SENTINEL_Report_{DateTime.Now:yyyyMMdd_HHmmss}.json", 
                "JSON Files (*.json)|*.json");
            
            _notificationService.ShowSuccess("Başarılı", "Rapor başarıyla dışa aktarıldı!");
        }
        catch (Exception ex)
        {
            _notificationService.ShowError("Hata", $"Rapor dışa aktarılamadı: {ex.Message}");
        }
    }

    private async Task UpdateSystemMetricsAsync()
    {
        try
        {
            var metrics = await _systemMonitor.GetSystemMetricsAsync();
            CpuUsage = metrics.CpuUsage;
            RamUsage = metrics.RamUsage;
            DiskUsage = metrics.DiskUsage;
            ActiveConnections = metrics.ActiveConnections;
        }
        catch { }
    }

    private async Task StartFullScanAsync()
    {
        IsScanning = true;
        SecurityIssues.Clear();
        OpenPorts.Clear();

        ScanStatus = "Sistem güvenliği kontrol ediliyor...";
        var defenderEnabled = await _systemSecurity.CheckWindowsDefenderAsync();
        var firewallEnabled = await _systemSecurity.CheckFirewallAsync();
        var updatesEnabled = await _systemSecurity.CheckWindowsUpdateAsync();

        if (!defenderEnabled) SecurityIssues.Add("⚠ Windows Defender devre dışı");
        if (!firewallEnabled) SecurityIssues.Add("⚠ Firewall devre dışı");
        if (!updatesEnabled) SecurityIssues.Add("⚠ Windows Update çalışmıyor");

        ScanStatus = "Portlar taranıyor...";
        var portResults = await _portScanner.ScanCommonPortsAsync();
        var openPorts = portResults.Where(p => p.IsOpen).ToList();

        foreach (var port in openPorts)
        {
            OpenPorts.Add(port);
            SecurityIssues.Add($"⚠ Açık port tespit edildi: {port.Port} ({port.ServiceName})");
        }

        ScanStatus = "Güvenlik skoru hesaplanıyor...";
        var report = await _scoreService.CalculateSecurityScoreAsync(
            defenderEnabled, firewallEnabled, updatesEnabled,
            openPorts.Count, 0, 0);

        SecurityScore = report.OverallScore;
        RiskLevel = report.RiskLevel;
        LastScanTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        _lastReport = report;
        
        // Geçmişe ekle
        SecurityHistories.Insert(0, new SecurityHistory
        {
            Timestamp = DateTime.Now,
            SecurityScore = report.OverallScore,
            OpenPortsCount = openPorts.Count,
            DefenderEnabled = defenderEnabled,
            FirewallEnabled = firewallEnabled,
            CpuUsage = CpuUsage,
            RamUsage = RamUsage
        });
        
        // Son 20 kayıt tut
        while (SecurityHistories.Count > 20)
            SecurityHistories.RemoveAt(SecurityHistories.Count - 1);
        
        // Önerileri yükle
        await LoadRecommendationsAsync(report);
        
        // Bildirim göster
        if (report.OverallScore < 60)
        {
            _notificationService.ShowWarning("Güvenlik Uyarısı", 
                $"Güvenlik skorunuz düşük: {report.OverallScore}/100");
        }
        
        ScanStatus = "Tarama tamamlandı";
        IsScanning = false;
    }

    private async Task AnalyzePasswordAsync()
    {
        var result = await _passwordAnalyzer.AnalyzePasswordAsync(PasswordInput);
        
        SecurityIssues.Add($"🔐 Şifre Analizi: {result.Strength} (Skor: {result.Score}/100, Entropy: {result.Entropy:F2})");
        
        // Şifre geçmişine ekle
        var history = new PasswordHistory
        {
            PasswordHash = GetPasswordHash(PasswordInput),
            Score = result.Score,
            AnalyzedAt = DateTime.Now,
            Strength = result.Strength
        };
        
        PasswordHistories.Insert(0, history);
        if (PasswordHistories.Count > 10)
            PasswordHistories.RemoveAt(PasswordHistories.Count - 1);
    }

    private async Task LoadRecommendationsAsync(SecurityReport? report = null)
    {
        if (report == null)
        {
            report = new SecurityReport
            {
                OverallScore = SecurityScore,
                RiskLevel = RiskLevel,
                DefenderEnabled = true,
                FirewallEnabled = true,
                UpdatesEnabled = true,
                OpenPortsCount = OpenPorts.Count
            };
        }

        var recommendations = await _recommendationService.GenerateRecommendationsAsync(report);
        Recommendations.Clear();
        foreach (var rec in recommendations)
        {
            Recommendations.Add(rec);
        }
    }

    private async Task LoadNetworkConnectionsAsync()
    {
        var connections = await _networkMonitor.GetActiveConnectionsAsync();
        NetworkConnections.Clear();
        foreach (var conn in connections)
        {
            NetworkConnections.Add(conn);
        }
    }

    private string GetPasswordHash(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class RelayCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
    public async void Execute(object? parameter) => await _execute();
    
    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }
}
