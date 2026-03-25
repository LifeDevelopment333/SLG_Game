using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button NewGame;
    [SerializeField] private Button ContinuedGame;
    [SerializeField] private Button GameClosed;

    private void Start()
    {
        NewGame?.onClick.AddListener(OnClickNewGame);
        ContinuedGame?.onClick.AddListener(OnClickContinuedGame);
        GameClosed?.onClick.AddListener(OnClickGameClosed);
    }

    private void OnClickNewGame()
    {
        gameObject.SetActive(false);
    }

    private void OnClickContinuedGame()
    {
        gameObject.SetActive(false);
    }

    private void OnClickGameClosed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
