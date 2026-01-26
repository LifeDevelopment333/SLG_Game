using System.Collections;
using UnityEngine;

public class TimeNeedle : MonoBehaviour
{
    [SerializeField] private RectTransform needle;

    [SerializeField] private float dayAngle = 50;
    [SerializeField] private float nightAngle = -50;

    private void Start()
    {
        SimulationSystem.Instance.OnDayStarted += SetDay;
        SimulationSystem.Instance.OnNightStarted += SetNight;
    }

    private void SetDay()
    {
        StartCoroutine(MoveNeedle(nightAngle, dayAngle));
    }

    private void SetNight()
    {
        StartCoroutine(MoveNeedle(dayAngle, nightAngle));
    }

    IEnumerator MoveNeedle(float fromAngle, float toAngle)
    {
        float duration = 2f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += GameTimeSystem.Instance.DeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float angle = Mathf.Lerp(fromAngle, toAngle, t);
            needle.localRotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
        needle.localRotation = Quaternion.Euler(0, 0, toAngle);
    }
}
