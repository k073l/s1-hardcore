using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if MONO
using ScheduleOne.Persistence;
using ScheduleOne.PlayerScripts;
using ScheduleOne.UI;
using TMPro;
#else
using Il2CppScheduleOne.Persistence;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI;
using Il2CppTMPro;
#endif

namespace Hardcore.Patches;

[HarmonyPatch(typeof(DeathScreen))]
public class DeathScreenPatch
{
    [HarmonyPatch(nameof(DeathScreen.Open))]
    [HarmonyPostfix]
    public static void SaveDeath(DeathScreen __instance)
    {
        if (!HardcoreSave.Instance.HardcoreModeData.HardcoreMode)
            return;

        Melon<Hardcore>.Logger.Msg("Player has died in Hardcore.");
        if (!string.IsNullOrEmpty(HardcoreSave.Instance.HardcoreModeData.Reason))
        {
            Melon<Hardcore>.Logger.Msg($"Death Reason: {HardcoreSave.Instance.HardcoreModeData.Reason}");
        }
        HardcoreSave.Instance.HardcoreModeData.Died = true;

        // Force a save
        SaveManager.Instance.Save();
    }

    public static void AddDeathUIInfo()
    {
        if (!HardcoreSave.Instance.HardcoreModeData.HardcoreMode)
            return;
        
        var container = DeathScreen.Instance.transform.Find("Container");

        var title = container.Find("Title");
        var respawnButton = container.Find("Respawn");
        var loadButton = container.Find("Load");

        EditButton(respawnButton);
        EditButton(loadButton);
    }

    public static void EditButton(Transform button)
    {
        var buttonComponent = button.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener((UnityAction)GhostMode);
        }

        var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = "Become a Ghost";
        }
    }

    public static void GhostMode()
    {
        DeathScreen.Instance.Close();
        PlayerCamera.Instance.LockMouse();
        PlayerCameraPatch.FreeCamPlayerInvisible();
    }
}