using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SkateWorld_2._0.KeyAuth;
using SkateWorld_2._0.Properties;

namespace SkateWorld_2._0.Client;

public static class Game
{
	public static string GameExecutablePath = AppDomain.CurrentDomain.BaseDirectory + "\\Skate.crack.exe";

	public static List<GameInstance> gameInstances = new List<GameInstance>();


	//Shitty Debugging Method
	//Change application name to bypass
	private static string[] programs = new string[16]
	{
		"Procmon64", "Procmon32", "Procmon", "dnSpy", "cheatengine-x86_64", "ollydbg", "ida", "ida64", "radare2", "x64dbg",
		"ProcessHacker", "WireShark", "HTTPDebugger", "HTTPDebuggerUI", "HTTPDebuggerSvc", "Fiddler"
	};

	public static bool clientRunning => gameInstances.Select((GameInstance x) => !x.server).Count() > 0;

	public static bool serverRunning => gameInstances.Select((GameInstance x) => x.server).Count() > 0;

	public static string SKey2 { get; private set; } = "NTc2RTU=";


	private static void LaunchGameProcess(params string[] args)
	{
		LaunchGameProcess(string.Join(" ", args));
	}

	private static void LaunchGameProcess(string args)
	{
		if (args.Contains("-DelMar.LocalPlayerDebugName " + API.KeyAuthApp.user_data.username))
		{
			Process process = Process.Start(new ProcessStartInfo
			{
				FileName = GameExecutablePath,
				Arguments = args
			});
			gameInstances.Add(new GameInstance
			{
				process = process,
				server = args.Contains("-Server")
			});
		}
	}

	public static void CheckForPrograms()
	{
		new Thread((ThreadStart)delegate
		{
			while (true)
			{
				string[] array = programs;
				for (int i = 0; i < array.Length; i++)
				{
					if (Process.GetProcessesByName(array[i]).Length != 0)
					{
						foreach (GameInstance gameInstance in gameInstances)
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
				}
				Task.Delay(5000);
			}
		}).Start();
	}

	public static string ReturnSuper(string a, string opta, string optb)
	{
		return a.Replace("SKATE", opta).Replace("WORLD", optb);
	}

	public static void PlaySolo(GameSettings settings)
	{
		settings.BuildingItems = 5000.0;
		if (Settings.Default.ModMenu)
		{
			settings.DX11 = true;
		}
		LaunchGameProcess(settings.GenerateParams());
	}

	public static void PlayOnline(string IP, GameSettings settings, string customargs)
	{
		settings.DebugRender = false;
		settings.BuildingItems = 200.0;
		foreach (api.Data subscription in API.KeyAuthApp.user_data.subscriptions)
		{
			if (subscription.subscription == "SkateWorldSuperAdmin")
			{
				settings.DebugRender = true;
				settings.BuildingItems = 500.0;
			}
		}
		if (Settings.Default.ModMenu)
		{
			settings.DX11 = true;
		}
		LaunchGameProcess(settings.GenerateParams(), "-Client.ServerIp " + IP + " " + customargs);
	}

	public static void StartServer(string level)
	{
		LaunchGameProcess("-Server");
	}

	public static void Game_ResetCFG()
	{
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Scripts/Win32Game.cfg"))
		{
			File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "/Scripts/Win32Game.cfg", "-super layout.toc");
		}
	}
}
