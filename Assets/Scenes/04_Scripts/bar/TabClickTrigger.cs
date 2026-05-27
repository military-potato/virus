using UnityEngine;

public class TabClickTrigger : MonoBehaviour
{
    public enum TabType { Blue = 0, Orange = 1, Green = 2 }

    [SerializeField] private TabGroupController controller;
    [SerializeField] private TabType tabType;

    private void OnMouseDown()
    {
        Debug.Log($"{gameObject.name} 클릭됨! 지정된 타입: {tabType}");

        if (controller == null) return;

        switch (tabType)
        {
            case TabType.Blue:
                controller.SelectBlueTab();
                break;
            case TabType.Orange:
                controller.SelectOrangeTab();
                break;
            case TabType.Green:
                controller.SelectGreenTab();
                break;
        }
    }
}