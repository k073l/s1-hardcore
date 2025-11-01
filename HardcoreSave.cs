using MelonLoader;
using S1API.Internal.Abstraction;
using S1API.Saveables;

#if MONO
using ScheduleOne.Persistence;
#else
using Il2CppScheduleOne.Persistence;
#endif

namespace Hardcore;

public class HardcoreData
{
    public bool HardcoreMode;
    public string Version = "1.0.0";
    public bool Died;
    public string Reason = "";
}

public class HardcoreSave : Saveable
{
    public static HardcoreSave Instance { get; } = new HardcoreSave();
    
    [SaveableField("hardcore_mode")]
    public HardcoreData HardcoreModeData = new HardcoreData();
    protected override void OnSaved()
    {
        // Force Hardcore on fresh save if the option is enabled
        if (Hardcore.ForceHardcoreMode)
            ApplyForceHardcore();
        Melon<Hardcore>.Logger.Msg("Game Saved!");
        Melon<Hardcore>.Logger.Msg($"Hardcore Mode is: {HardcoreModeData.HardcoreMode}");
        Instance.HardcoreModeData = HardcoreModeData;
    }

    protected override void OnLoaded()
    {
        Melon<Hardcore>.Logger.Msg("Game Loaded!");
        Melon<Hardcore>.Logger.Msg($"Hardcore Mode is: {HardcoreModeData.HardcoreMode}");
        Instance.HardcoreModeData = HardcoreModeData;
    }

    private void ApplyForceHardcore()
    {
        Hardcore.ForceHardcoreMode = false;
        HardcoreModeData.HardcoreMode = true;
        SaveManager.Instance.Save();
    }
}