using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace SkateWorld_2._0.UI;

public class Settings : Window, IComponentConnector
{
	internal Border Drag;

	internal Button Exit;

	internal Grid FeaturesGrid;

	internal CheckBox DX11;

	internal CheckBox VSync;

	internal CheckBox FullScreen;

	internal ComboBox AO;

	internal ComboBox AA;

	internal ComboBox ShaderQuality;

	internal Slider ResScale;

	internal CheckBox SkipIntro;

	internal CheckBox RemoveShirt;

	internal CheckBox Debug;

	internal CheckBox HideDebug;

	internal CheckBox DisableWipeouts;

	internal CheckBox DisableSpeedWobbles;

	internal ComboBox TOD;

	internal Slider Fov;

	internal Slider Trucks;

	internal Slider Hardness;

	internal CheckBox ModMenu;

	private bool _contentLoaded;

	public Settings()
	{
		InitializeComponent();
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
	}

	private void Drag_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		DragMove();
	}

	private void Exit_Click(object sender, RoutedEventArgs e)
	{
		Close();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "6.0.5.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/SkateWorld-2.0;component/ui/settings.xaml", UriKind.Relative);
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
			((Settings)target).Loaded += Window_Loaded;
			break;
		case 2:
			Drag = (Border)target;
			Drag.MouseLeftButtonDown += Drag_MouseLeftButtonDown;
			break;
		case 3:
			Exit = (Button)target;
			Exit.Click += Exit_Click;
			break;
		case 4:
			FeaturesGrid = (Grid)target;
			break;
		case 5:
			DX11 = (CheckBox)target;
			break;
		case 6:
			VSync = (CheckBox)target;
			break;
		case 7:
			FullScreen = (CheckBox)target;
			break;
		case 8:
			AO = (ComboBox)target;
			break;
		case 9:
			AA = (ComboBox)target;
			break;
		case 10:
			ShaderQuality = (ComboBox)target;
			break;
		case 11:
			ResScale = (Slider)target;
			break;
		case 12:
			SkipIntro = (CheckBox)target;
			break;
		case 13:
			RemoveShirt = (CheckBox)target;
			break;
		case 14:
			Debug = (CheckBox)target;
			break;
		case 15:
			HideDebug = (CheckBox)target;
			break;
		case 16:
			DisableWipeouts = (CheckBox)target;
			break;
		case 17:
			DisableSpeedWobbles = (CheckBox)target;
			break;
		case 18:
			TOD = (ComboBox)target;
			break;
		case 19:
			Fov = (Slider)target;
			break;
		case 20:
			Trucks = (Slider)target;
			break;
		case 21:
			Hardness = (Slider)target;
			break;
		case 22:
			ModMenu = (CheckBox)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
