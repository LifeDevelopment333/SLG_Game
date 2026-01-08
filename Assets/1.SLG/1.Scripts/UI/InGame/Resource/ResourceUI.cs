using SLG.EnumTypes;
using System;
using TMPro;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    [Serializable]
    public struct UI
    {
        public TMP_Text Food;
        public TMP_Text Wood;
        public TMP_Text Rock;
        public TMP_Text Iron;
        public TMP_Text Gold;
    }
    [SerializeField] private UI ui;

    private void Start()
    {
        ResourceManager.Instance.OnResourceChanged += UpdateResourceUI;

        foreach(var resource in ResourceManager.Instance.Resources)
        {
            UpdateResourceUI(resource.Key, resource.Value);
        }
    }

    private void UpdateResourceUI(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.나무:
                ui.Wood.text = amount.ToString();
                break;
            case ResourceType.돌:
                ui.Rock.text = amount.ToString();
                break;
            case ResourceType.광석:
                ui.Iron.text = amount.ToString();
                break;
            case ResourceType.골드:
                ui.Gold.text = amount.ToString();
                break;
            case ResourceType.식량:
                ui.Food.text = amount.ToString();
                break;
        }
    }
}
