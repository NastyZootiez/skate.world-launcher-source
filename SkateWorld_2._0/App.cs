using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;

namespace SkateWorld_2._0;

public class App : Application
{
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "6.0.5.0")]
	public void InitializeComponent()
	{
		base.StartupUri = new Uri("UI/Login.xaml", UriKind.Relative);
	}

	[STAThread]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "6.0.5.0")]
	public static void Main()
	{
		App app = new App();
		app.InitializeComponent();
		app.Run();
	}
}
