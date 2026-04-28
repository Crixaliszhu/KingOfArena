using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public TMP_Dropdown modeDropdown;
    public TMP_Text warningText;

    private int selectedMode = -1; // -1 = 未选择

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 初始化下拉菜单
        modeDropdown.ClearOptions();
        modeDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "-- 请选择模式 --",
            "自由模式",
            "竞技模式"
        });
        modeDropdown.value = 0;
        modeDropdown.onValueChanged.AddListener(OnModeChanged);

        if (warningText) warningText.gameObject.SetActive(false);
    }

    void OnModeChanged(int index)
    {
        selectedMode = index; // 0=占位, 1=自由, 2=竞技
        if (warningText) warningText.gameObject.SetActive(false);
    }

    public void OnStartGame()
    {
        if (selectedMode <= 0)
        {
            if (warningText)
            {
                warningText.text = "请选择模式";
                warningText.gameObject.SetActive(true);
            }
            return;
        }

        switch (selectedMode)
        {
            case 1:
                SceneManager.LoadScene("FreeMode");
                break;
            case 2:
                SceneManager.LoadScene("SampleScene");
                break;
        }
    }
}
