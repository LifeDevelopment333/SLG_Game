using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button BuildMenuButton;
    [SerializeField] private GameObject BuildMenuPanel;

    private void Start()
    {
        BuildMenuButton.onClick.AddListener(OpenBuildMenu);
    }

    private void OpenBuildMenu()
    {
        BuildMenuPanel.SetActive(true);
        BuildMenuButton.gameObject.SetActive(false);
    }
}
