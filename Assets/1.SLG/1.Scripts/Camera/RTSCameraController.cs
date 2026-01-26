using UnityEngine;

// SLG/RTS용 자유이동 카메라 컨트롤러 (오류 수정 버전)
// 기능
// - WASD 이동 (카메라 바라보는 방향 기준)
// - 마우스 휠 줌 (거리 제한)
// - 우클릭 드래그 회전 (Yaw/Pitch)
// - 휠 클릭 드래그 패닝(수평 이동)
// - Q/E 높이 조절 (옵션)
// - 화면 가장자리 이동(옵션)
// - 경계 박스(월드 좌표) 내 이동(옵션)
// - 지면 최소 높이 유지(옵션)

[DisallowMultipleComponent]
public class RTSCameraController : MonoBehaviour
{
    [Header("입력 키")]
    public KeyCode fastKey = KeyCode.LeftShift;

    [Header("마우스 버튼 설정")]
    [Tooltip("0=왼쪽, 1=오른쪽, 2=휠 클릭")]
    [Range(0, 2)] public int rotateMouseButton = 1; // 우클릭
    [Range(0, 2)] public int panMouseButton = 2;    // 휠 클릭

    [Header("이동 속도")]
    public float moveSpeed = 15f;
    public float fastMultiplier = 2.5f;
    public float edgePanSpeed = 20f;

    [Header("줌/회전")]
    public float zoomSpeed = 200f;
    public float minZoomDistance = 5f;
    public float maxZoomDistance = 80f;
    public float rotateSpeed = 120f;
    public float minPitch = 15f;
    public float maxPitch = 80f;

    [Header("패닝(휠 클릭 드래그)")]
    public float panDragSpeed = 1.0f;

    [Header("가장자리 이동(옵션)")]
    public bool useEdgePan = true;
    [Range(1, 50)] public int edgeThickness = 10;

    [Header("경계(옵션)")]
    public bool useBounds = false;
    public Bounds moveBounds = new Bounds(Vector3.zero, new Vector3(200, 200, 200));

    [Header("지형 충돌(옵션)")]
    public bool keepMinHeightOverGround = false;
    public float minHeightOverGround = 3f;
    public LayerMask groundMask = 1 << 0;

    [Header("기타")]
    public Transform pivot;
    public Transform cameraRig;
    public float smoothTime = 0.08f;

    private Vector3 _targetPos;
    private float _targetYaw;
    private float _targetPitch;
    private float _targetDistance;
    private Vector3 _vel;
    private Camera _cam;

    void Awake()
    {
        if (pivot == null) pivot = transform;

        if (cameraRig == null)
        {
            _cam = Camera.main;
            if (_cam != null) cameraRig = _cam.transform;
            else cameraRig = transform;
        }
        else
        {
            _cam = cameraRig.GetComponentInChildren<Camera>();
            if (_cam == null) _cam = Camera.main;
        }

        _targetPos = pivot.position;

        Vector3 dir = cameraRig.position - pivot.position;
        _targetDistance = dir.magnitude;
        if (_targetDistance < 0.01f) _targetDistance = 10f;

        Vector3 fwd = dir.normalized;
        _targetYaw = Mathf.Atan2(fwd.x, fwd.z) * Mathf.Rad2Deg;
        _targetPitch = Mathf.Clamp(-Mathf.Asin(fwd.y) * Mathf.Rad2Deg, minPitch, maxPitch);

        ApplyImmediate();
    }

    void Update()
    {
        HandleInput();
        ConstrainByBounds();
        KeepHeightOverGround();
        SmoothMove();
    }

    void HandleInput()
    {
        float dt = Time.deltaTime;
        float mult = Input.GetKey(fastKey) ? fastMultiplier : 1f;

        // WASD 이동
        Vector2 wasd = new Vector2(
            (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0),
            (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0)
        );

        Vector3 forwardFlat = Quaternion.Euler(0, _targetYaw, 0) * Vector3.forward;
        Vector3 rightFlat = Quaternion.Euler(0, _targetYaw, 0) * Vector3.right;
        Vector3 move = (forwardFlat * wasd.y + rightFlat * wasd.x) * (moveSpeed * mult) * dt;

        //if (Input.GetKey(KeyCode.Q)) move += Vector3.down * moveSpeed * 0.6f * dt;
        //if (Input.GetKey(KeyCode.E)) move += Vector3.up * moveSpeed * 0.6f * dt;

        _targetPos += move;

        // 화면 가장자리 이동
        if (useEdgePan && !Input.GetMouseButton(rotateMouseButton) && !Input.GetMouseButton(panMouseButton))
        {
            Vector2 m = Input.mousePosition;
            Vector3 edgeMove = Vector3.zero;
            if (m.x <= edgeThickness) edgeMove -= rightFlat;
            else if (m.x >= Screen.width - edgeThickness) edgeMove += rightFlat;
            if (m.y <= edgeThickness) edgeMove -= forwardFlat;
            else if (m.y >= Screen.height - edgeThickness) edgeMove += forwardFlat;
            _targetPos += edgeMove.normalized * edgePanSpeed * mult * dt;
        }

        // 줌
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            _targetDistance = Mathf.Clamp(_targetDistance - scroll * (zoomSpeed * 0.01f), minZoomDistance, maxZoomDistance);
        }

        // 회전 (우클릭)
        if (Input.GetMouseButton(rotateMouseButton))
        {
            float yawDelta = Input.GetAxis("Mouse X") * rotateSpeed * dt;
            float pitchDelta = -Input.GetAxis("Mouse Y") * rotateSpeed * dt;
            _targetYaw += yawDelta;
            _targetPitch = Mathf.Clamp(_targetPitch + pitchDelta, minPitch, maxPitch);
        }

        // 패닝 (휠 클릭)
        if (Input.GetMouseButton(panMouseButton))
        {
            float dx = -Input.GetAxis("Mouse X") * panDragSpeed * _targetDistance * dt;
            float dy = -Input.GetAxis("Mouse Y") * panDragSpeed * _targetDistance * dt;
            _targetPos += rightFlat * dx + forwardFlat * dy;
        }
    }

    void ConstrainByBounds()
    {
        if (!useBounds) return;
        _targetPos = moveBounds.ClosestPoint(_targetPos);
    }

    void KeepHeightOverGround()
    {
        if (!keepMinHeightOverGround || _cam == null) return;
        Vector3 origin = new Vector3(_targetPos.x, _targetPos.y + 200f, _targetPos.z);
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 500f, groundMask))
        {
            float groundY = hit.point.y;
            if (_targetPos.y < groundY + minHeightOverGround)
                _targetPos.y = groundY + minHeightOverGround;
        }
    }

    void SmoothMove()
    {
        pivot.position = Vector3.SmoothDamp(pivot.position, _targetPos, ref _vel, smoothTime);
        Quaternion rot = Quaternion.Euler(_targetPitch, _targetYaw, 0f);
        pivot.rotation = Quaternion.Slerp(pivot.rotation, rot, 1f - Mathf.Exp(-10f * Time.deltaTime));

        Vector3 desiredCamPos = pivot.position - pivot.forward * _targetDistance;
        cameraRig.position = Vector3.Lerp(cameraRig.position, desiredCamPos, 1f - Mathf.Exp(-10f * Time.deltaTime));
        cameraRig.rotation = pivot.rotation;
    }

    void ApplyImmediate()
    {
        pivot.position = _targetPos;
        pivot.rotation = Quaternion.Euler(_targetPitch, _targetYaw, 0f);
        cameraRig.position = pivot.position - pivot.forward * _targetDistance;
        cameraRig.rotation = pivot.rotation;
    }

    void OnDrawGizmosSelected()
    {
        if (useBounds)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(moveBounds.center, moveBounds.size);
        }
    }
}