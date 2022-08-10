using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using SkateWorld_2._0.ServerListing;

namespace SkateWorld_2._0.UI.Controls;

public class ServerItem : UserControl, IComponentConnector
{
	private SkateWorld_2._0.ServerListing.ServerListing listing;

	private int index;

	internal Image FlagIcon;

	internal Label ServerName;

	internal Label PlayerCount;

	internal Label ServerDescription;

	internal Button ConnectBtn;

	internal Label ConnectBtnlbl;

	internal Image ServerLocked;

	private bool _contentLoaded;

	public SkateWorld_2._0.ServerListing.ServerListing Listing
	{
		get
		{
			return listing;
		}
		private set
		{
			listing = value;
		}
	}

	public ServerItem(SkateWorld_2._0.ServerListing.ServerListing serverListing, int index)
	{
		listing = serverListing;
		this.index = index;
		string text = Encryption.Decrypt(listing.ip, ServerList.IPKey);
		string text2 = Encryption.Decrypt(listing.password, ServerList.IPKey);
		if (text != null)
		{
			listing.ip = text;
		}
		if (text2 != null)
		{
			listing.password = text2;
		}
		InitializeComponent();
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		ServerName.Content = listing.name;
		ServerDescription.Content = (string.IsNullOrEmpty(listing.extras) ? "No description" : listing.extras);
		PlayerCount.Content = listing.players + "/" + listing.capacity + "\nONLINE";
		new Thread((ThreadStart)delegate
		{
			if (listing.password == "")
			{
				base.Dispatcher.Invoke(delegate
				{
					ServerLocked.Visibility = Visibility.Hidden;
					ConnectBtnlbl.Content = "Connect";
				});
			}
			else
			{
				base.Dispatcher.Invoke(delegate
				{
					ServerLocked.Visibility = Visibility.Visible;
					ConnectBtnlbl.Content = "Connect";
				});
			}
			base.Dispatcher.Invoke(delegate
			{
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.UriSource = new Uri("https://flagcdn.com/w320/" + listing.region + ".png", UriKind.Absolute);
				bitmapImage.EndInit();
				FlagIcon.Source = bitmapImage;
			});
		}).Start();
	}

	public void UpdateUIVisibility(string search, ItemCollection items)
	{
		if (search == "" || search == "Search..." || listing.name.ToLower().Contains(search.ToLower()))
		{
			if (!items.Contains(this))
			{
				items.Add(this);
			}
			base.Visibility = Visibility.Visible;
		}
		else
		{
			if (items.Contains(this))
			{
				items.Remove(this);
			}
			base.Visibility = Visibility.Hidden;
		}
	}

	private void ConnectBtn_Click(object sender, RoutedEventArgs e)
	{
		if (listing.password == "")
		{
			if (Window.GetWindow(this) is Main main)
			{
				main.OpenConnectingBox(listing.name, listing.extras);
			}
			Main.Client_PlayOnline(listing.ip ?? "", listing.launchargs);
		}
		else if (Window.GetWindow(this) is Main main2)
		{
			main2.OpenPasswordBox(listing.name, listing.extras, listing.ip, listing.password, listing.launchargs);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "6.0.5.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/SkateWorld-2.0;component/ui/controls/serveritem.xaml", UriKind.Relative);
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
			((ServerItem)target).Loaded += Window_Loaded;
			break;
		case 2:
			FlagIcon = (Image)target;
			break;
		case 3:
			ServerName = (Label)target;
			break;
		case 4:
			PlayerCount = (Label)target;
			break;
		case 5:
			ServerDescription = (Label)target;
			break;
		case 6:
			ConnectBtn = (Button)target;
			ConnectBtn.Click += ConnectBtn_Click;
			break;
		case 7:
			ConnectBtnlbl = (Label)target;
			break;
		case 8:
			ServerLocked = (Image)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
