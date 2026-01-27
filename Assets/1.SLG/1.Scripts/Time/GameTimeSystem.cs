using UnityEngine;

public class GameTimeSystem : MonoBehaviour
{
    private static GameTimeSystem instance;
    public static GameTimeSystem Instance => instance;

    [Header("시간설정")]
    [SerializeField] private float timeScale = 1f;
    [SerializeField] private bool isPause = false;

    public float DeltaTime => isPause ? 0f : Time.deltaTime * timeScale;
    public float TimeScale => timeScale;
    public bool IsPause => isPause;
         
    private void Awake()
    {
        instance = this;
    }

    public void SetTimeScale(float timeScale)
    {
        this.timeScale = timeScale;
    }

    public void Pause()
    {
        isPause = true;
    }

    public void Resume()
    {
        isPause = false;
    }
}
