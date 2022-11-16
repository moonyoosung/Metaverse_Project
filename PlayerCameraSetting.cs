using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraSetting : MonoBehaviourPun
{
    [Header("PlayerCamera")]
    public Camera camSetting;
    [Header("WorldCamera")]
    public Camera mainCamera;
    public CinemachineFreeLook cinemachineFree;
    public float zoomValue = 100;
    public CinemachineFreeLook.Orbit[] originalOrbits;

    public CinemachineInputProvider cinemachineInputProvider;
    public InputActionReference touch0;
    public InputActionReference touch1;

    public static int fingerIndex = 0;

    private MainView mobilePlayerBehaviour;
    private bool isLocalPlayer;
    private ResourceManager resourceManager;

    private void OnDestroy()
    {
        if (isLocalPlayer)
        {
            mobilePlayerBehaviour.UnResistMobileRotate(OnDragBegin, OnDragEnd);
        }
    }

    public void Initialize(ResourceManager resourceManager, bool isLocalPlayer, bool activeAudioListener)
    {
        this.isLocalPlayer = isLocalPlayer;
        if (isLocalPlayer)
            camSetting.GetComponent<AudioListener>().enabled = activeAudioListener;

        this.resourceManager = resourceManager;
        mobilePlayerBehaviour = UIView.Get<MainView>();
        mobilePlayerBehaviour.ResistMobileRotate(OnDragBegin, OnDragEnd);

        StartCoroutine(Initialize());
    }

    public IEnumerator Initialize()
    {
        yield return new WaitForSeconds(0.2f);
        mainCamera = MindPlus.GameManager.Instance.SceneCamera;
        if (mainCamera == null)
            yield break;
        mainCamera.cullingMask = camSetting.cullingMask;
        //Debug.Log("??????????? , " + resourceManager);
        mainCamera.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = MindPlus.GameManager.Instance.Persistent.ResourceManager.SettingData.cameraBlendSpeed;

        originalOrbits = new CinemachineFreeLook.Orbit[cinemachineFree.m_Orbits.Length];
        for (int i = 0; i < cinemachineFree.m_Orbits.Length; i++)
        {
            originalOrbits[i].m_Height = cinemachineFree.m_Orbits[i].m_Height;
            originalOrbits[i].m_Radius = cinemachineFree.m_Orbits[i].m_Radius;
        }
        cinemachineFree.Priority = 10;
    }

    private void OnDragBegin(Vector2 pos)
    {
        if (fingerIndex == 0)
            cinemachineInputProvider.XYAxis = touch0;
        else
            cinemachineInputProvider.XYAxis = touch1;

        cinemachineFree.m_XAxis.m_MaxSpeed = Time.deltaTime * 10;
        cinemachineFree.m_YAxis.m_MaxSpeed = Time.deltaTime * 0.1f;
    }
    private void OnDragEnd()
    {
        cinemachineFree.m_XAxis.m_MaxSpeed = 0;
        cinemachineFree.m_YAxis.m_MaxSpeed = 0;
    }

    float perspectiveZoomSpeed = 0.05f;

    public void Zoom(UnityEngine.InputSystem.EnhancedTouch.Touch touchZero, UnityEngine.InputSystem.EnhancedTouch.Touch touchOne)
    {
        cinemachineFree.m_XAxis.m_MaxSpeed = 0;
        cinemachineFree.m_YAxis.m_MaxSpeed = 0;

        //터치에 대한 이전 위치값을 각각 저장함
        //처음 터치한 위치(touchZero.position)에서 이전 프레임에서의 터치 위치와 이번 프로임에서 터치 위치의 차이를 뺌
        Vector2 touchZeroPrevPos = touchZero.screenPosition - touchZero.delta; //deltaPosition는 이동방향 추적할 때 사용
        Vector2 touchOnePrevPos = touchOne.screenPosition - touchOne.delta;

        // 각 프레임에서 터치 사이의 벡터 거리 구함
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude; //magnitude는 두 점간의 거리 비교(벡터)
        float touchDeltaMag = (touchZero.screenPosition - touchOne.screenPosition).magnitude;

        // 거리 차이 구함(거리가 이전보다 크면(마이너스가 나오면)손가락을 벌린 상태_줌인 상태)
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        zoomValue += deltaMagnitudeDiff * perspectiveZoomSpeed;
        zoomValue = Mathf.Clamp(zoomValue, 10f, 100);


        for (int i = 0; i < cinemachineFree.m_Orbits.Length; i++)
        {
            cinemachineFree.m_Orbits[i].m_Height = Mathf.Clamp(zoomValue * 0.01f, 0.1f, 1) * originalOrbits[i].m_Height;
            cinemachineFree.m_Orbits[i].m_Radius = Mathf.Clamp(zoomValue * 0.01f, 0.1f, 1) * originalOrbits[i].m_Radius;
        }
    }
    public void Zoom(Touch touchZero, Touch touchOne)
    {
        cinemachineFree.m_XAxis.m_MaxSpeed = 0;
        cinemachineFree.m_YAxis.m_MaxSpeed = 0;

        //터치에 대한 이전 위치값을 각각 저장함
        //처음 터치한 위치(touchZero.position)에서 이전 프레임에서의 터치 위치와 이번 프로임에서 터치 위치의 차이를 뺌
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition; //deltaPosition는 이동방향 추적할 때 사용
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // 각 프레임에서 터치 사이의 벡터 거리 구함
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude; //magnitude는 두 점간의 거리 비교(벡터)
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // 거리 차이 구함(거리가 이전보다 크면(마이너스가 나오면)손가락을 벌린 상태_줌인 상태)
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        zoomValue += deltaMagnitudeDiff * perspectiveZoomSpeed;
        zoomValue = Mathf.Clamp(zoomValue, 10f, 100);


        for (int i = 0; i < cinemachineFree.m_Orbits.Length; i++)
        {
            cinemachineFree.m_Orbits[i].m_Height = Mathf.Clamp(zoomValue * 0.01f, 0.1f, 1) * originalOrbits[i].m_Height;
            cinemachineFree.m_Orbits[i].m_Radius = Mathf.Clamp(zoomValue * 0.01f, 0.1f, 1) * originalOrbits[i].m_Radius;
        }
    }

    void SetFOV(float value)
    {
        cinemachineFree.m_Lens.FieldOfView = value;
    }
    public void AddMask(string name)
    {
        mainCamera.cullingMask |= 1 << LayerMask.NameToLayer(name);
    }

    public void RemoveMask(string name)
    {
        mainCamera.cullingMask = mainCamera.cullingMask & ~(1 << LayerMask.NameToLayer(name));
    }

    public void RotateHeadCamera(Vector3 localRotation)
    {
        camSetting.transform.localRotation = Quaternion.Euler(localRotation);
    }
}
