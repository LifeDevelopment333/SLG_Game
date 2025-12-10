using System;
using UnityEngine;
using UnityEngine.UI;

public class BuildUI : MonoBehaviour
{
    [Serializable]
    public struct UI
    {
        public Button Build;
        public Button Cancle;
        public Button Remove;
        public Button Upgrade;
    }
    [SerializeField] private UI ui;

    private void Awake()
    {
        ButtonBind();
    }

    private void ButtonBind()
    {
        ui.Build.onClick.AddListener(OnclickBuildButton);
        ui.Cancle.onClick.AddListener(OnClickCancleButton);
        ui.Remove.onClick.AddListener(OnClickRemoveButton);
        ui.Upgrade.onClick.AddListener(OnClickUpgradeButton);
    }

    private void OnclickBuildButton()
    {

    }

    private void OnClickCancleButton()
    {

    }

    private void OnClickRemoveButton()
    {

    }

    private void OnClickUpgradeButton()
    {

    }
}
