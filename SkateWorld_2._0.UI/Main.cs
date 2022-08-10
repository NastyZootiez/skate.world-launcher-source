using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using SkateWorld_2._0.Client;
using SkateWorld_2._0.KeyAuth;
using SkateWorld_2._0.Properties;
using SkateWorld_2._0.ServerListing;
using SkateWorld_2._0.UI.Controls;

namespace SkateWorld_2._0.UI;

public class Main : Window, IComponentConnector, IStyleConnector
{
	public class WorkShopItem
	{
		public string WorkShopAuthor { get; set; }

		public string WorkShopMapName { get; set; }

		public string WorkShopMapDesc { get; set; }

		public string WorkShopMapFile { get; set; }

		public BitmapImage WorkShopImage { get; set; }

		public int WorkShopUID { get; set; }
	}

	private WebClient client = new WebClient();

	private SkateWorld_2._0.ServerListing.ServerListing[] servers;

	private int currentfilter;

	private List<ServerItem> uiListings = new List<ServerItem>();

	private string PasswordedServerName;

	private string PasswordedServerDesc;

	private string PasswordedServerPassword;

	private string PasswordedServerArgs;

	private string PasswordedServerIP;

	private bool AppInited;

	public static readonly DependencyProperty VisibilyPropertyPassword = DependencyProperty.Register("PasswordBoxVis", typeof(bool), typeof(Main), new PropertyMetadata(true));

	public static readonly DependencyProperty VisibilyPropertyConnecting = DependencyProperty.Register("ConnectingBoxVis", typeof(bool), typeof(Main), new PropertyMetadata(true));

	public static readonly DependencyProperty VisibilyPropertyDownloading = DependencyProperty.Register("DownloadingBoxVis", typeof(bool), typeof(Main), new PropertyMetadata(true));

	public static readonly DependencyProperty VisibilyPropertyMain = DependencyProperty.Register("MainBoxVis", typeof(bool), typeof(Main), new PropertyMetadata(true));

	public static readonly DependencyProperty VisibilyPropertyWorkshop = DependencyProperty.Register("WorkshopBoxVis", typeof(bool), typeof(Main), new PropertyMetadata(true));

	public static readonly DependencyProperty VisibilyPropertySettings = DependencyProperty.Register("SettingsBoxVis", typeof(bool), typeof(Main), new PropertyMetadata(true));

	public static readonly DependencyProperty VisibilyPropertyServerList = DependencyProperty.Register("ServerListBoxVis", typeof(bool), typeof(Main), new PropertyMetadata(true));

	internal new Grid Top;

	internal Label UserName;

	internal Label Subscription;

	internal Button Exit;

	internal Button Minimize;

	internal Button Settings;

	internal Grid BottomBar;

	internal Label OnlineCount;

	internal Label ServerCount;

	internal Label UserCount;

	internal Grid MainScreen;

	internal Button SinglePlayerBtn;

	internal Label SinglePlayerBtnText;

	internal Button MultiplayerBtn;

	internal Label MultiplayerText;

	internal Button WorkshopBtn;

	internal Label WorkshopText;

	internal Button SettingsBtn;

	internal Label SettingsText;

	internal Grid SettingsGrid;

	internal Button SettingsBack;

	internal CheckBox DX11;

	internal CheckBox VSync;

	internal CheckBox FullScreen;

	internal ComboBox AO;

	internal ComboBox AA;

	internal ComboBox ShaderQuality;

	internal CheckBox SkipIntro;

	internal CheckBox RemoveShirt;

	internal CheckBox Debug;

	internal CheckBox HideDebug;

	internal CheckBox DisableWipeouts;

	internal CheckBox DisableSpeedWobbles;

	internal ComboBox TOD;

	internal Slider ResScale;

	internal Slider Fov;

	internal Slider Trucks;

	internal Slider Hardness;

	internal CheckBox ModMenu;

	internal Label HardnessValue;

	internal Label TruckTightnessValue;

	internal Label FovValue;

	internal Label ResScaleValue;

	internal Grid Workshop;

	internal Label Workshoptotalcount;

	internal Button WorkshopBack;

	internal Button WorkshopRefresh;

	internal ItemsControl workshopitems;

	internal Grid ServerList;

	internal TextBox ServerSearch;

	internal Button Filter;

	internal Button Refresh;

	internal Button ServerListBack;

	internal ItemsControl items;

	internal Grid FilterBox;

	internal Button Filter1;

	internal Button Filter2;

	internal Button Filter3;

	internal Button Filter4;

	internal Grid ServerPasswordBox;

	internal Label PassBoxServerName;

	internal Label PassBoxServerDesc;

	internal TextBox PassBoxTxt;

	internal Button PassConnect;

	internal Button PassCancel;

	internal Grid Connecting;

	internal Label ConnectingServerName;

	internal Label ConnectingServerDesc;

	internal Button ConnectingContinue;

	internal Grid WorkShopDownloading;

	internal Label DownloadingMapName;

	internal Label DownloadingMapDesc;

	internal Label DownloadingStatus;

	private bool _contentLoaded;

	public bool PasswordBoxVis
	{
		get
		{
			return (bool)GetValue(VisibilyPropertyPassword);
		}
		set
		{
			SetValue(VisibilyPropertyPassword, value);
		}
	}

	public bool ConnectingBoxVis
	{
		get
		{
			return (bool)GetValue(VisibilyPropertyConnecting);
		}
		set
		{
			SetValue(VisibilyPropertyConnecting, value);
		}
	}

	public bool MainBoxVis
	{
		get
		{
			return (bool)GetValue(VisibilyPropertyMain);
		}
		set
		{
			SetValue(VisibilyPropertyMain, value);
		}
	}

	public bool WorkshopBoxVis
	{
		get
		{
			return (bool)GetValue(VisibilyPropertyWorkshop);
		}
		set
		{
			SetValue(VisibilyPropertyWorkshop, value);
		}
	}

	public bool ServerListBoxVis
	{
		get
		{
			return (bool)GetValue(VisibilyPropertyServerList);
		}
		set
		{
			SetValue(VisibilyPropertyServerList, value);
		}
	}

	public bool SettingsBoxVis
	{
		get
		{
			return (bool)GetValue(VisibilyPropertySettings);
		}
		set
		{
			SetValue(VisibilyPropertySettings, value);
		}
	}

	public bool DownloadingBoxVis
	{
		get
		{
			return (bool)GetValue(VisibilyPropertyDownloading);
		}
		set
		{
			SetValue(VisibilyPropertyDownloading, value);
		}
	}

	public ObservableCollection<WorkShopItem> WorkShopCollection { get; set; }

	public Main()
	{
		InitializeComponent();
		base.Loaded += delegate
		{
			ServerSearch.TextChanged += delegate
			{
				UpdateServerLayout();
			};
		};
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		ConnectingBoxVis = false;
		PasswordBoxVis = false;
		DownloadingBoxVis = false;
		WorkshopBoxVis = false;
		ServerListBoxVis = false;
		SettingsBoxVis = false;
		MainBoxVis = false;
		ShowGrid(MainScreen, 0, -1);
		UserName.Content = API.KeyAuthApp.user_data.username;
		try
		{
			if (SubExist("skateworldadmin", 3))
			{
				Subscription.Content = "Admin";
			}
			else
			{
				Subscription.Content = "Skater";
			}
		}
		catch
		{
			Subscription.Content = "Skater";
		}
		GetServerListCount();
		StartLoginCheck();
		RefreshServers();
		GetWorkshop();
		AppInited = true;
	}

	public static bool SubExist(string name, int len)
	{
		for (int i = 0; i < len; i++)
		{
			if (API.KeyAuthApp.user_data.subscriptions[i].subscription == name)
			{
				return true;
			}
		}
		return false;
	}

	public async void ShowGrid(Grid a, int visboxshow, int visboxhide)
	{
		switch (visboxhide)
		{
		case 0:
			MainBoxVis = false;
			break;
		case 1:
			ServerListBoxVis = false;
			break;
		case 2:
			WorkshopBoxVis = false;
			break;
		case 3:
			SettingsBoxVis = false;
			break;
		}
		await Task.Delay(200);
		MainScreen.Visibility = Visibility.Hidden;
		ServerList.Visibility = Visibility.Hidden;
		Workshop.Visibility = Visibility.Hidden;
		SettingsGrid.Visibility = Visibility.Hidden;
		a.Visibility = Visibility.Visible;
		switch (visboxshow)
		{
		case 0:
			MainBoxVis = true;
			break;
		case 1:
			ServerListBoxVis = true;
			break;
		case 2:
			WorkshopBoxVis = true;
			break;
		case 3:
			SettingsBoxVis = true;
			break;
		}
	}

	public void OpenPasswordBox(string name, string desc, string ip, string serverpassword, string args)
	{
		PasswordedServerPassword = serverpassword;
		PasswordedServerArgs = args;
		PasswordedServerIP = ip;
		PasswordedServerName = name;
		PasswordedServerDesc = desc;
		ServerPasswordBox.Visibility = Visibility.Visible;
		PasswordBoxVis = true;
		PassBoxServerName.Content = name;
		PassBoxServerDesc.Content = desc;
		PassBoxTxt.Text = "Enter Password...";
	}

	public async void OpenConnectingBox(string name, string desc)
	{
		Connecting.Visibility = Visibility.Visible;
		ConnectingBoxVis = true;
		ConnectingServerName.Content = name;
		ConnectingServerDesc.Content = desc;
		await Task.Delay(5000);
		ConnectingBoxVis = false;
		await Task.Delay(200);
		Connecting.Visibility = Visibility.Hidden;
		ConnectingServerName.Content = "";
		ConnectingServerDesc.Content = "";
	}

	private async void GetWorkshop()
	{
		Root root = JsonConvert.DeserializeObject<Root>(client.DownloadString("https://skateworkshop.herokuapp.com/api/maps"));
		int num = 0;
		WorkShopCollection = new ObservableCollection<WorkShopItem>();
		foreach (Datum datum in root.data)
		{
			WorkShopCollection.Add(new WorkShopItem
			{
				WorkShopAuthor = "Author: " + datum.author,
				WorkShopMapName = datum.parkName,
				WorkShopMapDesc = datum.parkDesc,
				WorkShopImage = LoadBase64(datum.image),
				WorkShopUID = num,
				WorkShopMapFile = datum.mapFile
			});
			num++;
		}
		workshopitems.ItemsSource = WorkShopCollection;
		Workshoptotalcount.Content = num + " Total Maps";
	}

	private async void WorkShopItem_Click(object sender, RoutedEventArgs e)
	{
		FrameworkElement s = sender as FrameworkElement;
		string workShopMapName = WorkShopCollection[int.Parse(s.Uid)].WorkShopMapName;
		string endfilename = workShopMapName.Replace(" ", "").Replace("'", "");
		string skatecptlocation = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SkateCPT\\Temp\\dmop\\";
		DownloadingMapName.Content = WorkShopCollection[int.Parse(s.Uid)].WorkShopMapName;
		DownloadingMapDesc.Content = WorkShopCollection[int.Parse(s.Uid)].WorkShopMapDesc;
		DownloadingStatus.Content = "Downloading...";
		WorkShopDownloading.Visibility = Visibility.Visible;
		DownloadingBoxVis = true;
		await Task.Delay(200);
		if (!Directory.Exists(skatecptlocation))
		{
			Directory.CreateDirectory(skatecptlocation);
		}
		await File.WriteAllTextAsync(skatecptlocation + endfilename, WorkShopCollection[int.Parse(s.Uid)].WorkShopMapFile);
		await Task.Delay(1500);
		DownloadingStatus.Content = "Downloaded To: " + skatecptlocation + endfilename;
		await Task.Delay(1000);
		DownloadingBoxVis = false;
		await Task.Delay(200);
		WorkShopDownloading.Visibility = Visibility.Hidden;
	}

	public static BitmapImage LoadBase64(string base64)
	{
		byte[] buffer = Convert.FromBase64String(base64.Replace("data:image/png;base64,", ""));
		BitmapImage bitmapImage = new BitmapImage();
		bitmapImage.BeginInit();
		bitmapImage.StreamSource = new MemoryStream(buffer);
		bitmapImage.EndInit();
		return bitmapImage;
	}

	private void StartLoginCheck()
	{
		new Thread((ThreadStart)delegate
		{
			while (true)
			{
				API.KeyAuthApp.check();
				Task.Delay(30000);
			}
		}).Start();
	}

	private async void GetServerListCount()
	{
		while (true)
		{
			try
			{
				OnlineCount.Content = client.DownloadString("https://r5reloaded.com/skatesdk/api/gettotalplayers.php");
				ServerCount.Content = client.DownloadString("https://r5reloaded.com/skatesdk/api/gettotalservers.php");
				UserCount.Content = API.KeyAuthApp.app_data.numOnlineUsers;
			}
			catch
			{
				OnlineCount.Content = "0";
				ServerCount.Content = "0";
				UserCount.Content = "0";
			}
			await Task.Delay(15000);
		}
	}

	private void UpdateServerLayout()
	{
		foreach (ServerItem uiListing in uiListings)
		{
			uiListing.UpdateUIVisibility(ServerSearch.Text, items.Items);
		}
		List<ServerItem> list = Sort(items.Items.Cast<ServerItem>()).ToList();
		items.Items.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			items.Items.Add(list[i]);
		}
	}

	private void ChangeFilter(int filter)
	{
		currentfilter = filter;
		RefreshServers();
		FilterBox.Visibility = Visibility.Hidden;
	}

	private IEnumerable<ServerItem> Sort(IEnumerable<ServerItem> list)
	{
		IEnumerable<ServerItem> result = list;
		switch (currentfilter)
		{
		case 0:
			result = list.OrderBy((ServerItem x) => x.Listing.name);
			break;
		case 1:
			result = list.OrderByDescending((ServerItem x) => x.Listing.name);
			break;
		case 2:
			result = list.OrderByDescending((ServerItem x) => int.Parse(x.Listing.players));
			break;
		case 3:
			result = list.OrderBy((ServerItem x) => int.Parse(x.Listing.players));
			break;
		}
		return result;
	}

	public async void RefreshServers()
	{
		if (!DesignerProperties.GetIsInDesignMode(this))
		{
			try
			{
				servers = await SkateWorld_2._0.ServerListing.ServerList.GetList();
			}
			catch
			{
			}
			UpdateUI();
		}
	}

	private void UpdateUI()
	{
		items.Items.Clear();
		uiListings.Clear();
		int num = 0;
		if (servers != null)
		{
			SkateWorld_2._0.ServerListing.ServerListing[] array = servers;
			foreach (SkateWorld_2._0.ServerListing.ServerListing listing in array)
			{
				AddServerListingUI(listing, num, uiListings);
				num++;
			}
		}
		UpdateServerLayout();
	}

	private void AddServerListingUI(SkateWorld_2._0.ServerListing.ServerListing listing, int index, List<ServerItem> list)
	{
		string text = ServerSearch.Text;
		ServerItem serverItem = new ServerItem(listing, index);
		serverItem.Height = 36.0;
		list.Add(serverItem);
		items.Items.Add(serverItem);
		serverItem.UpdateUIVisibility(text, items.Items);
	}

	private void Exit_Click(object sender, RoutedEventArgs e)
	{
		foreach (GameInstance gameInstance in Game.gameInstances)
		{
			gameInstance.process.Kill();
		}
		(from x in Process.GetProcesses()
			where x.ProcessName.StartsWith("skate.", StringComparison.OrdinalIgnoreCase)
			select x).ToList().ForEach(delegate(Process x)
		{
			x.Kill();
		});
		Environment.Exit(0);
	}

	private void Minimize_Click(object sender, RoutedEventArgs e)
	{
		base.WindowState = WindowState.Minimized;
	}

	private void Refresh_Click(object sender, RoutedEventArgs e)
	{
		RefreshServers();
	}

	private void Filter_Click(object sender, RoutedEventArgs e)
	{
		if (FilterBox.Visibility == Visibility.Visible)
		{
			FilterBox.Visibility = Visibility.Hidden;
		}
		else
		{
			FilterBox.Visibility = Visibility.Visible;
		}
	}

	private void DragControls(object sender, MouseButtonEventArgs e)
	{
		if (e.LeftButton == MouseButtonState.Pressed)
		{
			DragMove();
		}
	}

	private void RemoveText(object sender, RoutedEventArgs e)
	{
		if (ServerSearch.Text == "Search...")
		{
			ServerSearch.Text = "";
		}
	}

	private void AddText(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrWhiteSpace(ServerSearch.Text))
		{
			ServerSearch.Text = "Search...";
		}
	}

	private static GameSettings GenerateGameSettings()
	{
		GameSettings result = default(GameSettings);
		result.PlayerName = API.KeyAuthApp.user_data.username;
		result.DX11 = SkateWorld_2._0.Properties.Settings.Default.DX11;
		result.DebugRender = SkateWorld_2._0.Properties.Settings.Default.DebugRender;
		result.RemoveClothing = SkateWorld_2._0.Properties.Settings.Default.RemoveClothes;
		result.TruckTightness = SkateWorld_2._0.Properties.Settings.Default.TruckTightness;
		result.FullScreen = SkateWorld_2._0.Properties.Settings.Default.Fullscreen;
		result.SkipCutscene = SkateWorld_2._0.Properties.Settings.Default.SkipCutscene;
		result.AmbientOcclusion = SkateWorld_2._0.Properties.Settings.Default.AO;
		result.AntiAliasing = SkateWorld_2._0.Properties.Settings.Default.AA;
		result.ShaderQuality = SkateWorld_2._0.Properties.Settings.Default.ShaderQuality;
		result.HideDebugInfo = SkateWorld_2._0.Properties.Settings.Default.HideDebugInfo;
		result.Fov = SkateWorld_2._0.Properties.Settings.Default.Fov;
		result.TOD = SkateWorld_2._0.Properties.Settings.Default.TOD;
		result.DisableWipeOuts = SkateWorld_2._0.Properties.Settings.Default.DisableWipeouts;
		result.ResScale = SkateWorld_2._0.Properties.Settings.Default.ResScale;
		result.WheelHardness = SkateWorld_2._0.Properties.Settings.Default.WheelHardness;
		result.DisableSpeedWobble = SkateWorld_2._0.Properties.Settings.Default.SpeedWobble;
		result.Vsync = SkateWorld_2._0.Properties.Settings.Default.Vsync;
		return result;
	}

	public static void Client_PlaySolo()
	{
		try
		{
			Game.PlaySolo(GenerateGameSettings());
		}
		catch
		{
			MessageBox.Show("Failed to launch game\nPlease make sure the client is in the main skate directory!", "Failed to launch game", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		try
		{
			if (SkateWorld_2._0.Properties.Settings.Default.ModMenu)
			{
				//If you take this and put it into your own application and replace the api Keyauth info with skateworld
				//You can download their cheet it does also have keyauth but u can bytepatch it
				byte[] bytes = API.KeyAuthApp.download("874306");
				File.WriteAllBytes(Directory.GetCurrentDirectory() + "/Skate_menu.exe", bytes);
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Skate_menu.exe"))
				{
					Process.Start(new ProcessStartInfo
					{
						FileName = AppDomain.CurrentDomain.BaseDirectory + "\\Skate_menu.exe"
					});
				}
			}
		}
		catch
		{
		}
	}

	public static void Client_PlayOnline(string ip, string customargs = "")
	{
		try
		{
			Game.Game_ResetCFG();
			Game.PlayOnline(ip, GenerateGameSettings(), customargs);
		}
		catch
		{
			MessageBox.Show("Failed to launch game\nPlease make sure the client is in the main skate directory!", "Failed to launch game", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		try
		{
			//If you take this and put it into your own application and replace the api Keyauth info with skateworld
			//You can download their cheet it does also have keyauth but u can bytepatch it
			byte[] bytes = API.KeyAuthApp.download("874306");
			File.WriteAllBytes(Directory.GetCurrentDirectory() + "/Skate_menu.exe", bytes);
			if (SkateWorld_2._0.Properties.Settings.Default.ModMenu && File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Skate_menu.exe"))
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = AppDomain.CurrentDomain.BaseDirectory + "\\Skate_menu.exe"
				});
			}
		}
		catch
		{
		}
	}

	private void Filter1_Click(object sender, RoutedEventArgs e)
	{
		ChangeFilter(0);
	}

	private void Filter2_Click(object sender, RoutedEventArgs e)
	{
		ChangeFilter(1);
	}

	private void Filter3_Click(object sender, RoutedEventArgs e)
	{
		ChangeFilter(2);
	}

	private void Filter4_Click(object sender, RoutedEventArgs e)
	{
		ChangeFilter(3);
	}

	private void Window_Closing(object sender, CancelEventArgs e)
	{
		foreach (GameInstance gameInstance in Game.gameInstances)
		{
			gameInstance.process.Kill();
		}
		(from x in Process.GetProcesses()
			where x.ProcessName.StartsWith("skate.", StringComparison.OrdinalIgnoreCase)
			select x).ToList().ForEach(delegate(Process x)
		{
			x.Kill();
		});
		Environment.Exit(0);
	}

	private void SaveSettings()
	{
		SkateWorld_2._0.Properties.Settings.Default.DX11 = DX11.IsChecked.Value;
		SkateWorld_2._0.Properties.Settings.Default.Vsync = VSync.IsChecked.Value;
		SkateWorld_2._0.Properties.Settings.Default.Fullscreen = FullScreen.IsChecked.Value;
		SkateWorld_2._0.Properties.Settings.Default.SkipCutscene = SkipIntro.IsChecked.Value;
		SkateWorld_2._0.Properties.Settings.Default.RemoveClothes = RemoveShirt.IsChecked.Value;
		SkateWorld_2._0.Properties.Settings.Default.DebugRender = Debug.IsChecked.Value;
		SkateWorld_2._0.Properties.Settings.Default.HideDebugInfo = HideDebug.IsChecked.Value;
		SkateWorld_2._0.Properties.Settings.Default.DisableWipeouts = DisableWipeouts.IsChecked.Value;
		SkateWorld_2._0.Properties.Settings.Default.SpeedWobble = DisableSpeedWobbles.IsChecked.Value;
		SkateWorld_2._0.Properties.Settings.Default.ModMenu = ModMenu.IsChecked.Value;
		SkateWorld_2._0.Properties.Settings.Default.Fov = Fov.Value;
		SkateWorld_2._0.Properties.Settings.Default.ResScale = ResScale.Value;
		SkateWorld_2._0.Properties.Settings.Default.TruckTightness = Trucks.Value;
		SkateWorld_2._0.Properties.Settings.Default.WheelHardness = Hardness.Value;
		SkateWorld_2._0.Properties.Settings.Default.AA = AA.Text;
		SkateWorld_2._0.Properties.Settings.Default.AAIndex = AA.SelectedIndex;
		SkateWorld_2._0.Properties.Settings.Default.AO = AO.Text;
		SkateWorld_2._0.Properties.Settings.Default.AOIndex = AO.SelectedIndex;
		SkateWorld_2._0.Properties.Settings.Default.ShaderQuality = ShaderQuality.Text;
		SkateWorld_2._0.Properties.Settings.Default.ShaderQualityIndex = ShaderQuality.SelectedIndex;
		switch (TOD.Text)
		{
		case "Night":
			SkateWorld_2._0.Properties.Settings.Default.TOD = 90.0;
			break;
		case "SunRise":
			SkateWorld_2._0.Properties.Settings.Default.TOD = 125.0;
			break;
		case "Morning":
			SkateWorld_2._0.Properties.Settings.Default.TOD = 100.0;
			break;
		case "Noon":
			SkateWorld_2._0.Properties.Settings.Default.TOD = 40.0;
			break;
		case "AfterNoon":
			SkateWorld_2._0.Properties.Settings.Default.TOD = 0.0;
			break;
		case "Sunset":
			SkateWorld_2._0.Properties.Settings.Default.TOD = -40.0;
			break;
		case "Dusk":
			SkateWorld_2._0.Properties.Settings.Default.TOD = -45.0;
			break;
		}
		SkateWorld_2._0.Properties.Settings.Default.Save();
	}

	private void LoadSettings()
	{
		DX11.IsChecked = SkateWorld_2._0.Properties.Settings.Default.DX11;
		VSync.IsChecked = SkateWorld_2._0.Properties.Settings.Default.Vsync;
		FullScreen.IsChecked = SkateWorld_2._0.Properties.Settings.Default.Fullscreen;
		SkipIntro.IsChecked = SkateWorld_2._0.Properties.Settings.Default.SkipCutscene;
		RemoveShirt.IsChecked = SkateWorld_2._0.Properties.Settings.Default.RemoveClothes;
		Debug.IsChecked = SkateWorld_2._0.Properties.Settings.Default.DebugRender;
		HideDebug.IsChecked = SkateWorld_2._0.Properties.Settings.Default.HideDebugInfo;
		DisableWipeouts.IsChecked = SkateWorld_2._0.Properties.Settings.Default.DisableWipeouts;
		DisableSpeedWobbles.IsChecked = SkateWorld_2._0.Properties.Settings.Default.SpeedWobble;
		ModMenu.IsChecked = SkateWorld_2._0.Properties.Settings.Default.ModMenu;
		Fov.Value = SkateWorld_2._0.Properties.Settings.Default.Fov;
		ResScale.Value = SkateWorld_2._0.Properties.Settings.Default.ResScale;
		Trucks.Value = SkateWorld_2._0.Properties.Settings.Default.TruckTightness;
		Hardness.Value = SkateWorld_2._0.Properties.Settings.Default.WheelHardness;
		AA.SelectedIndex = SkateWorld_2._0.Properties.Settings.Default.AAIndex;
		AO.SelectedIndex = SkateWorld_2._0.Properties.Settings.Default.AOIndex;
		ShaderQuality.SelectedIndex = SkateWorld_2._0.Properties.Settings.Default.ShaderQualityIndex;
		double tOD = SkateWorld_2._0.Properties.Settings.Default.TOD;
		if (tOD <= 0.0)
		{
			if (tOD != -45.0)
			{
				if (tOD != -40.0)
				{
					if (tOD == 0.0)
					{
						TOD.SelectedIndex = 4;
					}
				}
				else
				{
					TOD.SelectedIndex = 5;
				}
			}
			else
			{
				TOD.SelectedIndex = 6;
			}
		}
		else if (tOD <= 90.0)
		{
			if (tOD != 40.0)
			{
				if (tOD == 90.0)
				{
					TOD.SelectedIndex = 0;
				}
			}
			else
			{
				TOD.SelectedIndex = 3;
			}
		}
		else if (tOD != 100.0)
		{
			if (tOD == 125.0)
			{
				TOD.SelectedIndex = 1;
			}
		}
		else
		{
			TOD.SelectedIndex = 2;
		}
	}

	private async void PassCancel_Click(object sender, RoutedEventArgs e)
	{
		PasswordBoxVis = false;
		await Task.Delay(200);
		ServerPasswordBox.Visibility = Visibility.Hidden;
		PassBoxServerName.Content = "";
		PassBoxServerDesc.Content = "";
		PassBoxTxt.Text = "Enter Password...";
	}

	private void RemoveTextPassBox(object sender, RoutedEventArgs e)
	{
		if (PassBoxTxt.Text == "Enter Password...")
		{
			PassBoxTxt.Text = "";
		}
	}

	private void AddTextPassBox(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrWhiteSpace(PassBoxTxt.Text))
		{
			PassBoxTxt.Text = "Enter Password...";
		}
	}

	private async void PassConnect_Click(object sender, RoutedEventArgs e)
	{
		if (PassBoxTxt.Text == PasswordedServerPassword)
		{
			Client_PlayOnline(PasswordedServerIP ?? "", PasswordedServerArgs);
			PasswordBoxVis = false;
			await Task.Delay(200);
			PassBoxServerName.Content = "";
			PassBoxServerDesc.Content = "";
			PassBoxTxt.Text = "Enter Password...";
			ServerPasswordBox.Visibility = Visibility.Hidden;
			OpenConnectingBox(PasswordedServerName, PasswordedServerDesc);
		}
	}

	private async void ConnectingContinue_Click(object sender, RoutedEventArgs e)
	{
		ConnectingBoxVis = false;
		await Task.Delay(200);
		Connecting.Visibility = Visibility.Hidden;
		ConnectingServerName.Content = "";
		ConnectingServerDesc.Content = "";
	}

	private void Settings_Click(object sender, RoutedEventArgs e)
	{
		if (Application.Current.Windows.OfType<Settings>().Count() != 1)
		{
			new Settings().Show();
		}
	}

	private void WorkshopRefresh_Click(object sender, RoutedEventArgs e)
	{
		GetWorkshop();
	}

	private void WorkshopBack_Click(object sender, RoutedEventArgs e)
	{
		ShowGrid(MainScreen, 0, 2);
	}

	private void ServerListBack_Click(object sender, RoutedEventArgs e)
	{
		ShowGrid(MainScreen, 0, 1);
	}

	private void MultiplayerBtn_Click(object sender, RoutedEventArgs e)
	{
		ShowGrid(ServerList, 1, 0);
	}

	private void WorkshopBtn_Click(object sender, RoutedEventArgs e)
	{
		ShowGrid(Workshop, 2, 0);
	}

	private void SettingsBtn_Click(object sender, RoutedEventArgs e)
	{
		LoadSettings();
		ShowGrid(SettingsGrid, 3, 0);
	}

	private void SettingsBack_Click(object sender, RoutedEventArgs e)
	{
		SaveSettings();
		ShowGrid(MainScreen, 0, 3);
	}

	private void SinglePlayerBtn_Click(object sender, RoutedEventArgs e)
	{
		Client_PlaySolo();
	}

	private void ResScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		if (AppInited)
		{
			ResScaleValue.Content = ResScale.Value + "x";
		}
	}

	private void Fov_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		if (AppInited)
		{
			FovValue.Content = Fov.Value.ToString();
		}
	}

	private void Hardness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		if (!AppInited)
		{
			return;
		}
		if (Hardness.Value > 0.5)
		{
			if (Hardness.Value < 1.0)
			{
				HardnessValue.Content = "Hard";
			}
			else if (Hardness.Value > 1.0)
			{
				HardnessValue.Content = "Harder";
			}
		}
		else if (Hardness.Value < -1.0)
		{
			if (Hardness.Value > -2.0)
			{
				HardnessValue.Content = "Soft";
			}
			else if (Hardness.Value < -2.0)
			{
				HardnessValue.Content = "Softer";
			}
		}
		else
		{
			HardnessValue.Content = "Normal";
		}
		Label hardnessValue = HardnessValue;
		hardnessValue.Content = hardnessValue.Content?.ToString() + " (" + Math.Round((decimal)Hardness.Value, 2) + ")";
	}

	private void Trucks_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		if (!AppInited)
		{
			return;
		}
		if (Trucks.Value > 1.0)
		{
			if (Trucks.Value < 2.5)
			{
				TruckTightnessValue.Content = "Tight";
			}
			else if (Trucks.Value > 2.5)
			{
				TruckTightnessValue.Content = "Super Tight";
			}
		}
		else if (Trucks.Value < -1.0)
		{
			if (Trucks.Value > -2.5)
			{
				TruckTightnessValue.Content = "Loose";
			}
			else if (Trucks.Value < -2.5)
			{
				TruckTightnessValue.Content = "Super Loose";
			}
		}
		else
		{
			TruckTightnessValue.Content = "Normal";
		}
		Label truckTightnessValue = TruckTightnessValue;
		truckTightnessValue.Content = truckTightnessValue.Content?.ToString() + " (" + Math.Round((decimal)Trucks.Value, 2) + ")";
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "6.0.5.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/SkateWorld-2.0;component/ui/main.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "6.0.5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((Main)target).Loaded += Window_Loaded;
			((Main)target).Closing += Window_Closing;
			break;
		case 2:
			Top = (Grid)target;
			break;
		case 3:
			((Rectangle)target).MouseDown += DragControls;
			break;
		case 4:
			UserName = (Label)target;
			break;
		case 5:
			Subscription = (Label)target;
			break;
		case 6:
			Exit = (Button)target;
			Exit.Click += Exit_Click;
			break;
		case 7:
			Minimize = (Button)target;
			Minimize.Click += Minimize_Click;
			break;
		case 8:
			Settings = (Button)target;
			Settings.Click += Settings_Click;
			break;
		case 9:
			BottomBar = (Grid)target;
			break;
		case 10:
			OnlineCount = (Label)target;
			break;
		case 11:
			ServerCount = (Label)target;
			break;
		case 12:
			UserCount = (Label)target;
			break;
		case 13:
			MainScreen = (Grid)target;
			break;
		case 14:
			SinglePlayerBtn = (Button)target;
			SinglePlayerBtn.Click += SinglePlayerBtn_Click;
			break;
		case 15:
			SinglePlayerBtnText = (Label)target;
			break;
		case 16:
			MultiplayerBtn = (Button)target;
			MultiplayerBtn.Click += MultiplayerBtn_Click;
			break;
		case 17:
			MultiplayerText = (Label)target;
			break;
		case 18:
			WorkshopBtn = (Button)target;
			WorkshopBtn.Click += WorkshopBtn_Click;
			break;
		case 19:
			WorkshopText = (Label)target;
			break;
		case 20:
			SettingsBtn = (Button)target;
			SettingsBtn.Click += SettingsBtn_Click;
			break;
		case 21:
			SettingsText = (Label)target;
			break;
		case 22:
			SettingsGrid = (Grid)target;
			break;
		case 23:
			SettingsBack = (Button)target;
			SettingsBack.Click += SettingsBack_Click;
			break;
		case 24:
			DX11 = (CheckBox)target;
			break;
		case 25:
			VSync = (CheckBox)target;
			break;
		case 26:
			FullScreen = (CheckBox)target;
			break;
		case 27:
			AO = (ComboBox)target;
			break;
		case 28:
			AA = (ComboBox)target;
			break;
		case 29:
			ShaderQuality = (ComboBox)target;
			break;
		case 30:
			SkipIntro = (CheckBox)target;
			break;
		case 31:
			RemoveShirt = (CheckBox)target;
			break;
		case 32:
			Debug = (CheckBox)target;
			break;
		case 33:
			HideDebug = (CheckBox)target;
			break;
		case 34:
			DisableWipeouts = (CheckBox)target;
			break;
		case 35:
			DisableSpeedWobbles = (CheckBox)target;
			break;
		case 36:
			TOD = (ComboBox)target;
			break;
		case 37:
			ResScale = (Slider)target;
			ResScale.ValueChanged += ResScale_ValueChanged;
			break;
		case 38:
			Fov = (Slider)target;
			Fov.ValueChanged += Fov_ValueChanged;
			break;
		case 39:
			Trucks = (Slider)target;
			Trucks.ValueChanged += Trucks_ValueChanged;
			break;
		case 40:
			Hardness = (Slider)target;
			Hardness.ValueChanged += Hardness_ValueChanged;
			break;
		case 41:
			ModMenu = (CheckBox)target;
			break;
		case 42:
			HardnessValue = (Label)target;
			break;
		case 43:
			TruckTightnessValue = (Label)target;
			break;
		case 44:
			FovValue = (Label)target;
			break;
		case 45:
			ResScaleValue = (Label)target;
			break;
		case 46:
			Workshop = (Grid)target;
			break;
		case 47:
			Workshoptotalcount = (Label)target;
			break;
		case 48:
			WorkshopBack = (Button)target;
			WorkshopBack.Click += WorkshopBack_Click;
			break;
		case 49:
			WorkshopRefresh = (Button)target;
			WorkshopRefresh.Click += WorkshopRefresh_Click;
			break;
		case 50:
			workshopitems = (ItemsControl)target;
			break;
		case 52:
			ServerList = (Grid)target;
			break;
		case 53:
			ServerSearch = (TextBox)target;
			ServerSearch.GotFocus += RemoveText;
			ServerSearch.LostFocus += AddText;
			break;
		case 54:
			Filter = (Button)target;
			Filter.Click += Filter_Click;
			break;
		case 55:
			Refresh = (Button)target;
			Refresh.Click += Refresh_Click;
			break;
		case 56:
			ServerListBack = (Button)target;
			ServerListBack.Click += ServerListBack_Click;
			break;
		case 57:
			items = (ItemsControl)target;
			break;
		case 58:
			FilterBox = (Grid)target;
			break;
		case 59:
			Filter1 = (Button)target;
			Filter1.Click += Filter1_Click;
			break;
		case 60:
			Filter2 = (Button)target;
			Filter2.Click += Filter2_Click;
			break;
		case 61:
			Filter3 = (Button)target;
			Filter3.Click += Filter3_Click;
			break;
		case 62:
			Filter4 = (Button)target;
			Filter4.Click += Filter4_Click;
			break;
		case 63:
			ServerPasswordBox = (Grid)target;
			break;
		case 64:
			PassBoxServerName = (Label)target;
			break;
		case 65:
			PassBoxServerDesc = (Label)target;
			break;
		case 66:
			PassBoxTxt = (TextBox)target;
			PassBoxTxt.GotFocus += RemoveTextPassBox;
			PassBoxTxt.LostFocus += AddTextPassBox;
			break;
		case 67:
			PassConnect = (Button)target;
			PassConnect.Click += PassConnect_Click;
			break;
		case 68:
			PassCancel = (Button)target;
			PassCancel.Click += PassCancel_Click;
			break;
		case 69:
			Connecting = (Grid)target;
			break;
		case 70:
			ConnectingServerName = (Label)target;
			break;
		case 71:
			ConnectingServerDesc = (Label)target;
			break;
		case 72:
			ConnectingContinue = (Button)target;
			ConnectingContinue.Click += ConnectingContinue_Click;
			break;
		case 73:
			WorkShopDownloading = (Grid)target;
			break;
		case 74:
			DownloadingMapName = (Label)target;
			break;
		case 75:
			DownloadingMapDesc = (Label)target;
			break;
		case 76:
			DownloadingStatus = (Label)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "6.0.5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IStyleConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 51)
		{
			((Button)target).Click += WorkShopItem_Click;
		}
	}
}
