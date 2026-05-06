using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 挂在角色头顶的 World Space 血条，始终朝向主相机
/// </summary>
public class WorldSpaceHealthBar : MonoBehaviour
{
    public Fighter fighter;         // 绑定的 Fighter 组件
    public Slider hpSlider;         // 血条 Slider
    public Vector3 offset = new Vector3(0, 1.5f, 0); // 相对角色的偏移（头顶高度）

    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;

        if (fighter != null)
        {
            fighter.OnHPChanged += OnHPChanged;
            hpSlider.value = 1f;
        }
    }

    void LateUpdate()
    {
        // 跟随角色位置
        transform.position = fighter.transform.position + offset;

        // 始终朝向相机（Billboard 效果）
        transform.LookAt(transform.position + cam.forward);
    }

    void OnHPChanged(int current, int max)
    {
        hpSlider.value = (float)current / max;
    }
}
