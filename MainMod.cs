using System.Collections;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using S1API.Saveables;
using Hardcore.Patches;
using Hardcore.Helpers;

#if MONO
using ScheduleOne.Persistence;
#else
using Il2CppScheduleOne.Persistence;
#endif


[assembly: MelonInfo(
    typeof(Hardcore.Hardcore),
    Hardcore.BuildInfo.Name,
    Hardcore.BuildInfo.Version,
    Hardcore.BuildInfo.Author
)]
[assembly: MelonColor(1, 255, 0, 0)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace Hardcore;

public static class BuildInfo
{
    public const string Name = "Hardcore";
    public const string Description = "Adds a Hardcore mode to the game";
    public const string Author = "k073l";
    public const string Version = "1.0.5";
}

public class Hardcore : MelonMod
{
    private static MelonLogger.Instance _logger;

    private static Texture2D _heartTexture2D;

    internal static bool ForceHardcoreMode = false;

    private static MelonPreferences_Category _category;
    private static MelonPreferences_Entry<bool> _showHeartHUD;
    private static MelonPreferences_Entry<int> _heartHUDsize;
    private static MelonPreferences_Entry<int> _heartHUDXOffset;
    private static MelonPreferences_Entry<int> _heartHUDYOffset;


    public override void OnInitializeMelon()
    {
        _logger = LoggerInstance;
        _logger.Msg("Hardcore initialized");

        _category = MelonPreferences.CreateCategory("Hardcore");
        _showHeartHUD = _category.CreateEntry("ShowHeartHUD", true, "Show Heart HUD",
            "Show a heart icon in the HUD when in Hardcore mode");
        _heartHUDsize =
            _category.CreateEntry("HeartHUDSize", 32, "Heart HUD Size", "Size of the heart icon in the HUD");
        _heartHUDXOffset = _category.CreateEntry("HeartHUDXOffset", 5, "Heart HUD Position (horizontal)",
            "Horizontal offset from the right side of the screen (in px)");
        _heartHUDYOffset = _category.CreateEntry("HeartHUDYOffset", 5, "Heart HUD Position (vertical)",
            "Vertical offset from the top of the screen (in px)");

        LoadEmbeddedImage();
        ModSaveableRegistry.Register(HardcoreSave.Instance);
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        _logger.Debug($"Scene loaded: {sceneName}");
        switch (sceneName)
        {
            case "Menu":
                HardcoreSave.Instance.HardcoreModeData = new HardcoreData(); // reset on main menu
                break;
            case "Main":
                _logger.Debug("Main scene loaded, waiting for player");
                MelonCoroutines.Start(Utils.WaitForPlayer(CheckHardcore()));
                break;
        }
    }

    private void LoadEmbeddedImage()
    {
        var assembly = Assembly.GetExecutingAssembly();

        const string resourceName = "Hardcore.assets.heart.png";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            _logger.Error("Embedded image not found!");
            return;
        }

        var data = new byte[stream.Length];
        stream.Read(data, 0, data.Length);

        _heartTexture2D = new Texture2D(2, 2);
        if (!_heartTexture2D.LoadImage(data))
        {
            _logger.Error("Failed to load image data into Texture2D.");
        }
    }

    public override void OnGUI()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Main") return;
        if (_heartTexture2D == null) return;
        if (!HardcoreSave.Instance.HardcoreModeData.HardcoreMode) return;
        if (!_showHeartHUD.Value) return;

        var width = (float)_heartHUDsize.Value; // Square size

        var x = Screen.width - width - (float)_heartHUDXOffset.Value;
        var y = (float)_heartHUDYOffset.Value;

        GUI.DrawTexture(new Rect(x, y, width, width), _heartTexture2D);
    }

    private static IEnumerator CheckHardcore()
    {
        yield return new WaitForSeconds(1f);
        _logger.Msg(
            $"Hardcore mode is {(HardcoreSave.Instance.HardcoreModeData.HardcoreMode ? "enabled" : "disabled")}");
        var saveLocation = LoadManager.Instance.ActiveSaveInfo.SavePath;
        _logger.Msg(
            $"If you NEED to edit the Hardcore status, modify: {Path.Combine(saveLocation, "Modded", "Saveables", "HardcoreSave", "hardcore_mode.json")}");

        DeathScreenPatch.AddDeathUIInfo();

        if (HardcoreSave.Instance.HardcoreModeData.Died)
        {
            PlayerCameraPatch.FreeCamPlayerInvisible();
        }
    }
}