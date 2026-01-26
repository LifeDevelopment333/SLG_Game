using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    [SerializeField] private Button ExitButton;
    [SerializeField] private Button SaveButton;
    [SerializeField] private Button LoadButton;

    private void Start()
    {
        ExitButton.onClick.AddListener(OnClickExitButton);
    }

    public void OnClickExitButton()
    {
        gameObject.SetActive(false);
    }

    public void OnClickSaveButton()
    {
        SaveSystem.SaveGame();
    }

    public void OnClickLoadButton()
    {
        SaveSystem.LoadGame();
    }
}
