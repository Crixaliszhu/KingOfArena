using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class AddPauseToSampleScene : EditorWindow
{
    [MenuItem("Tools/Add Pause Menu to Current Scene")]
    public static void AddPauseMenu()
    {
        // 检查是否已存在
        if (GameObject.Find("PauseMenuManager") != null)
        {
            EditorUtility.DisplayDialog("提示", "当前场景已有 PauseMenuManager", "确定");
            return;
        }

        // 查找或创建 PauseCanvas
        var canvasGo = new GameObject("PauseCanvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.AddComponent<GraphicRaycaster>();

        // Panel
        var panelGo = new GameObject("PauseMenuPanel");
        panelGo.transform.SetParent(canvasGo.transform, false);
        var panelImage = panelGo.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);
        var panelRect = panelGo.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Title
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
        var resumeBtnGo = CreateBtn(panelGo.transform, "ResumeButton", "继续游戏", new Vector2(0, 0), new Vector2(250, 50));

        // Quit Button
        var quitBtnGo = CreateBtn(panelGo.transform, "QuitButton", "退出游戏", new Vector2(0, -70), new Vector2(250, 50));

        panelGo.SetActive(false);

        // PauseMenuManager
        var managerGo = new GameObject("PauseMenuManager");
        var manager = managerGo.AddComponent<PauseMenuManager>();
        manager.pauseMenuPanel = panelGo;

        // Wire buttons
        var resumeBtn = resumeBtnGo.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(resumeBtn.onClick, manager.ResumeGame);

        var quitBtn = quitBtnGo.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(quitBtn.onClick, manager.QuitToMainMenu);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.DisplayDialog("完成", "暂停菜单已添加到当前场景！请保存场景。", "确定");
    }

    static GameObject CreateBtn(Transform parent, string name, string label, Vector2 pos, Vector2 size)
    {
        var btnGo = new GameObject(name);
        btnGo.transform.SetParent(parent, false);
        var btnImage = btnGo.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        btnGo.AddComponent<Button>();
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
}
