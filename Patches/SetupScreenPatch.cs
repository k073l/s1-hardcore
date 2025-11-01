using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

#if MONO
using ScheduleOne.UI.MainMenu;
using TMPro;
#else
using Il2CppScheduleOne.UI.MainMenu;
using Il2CppTMPro;
#endif

namespace Hardcore.Patches;

[HarmonyPatch(typeof(SetupScreen))]
public class SetupScreenPatch
{
    public static Toggle hardcoreToggle;
    public static TextMeshProUGUI hardcoreToggleLabel;
    public static GameObject hardcoreToggleContainer;

    [HarmonyPatch(nameof(SetupScreen.Start))]
    [HarmonyPostfix]
    public static void AddHardcoreToggle(SetupScreen __instance)
    {
        var parent = __instance.transform;
        var skipIntro = parent.Find("SkipIntro");
        var confirm = parent.Find("Confirm");

        if (skipIntro == null)
        {
            Melon<Hardcore>.Logger.Error("SkipIntro not found!");
            return;
        }

        // Get reference objects for styling
        var skipIntroToggle = skipIntro.GetComponent<Toggle>();
        var skipIntroLabel = skipIntro.GetComponentInChildren<TextMeshProUGUI>();

        hardcoreToggleContainer = new GameObject("HardcoreMode");
        hardcoreToggleContainer.transform.SetParent(parent);
        hardcoreToggleContainer.transform.localScale = Vector3.one;

        var containerRect = hardcoreToggleContainer.AddComponent<RectTransform>();
        var skipRect = skipIntro.GetComponent<RectTransform>();

        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = skipRect.sizeDelta;

        containerRect.anchoredPosition = new Vector2(0,
            skipRect.anchoredPosition.y - skipRect.sizeDelta.y - 30);


        var confirmRect = confirm.GetComponent<RectTransform>();
        confirmRect.anchoredPosition = new Vector2(confirmRect.anchoredPosition.x,
            confirmRect.anchoredPosition.y - 35);

        var toggleObj = new GameObject("Toggle");
        toggleObj.transform.SetParent(hardcoreToggleContainer.transform);

        var toggleRect = toggleObj.AddComponent<RectTransform>();
        // Center
        toggleRect.anchorMin = new Vector2(0.5f, 0.5f);
        toggleRect.anchorMax = new Vector2(0.5f, 0.5f);
        toggleRect.pivot = new Vector2(0.5f, 0.5f);
        toggleRect.anchoredPosition = new Vector2(-48, 0);
        toggleRect.sizeDelta = new Vector2(25, 25);

        // Add background image
        var backgroundImage = toggleObj.AddComponent<Image>();
        hardcoreToggle = toggleObj.AddComponent<Toggle>();
        hardcoreToggle.targetGraphic = backgroundImage;
        hardcoreToggle.isOn = false;

        // Create checkmark
        var checkmarkObj = new GameObject("Checkmark");
        checkmarkObj.transform.SetParent(toggleObj.transform);

        var checkmarkRect = checkmarkObj.AddComponent<RectTransform>();
        var checkmarkImage = checkmarkObj.AddComponent<Image>();
        hardcoreToggle.graphic = checkmarkImage;

        // Copy toggle properties
        hardcoreToggle.transition = skipIntroToggle.transition;
        hardcoreToggle.colors = skipIntroToggle.colors;
        hardcoreToggle.spriteState = skipIntroToggle.spriteState;
        hardcoreToggle.animationTriggers = skipIntroToggle.animationTriggers;

        // Copy background styling
        Image sourceBgImage = null;

        // Try multiple ways to get the Image component (IL2CPP compatibility)
        if (skipIntroToggle.targetGraphic is Image directCastBg)
        {
            sourceBgImage = directCastBg;
        }
        else if (skipIntroToggle.targetGraphic != null)
        {
            sourceBgImage = skipIntroToggle.targetGraphic.GetComponent<Image>();
        }

        if (sourceBgImage != null)
        {
            backgroundImage.sprite = sourceBgImage.sprite;
            backgroundImage.type = sourceBgImage.type;
            backgroundImage.color = sourceBgImage.color;
            backgroundImage.material = sourceBgImage.material;
            backgroundImage.raycastTarget = sourceBgImage.raycastTarget;
        }
        else
        {
            // Fallback configuration for background
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.color = Color.gray;
            backgroundImage.raycastTarget = true;

            if (skipIntroToggle.targetGraphic != null)
            {
                backgroundImage.material = skipIntroToggle.targetGraphic.material;
                backgroundImage.raycastTarget = skipIntroToggle.targetGraphic.raycastTarget;
            }

            Melon<Hardcore>.Logger.Warning("Using fallback background configuration");
        }

        // Copy checkmark styling
        Image sourceImage = null;

        // Try multiple ways to get the Image component (IL2CPP compatibility)
        if (skipIntroToggle.graphic is Image directCast)
        {
            sourceImage = directCast;
        }
        else if (skipIntroToggle.graphic != null)
        {
            sourceImage = skipIntroToggle.graphic.GetComponent<Image>();
        }

        if (sourceImage != null)
        {
            // Copy from source Image
            checkmarkImage.sprite = sourceImage.sprite;
            checkmarkImage.type = sourceImage.type;
            checkmarkImage.color = new Color(238f / 255f, 75f / 255f, 43f / 255f);
            checkmarkImage.material = sourceImage.material;
            checkmarkImage.raycastTarget = sourceImage.raycastTarget;

            var originalCheckRect = sourceImage.GetComponent<RectTransform>();
            if (originalCheckRect != null)
            {
                checkmarkRect.anchorMin = originalCheckRect.anchorMin;
                checkmarkRect.anchorMax = originalCheckRect.anchorMax;
                checkmarkRect.pivot = originalCheckRect.pivot;
                checkmarkRect.anchoredPosition = originalCheckRect.anchoredPosition;
                checkmarkRect.sizeDelta = originalCheckRect.sizeDelta;
                checkmarkRect.offsetMin = originalCheckRect.offsetMin;
                checkmarkRect.offsetMax = originalCheckRect.offsetMax;
            }
        }
        else
        {
            // Fallback configuration
            checkmarkImage.color = new Color(238f / 255f, 75f / 255f, 43f / 255f);
            checkmarkImage.type = Image.Type.Simple;

            if (skipIntroToggle.graphic != null)
            {
                checkmarkImage.material = skipIntroToggle.graphic.material;
                checkmarkImage.raycastTarget = skipIntroToggle.graphic.raycastTarget;
            }

            // Try to find a suitable checkmark sprite
            var allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
            var checkmarkSprite = allSprites.FirstOrDefault(s =>
                s.name.ToLower().Contains("check") ||
                s.name.ToLower().Contains("tick") ||
                s.name.ToLower().Contains("mark"));

            if (checkmarkSprite != null)
            {
                checkmarkImage.sprite = checkmarkSprite;
            }

            Melon<Hardcore>.Logger.Warning("Using fallback checkmark configuration");
        }

        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(hardcoreToggleContainer.transform);

        var labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.5f, 0.5f);
        labelRect.anchorMax = new Vector2(0.5f, 0.5f);
        labelRect.pivot = new Vector2(0, 0.5f);
        labelRect.anchoredPosition = new Vector2(-30, 0);
        labelRect.sizeDelta = new Vector2(200, 20);

        hardcoreToggleLabel = labelObj.AddComponent<TextMeshProUGUI>();
        hardcoreToggleLabel.text = "Hardcore Mode";

        hardcoreToggleLabel.font = skipIntroLabel.font;
        hardcoreToggleLabel.fontSize = skipIntroLabel.fontSize * 1.25f;
        hardcoreToggleLabel.color = new Color(220f / 255f, 20f / 255f, 60f / 255f);
        hardcoreToggleLabel.alignment = skipIntroLabel.alignment;
        hardcoreToggleLabel.fontStyle = skipIntroLabel.fontStyle;
        hardcoreToggleLabel.lineSpacing = skipIntroLabel.lineSpacing;

        hardcoreToggle.interactable = true;

        // Resize background to fit new toggle
        var background = parent.Find("Background");
        var backgroundRect = background.GetComponent<RectTransform>();
        var currentSize = backgroundRect.sizeDelta;
        backgroundRect.sizeDelta = new Vector2(currentSize.x, currentSize.y + 65);

        // Debug: log toggle changes, commented out cause I don't want to deal with it in Il2Cpp
        // hardcoreToggle.onValueChanged.AddListener((bool isOn) =>
        // {
        //     MelonLogger.Msg($"Hardcore Mode toggled: {isOn}");
        // });

        Melon<Hardcore>.Logger.Msg("Hardcore toggle created successfully!");
    }

    [HarmonyPatch(nameof(SetupScreen.StartGame))]
    [HarmonyPostfix]
    public static void ApplyHardcoreSetting(SetupScreen __instance)
    {
        MelonLogger.Msg($"Applying Hardcode: {hardcoreToggle.isOn}");
        Hardcore.ForceHardcoreMode = true;
        HardcoreSave.Instance.HardcoreModeData.HardcoreMode = hardcoreToggle.isOn;
    }
}