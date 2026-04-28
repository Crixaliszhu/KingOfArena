using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneSetupEditor : EditorWindow
{
    [MenuItem("Tools/Setup Game Scenes")]
    public static void SetupScenes()
    {
        if (!EditorUtility.DisplayDialog("场景生成",
            "将创建 MainMenu 和 FreeMode 场景，并配置 Build Settings。继续？",
            "确定", "取消"))
            return;

        CreateMainMenuScene();
        CreateFreeModeScene();
        SetupBuildSettings();
        EditorUtility.DisplayDialog("完成", "MainMenu 和 FreeMode 场景已创建！\n请在各场景中手动调整 UI 细节。", "确定");
    }

    static void CreateMainMenuScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Canvas
        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.AddComponent<GraphicRaycaster>();

        // EventSystem
        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        // Background Image
        var bgGo = new GameObject("Background");
        bgGo.transform.SetParent(canvasGo.transform, false);
        var bgImage = bgGo.AddComponent<Image>();
        var bgRect = bgGo.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        // 尝试加载背景图
        var mainMenuSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/mainMenu.png");
        if (mainMenuSprite != null)
            bgImage.sprite = mainMenuSprite;

        // Title
        var titleGo = new GameObject("Title");
        titleGo.transform.SetParent(canvasGo.transform, false);
        var titleText = titleGo.AddComponent<TextMeshProUGUI>();
        titleText.text = "King Of Arena";
        titleText.fontSize = 72;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        var titleRect = titleGo.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 200);
        titleRect.sizeDelta = new Vector2(800, 100);

        // Mode Dropdown
        var dropdownGo = CreateTMPDropdown(canvasGo.transform, "ModeDropdown", new Vector2(0, 20), new Vector2(300, 50));

        // Start Button
        var startBtnGo = CreateButton(canvasGo.transform, "StartButton", "开始游戏", new Vector2(0, -60), new Vector2(300, 60));

        // Warning Text
        var warningGo = new GameObject("WarningText");
        warningGo.transform.SetParent(canvasGo.transform, false);
        var warningText = warningGo.AddComponent<TextMeshProUGUI>();
        warningText.text = "请选择模式";
        warningText.fontSize = 28;
        warningText.alignment = TextAlignmentOptions.Center;
        warningText.color = Color.red;
        var warningRect = warningGo.GetComponent<RectTransform>();
        warningRect.anchoredPosition = new Vector2(0, -130);
        warningRect.sizeDelta = new Vector2(400, 40);
        warningGo.SetActive(false);

        // MainMenuManager
        var managerGo = new GameObject("MainMenuManager");
        var manager = managerGo.AddComponent<MainMenuManager>();
        manager.modeDropdown = dropdownGo.GetComponent<TMP_Dropdown>();
        manager.warningText = warningText;

        // Wire button
        var startBtn = startBtnGo.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(startBtn.onClick, manager.OnStartGame);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
    }

    static void CreateFreeModeScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Ground
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(10, 1, 10);

        // PauseMenu UI
        CreatePauseMenuUI();

        // PauseMenuManager
        var pauseGo = new GameObject("PauseMenuManager");
        var pauseManager = pauseGo.AddComponent<PauseMenuManager>();

        // 找到 PauseMenuPanel 并绑定
        var pausePanel = GameObject.Find("PauseMenuPanel");
        if (pausePanel != null)
            pauseManager.pauseMenuPanel = pausePanel;

        // Wire buttons
        var resumeBtn = GameObject.Find("ResumeButton");
        if (resumeBtn != null)
        {
            var btn = resumeBtn.GetComponent<Button>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, pauseManager.ResumeGame);
        }
        var quitBtn = GameObject.Find("QuitButton");
        if (quitBtn != null)
        {
            var btn = quitBtn.GetComponent<Button>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, pauseManager.QuitToMainMenu);
        }

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/FreeMode.unity");
    }

    static void CreatePauseMenuUI()
    {
        // Canvas
        var canvasGo = new GameObject("PauseCanvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.AddComponent<GraphicRaycaster>();

        // Panel (半透明背景)
        var panelGo = new GameObject("PauseMenuPanel");
        panelGo.transform.SetParent(canvasGo.transform, false);
        var panelImage = panelGo.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);
        var panelRect = panelGo.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Pause Title
        var titleGo = new GameObject("PauseTitle");
        titleGo.transform.SetParent(panelGo.transform, false);
        var titleText = titleGo.AddComponent<TextMeshProUGUI>();
        titleText.text = "游戏暂停";
        titleText.fontSize = 48;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        var titleRect = titleGo.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 100);
        titleRect.sizeDelta = new Vector2(400, 60);

        // Resume Button
        CreateButton(panelGo.transform, "ResumeButton", "继续游戏", new Vector2(0, 0), new Vector2(250, 50));

        // Quit Button
        CreateButton(panelGo.transform, "QuitButton", "退出游戏", new Vector2(0, -70), new Vector2(250, 50));

        panelGo.SetActive(false);
    }

    static GameObject CreateButton(Transform parent, string name, string label, Vector2 pos, Vector2 size)
    {
        var btnGo = new GameObject(name);
        btnGo.transform.SetParent(parent, false);
        var btnImage = btnGo.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        var btn = btnGo.AddComponent<Button>();
        var btnRect = btnGo.GetComponent<RectTransform>();
        btnRect.anchoredPosition = pos;
        btnRect.sizeDelta = size;

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(btnGo.transform, false);
        var text = textGo.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 24;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        var textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return btnGo;
    }

    static GameObject CreateTMPDropdown(Transform parent, string name, Vector2 pos, Vector2 size)
    {
        // 使用 TMP_DefaultControls 创建标准 Dropdown
        var resources = new TMP_DefaultControls.Resources();
        var dropdownGo = TMP_DefaultControls.CreateDropdown(resources);
        dropdownGo.name = name;
        dropdownGo.transform.SetParent(parent, false);
        var rect = dropdownGo.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;
        return dropdownGo;
    }

    static void SetupBuildSettings()
    {
        var scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/FreeMode.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/SampleScene.unity", true),
        };
        EditorBuildSettings.scenes = scenes;
    }
}
