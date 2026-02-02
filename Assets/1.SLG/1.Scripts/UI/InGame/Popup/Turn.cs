using UnityEngine;

public class Turn : MonoBehaviour
{
    private float size = 1;

    private RectTransform rect;
    private Camera mainCam;

    private Vector3 buildingPosition;

    /// <summary>
    /// UI 설정
    /// </summary>
    public void Set(int _size)
    {
        size = _size * (0.3f / 7f);

        gameObject.SetActive(true);

        if(mainCam == null)
        {
            mainCam = Camera.main;
        }

        if(rect == null)
        {
            rect = GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// UI 위치 업데이트
    /// </summary>
    public void UpdatePosition(Vector3 _position)
    {
        if(buildingPosition == _position)
            return;

        buildingPosition = _position;
        Vector3 screenPos = mainCam.WorldToScreenPoint(buildingPosition);
        rect.position = screenPos;
    }

    /// <summary>
    /// UI 초기화
    /// </summary>
    public void Clear()
    {
        gameObject.SetActive(false);
    }
}
