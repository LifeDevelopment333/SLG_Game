using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    private static UI_Manager instance;
    public static UI_Manager Instance => instance;

    [SerializeField] private Button createModeButton;
    [SerializeField] private Button displayModeButton;

    private bool isCreateMode = false;
    private bool isDisplayMode = false;

    public bool IsCreateMode => isCreateMode;
    public bool IsDisplayMode => isDisplayMode;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if(createModeButton != null)
            createModeButton.onClick.AddListener(OnClickCreateModeButton);

        if(displayModeButton != null)
            displayModeButton.onClick.AddListener(OnClickDisplayModeButton);
    }

    private void Reset()
    {
        isCreateMode = false;
        isDisplayMode = true;
    }

    private void OnClickCreateModeButton()
    {
        isCreateMode = true;
        isDisplayMode = false;
    }

    private void OnClickDisplayModeButton()
    {
        isDisplayMode = true;
        isCreateMode = false;
    }
}
