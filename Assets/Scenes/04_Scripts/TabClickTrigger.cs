using UnityEngine;

public class TabClickTrigger : MonoBehaviour
{
    [SerializeField] private TabGroupController controller;
    [SerializeField] private bool isBlueTab;

    private void OnMouseDown()
    {
        if (controller == null) return;

        if (isBlueTab)
        {
            controller.SelectBlueTab();
        }
        else
        {
            controller.SelectOrangeTab();
        }
    }
}