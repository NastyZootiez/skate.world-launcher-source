using System.Text;
using SkateWorld_2._0.KeyAuth;

namespace SkateWorld_2._0.Client;

public struct GameSettings
{
	public string PlayerName;

	public bool DX11;

	public bool RemoveClothing;

	public bool DebugRender;

	public double TruckTightness;

	public double WheelHardness;

	public double Fov;

	public bool FullScreen;

	public bool SkipCutscene;

	public string AmbientOcclusion;

	public string AntiAliasing;

	public string ShaderQuality;

	public bool HideDebugInfo;

	public double TOD;

	public double ResScale;

	public bool DisableWipeOuts;

	public bool DisableSpeedWobble;

	public bool Vsync;

	public double BuildingItems;

	public override string ToString()
	{
		return GenerateParams();
	}

	public string GenerateParams()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("-DelMar.LocalPlayerDebugName " + API.KeyAuthApp.user_data.username + " -DelMarOnline.Enable false -Online.ClientIsPresenceEnabled false ");
		if (DX11)
		{
			stringBuilder.Append("-Render.ForceDx11 true ");
		}
		if (RemoveClothing)
		{
			stringBuilder.Append("-ClothSystem.ClientClothWorldThreadCount 0 ");
		}
		if (DebugRender)
		{
			stringBuilder.Append("-DebugRender true ");
		}
		if (FullScreen)
		{
			stringBuilder.Append("-ProfileOptions.ForceDefaultFullscreenEnabled true ");
		}
		if (SkipCutscene)
		{
			stringBuilder.Append("-DelMarGame.AllowBootPrompts false ");
		}
		if (HideDebugInfo)
		{
			stringBuilder.Append("-PerfOverlay.DrawFps False -GestureVis.OverlayXOffset 9999 -DelMar.SkaterGameplayDebugEnabled False -BundleManager.DrawStats 0 ");
		}
		if (DisableWipeOuts)
		{
			stringBuilder.Append("-DelMar.kWipeoutDisableAllWipeouts true ");
		}
		if (TOD == 90.0)
		{
			stringBuilder.Append("-WorldRender.PhysicalSkyPrecisionSun 0 -WorldRender.OutdoorLightEnable false -WorldRender.SkyLightingEnable false -PostProcess.UserBrightnessMulScale 0.9 -VisualEnvironment.SunRotationY 90 ");
		}
		else
		{
			stringBuilder.Append("-VisualEnvironment.SunRotationY " + TOD + " ");
		}
		if (DisableSpeedWobble)
		{
			stringBuilder.Append("-EmitterSystem.TurbulenceMasxDistance 1000000000 ");
		}
		if (Vsync)
		{
			stringBuilder.Append("-Render.VSyncEnable true -Render.ForceVSyncEnable true ");
		}
		stringBuilder.Append("-DelMarUI.EnableWatermark false ");
		stringBuilder.Append("-DelMarExperimentalSettingsEnabled true -DelMarEnableExperimentalFeatures true ");
		stringBuilder.Append("-ItemManager.ForceOwnAll true -Architect.ShowAllCategories true -Player.EnableAllUnlocks true -ItemManager.MaxOfflineInstances " + BuildingItems + " ");
		stringBuilder.Append("-FBCorePhysics.TruckTightness " + TruckTightness + " ");
		stringBuilder.Append("-FBCorePhysics.WheelHardness " + WheelHardness + " ");
		stringBuilder.Append("-Render.ResolutionScale " + ResScale + " ");
		stringBuilder.Append("-Render.ForceFov " + Fov + " ");
		stringBuilder.Append("-PostProcess.DynamicAOMethod " + AmbientOcclusion + " ");
		stringBuilder.Append("-WorldRender.PostProcessAntiAliasingMode " + AntiAliasing + " ");
		stringBuilder.Append("-ShaderSystem.ShaderQualityLevel " + ShaderQuality + " ");
		return stringBuilder.ToString();
	}
}
