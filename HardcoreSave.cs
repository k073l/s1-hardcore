using MelonLoader;
using S1API.Internal.Abstraction;
using S1API.Saveables;

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
        Melon<Hardcore>.Logger.Msg("Game Saved!");
        Melon<Hardcore>.Logger.Msg($"Hardcore Mode is: {HardcoreModeData.HardcoreMode}");
    }

    protected override void OnLoaded()
    {
        Melon<Hardcore>.Logger.Msg("Game Loaded!");
        Melon<Hardcore>.Logger.Msg($"Hardcore Mode is: {HardcoreModeData.HardcoreMode}");
    }
}