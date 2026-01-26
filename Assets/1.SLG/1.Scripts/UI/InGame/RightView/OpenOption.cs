using UnityEngine;
using UnityEngine.UI;

public class OpenOption : MonoBehaviour
{
    private Button optionButton;

    [SerializeField] private Option Option;

    private void Start()
    {
        optionButton = GetComponent<Button>();
        optionButton.onClick.AddListener(OnClickOpenOption);
    }

    public void OnClickOpenOption()
    {
        Option.gameObject.SetActive(true);
    }
}
