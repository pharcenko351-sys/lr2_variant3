using Microsoft.Extensions.DependencyInjection;

namespace DormitoryXml.UI;

public partial class App : Application
{
	public static bool ExitConfirmed { get; set; }

	public App()
	{
		InitializeComponent();
		UserAppTheme = AppTheme.Dark;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new AppShell()) {
			Width = 1000, Height = 700,
			MinimumWidth = 800, MinimumHeight = 600,
			Title = "Гуртожиток · XML"
		};

#if WINDOWS
		bool subscribed = false;
		window.HandlerChanged += (_, _) =>
		{
			if (subscribed || window.Handler?.PlatformView is not Microsoft.UI.Xaml.Window native) return;
			subscribed = true;
			native.AppWindow.Closing += async (_, args) =>
			{
				if (ExitConfirmed) return;
				args.Cancel = true;
				if (window.Page is null) return;
				bool yes = await window.Page.DisplayAlertAsync("Вихід", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
				if (!yes) return;
				ExitConfirmed = true;
				Application.Current?.Quit();
			};
		};
#endif

		return window;
	}
}
