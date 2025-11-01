using HarmonyLib;

#if MONO
using ScheduleOne.PlayerScripts;
using ScheduleOne.UI;
#else
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI;
#endif

namespace Hardcore.Patches;

[HarmonyPatch(typeof(PlayerCamera))]
public class PlayerCameraPatch
{
    [HarmonyPatch(nameof(PlayerCamera.Exit))]
    [HarmonyPostfix]
    public static void FreecamJailPost()
    {
        if (!HardcoreSave.Instance.HardcoreModeData.HardcoreMode || !HardcoreSave.Instance.HardcoreModeData.Died)
            return;
        // Re-enable freecam if player is dead in hardcore mode
        FreeCamPlayerInvisible();
        
        // Hacky, but it works
        if (PauseMenu.Instance.IsPaused)
            PauseMenu.Instance.Resume();
        else
            PauseMenu.Instance.Pause();
    }
    
    public static void FreeCamPlayerInvisible()
    {
        PlayerCamera.Instance.FreeCamEnabled = true;
        HUD.Instance.canvas.enabled = false;
        PlayerMovement.Instance.CanMove = false;
        Player.Local.SetVisibleToLocalPlayer(false);
        
        PlayerCamera.Instance.OverrideTransform(PlayerCamera.Instance.transform.position, PlayerCamera.Instance.transform.rotation, 0f);
        PlayerCamera.Instance.AddActiveUIElement(PlayerCamera.Instance.name);
    }
}