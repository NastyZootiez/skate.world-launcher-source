using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SkateWorld_2._0.Properties;

[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.2.0.0")]
internal sealed class Settings : ApplicationSettingsBase
{
	private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

	public static Settings Default => defaultInstance;

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string Username
	{
		get
		{
			return (string)this["Username"];
		}
		set
		{
			this["Username"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string Password
	{
		get
		{
			return (string)this["Password"];
		}
		set
		{
			this["Password"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool SaveDetails
	{
		get
		{
			return (bool)this["SaveDetails"];
		}
		set
		{
			this["SaveDetails"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool DX11
	{
		get
		{
			return (bool)this["DX11"];
		}
		set
		{
			this["DX11"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool DebugRender
	{
		get
		{
			return (bool)this["DebugRender"];
		}
		set
		{
			this["DebugRender"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool RemoveClothes
	{
		get
		{
			return (bool)this["RemoveClothes"];
		}
		set
		{
			this["RemoveClothes"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool Fullscreen
	{
		get
		{
			return (bool)this["Fullscreen"];
		}
		set
		{
			this["Fullscreen"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool SkipCutscene
	{
		get
		{
			return (bool)this["SkipCutscene"];
		}
		set
		{
			this["SkipCutscene"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("HBAO")]
	public string AO
	{
		get
		{
			return (string)this["AO"];
		}
		set
		{
			this["AO"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("Smaa1x")]
	public string AA
	{
		get
		{
			return (string)this["AA"];
		}
		set
		{
			this["AA"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("Ultra")]
	public string ShaderQuality
	{
		get
		{
			return (string)this["ShaderQuality"];
		}
		set
		{
			this["ShaderQuality"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool HideDebugInfo
	{
		get
		{
			return (bool)this["HideDebugInfo"];
		}
		set
		{
			this["HideDebugInfo"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("70")]
	public double Fov
	{
		get
		{
			return (double)this["Fov"];
		}
		set
		{
			this["Fov"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("40")]
	public double TOD
	{
		get
		{
			return (double)this["TOD"];
		}
		set
		{
			this["TOD"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool DisableWipeouts
	{
		get
		{
			return (bool)this["DisableWipeouts"];
		}
		set
		{
			this["DisableWipeouts"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("1")]
	public double ResScale
	{
		get
		{
			return (double)this["ResScale"];
		}
		set
		{
			this["ResScale"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("0")]
	public double WheelHardness
	{
		get
		{
			return (double)this["WheelHardness"];
		}
		set
		{
			this["WheelHardness"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("0")]
	public double TruckTightness
	{
		get
		{
			return (double)this["TruckTightness"];
		}
		set
		{
			this["TruckTightness"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool SpeedWobble
	{
		get
		{
			return (bool)this["SpeedWobble"];
		}
		set
		{
			this["SpeedWobble"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool Vsync
	{
		get
		{
			return (bool)this["Vsync"];
		}
		set
		{
			this["Vsync"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool ModMenu
	{
		get
		{
			return (bool)this["ModMenu"];
		}
		set
		{
			this["ModMenu"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("0")]
	public int AAIndex
	{
		get
		{
			return (int)this["AAIndex"];
		}
		set
		{
			this["AAIndex"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("0")]
	public int AOIndex
	{
		get
		{
			return (int)this["AOIndex"];
		}
		set
		{
			this["AOIndex"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("0")]
	public int ShaderQualityIndex
	{
		get
		{
			return (int)this["ShaderQualityIndex"];
		}
		set
		{
			this["ShaderQualityIndex"] = value;
		}
	}
}
