using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [Header("Virtual Cameras")]
    public CinemachineCamera firstPersonCam;
    public CinemachineCamera thirdPersonCam;
    public CinemachineCamera zoomCam;

    [Header("Shake Settings")]
    public float defaultShakeAmplitude = 1f;
    public float defaultShakeFrequency = 2f;

    //zoom settings
    private bool isZoomedIn = false;
    private float defaultFOV;
    private float zoomOutTime = 0.2f;

    private CinemachineBasicMultiChannelPerlin currentNoise;
    private Coroutine zoomRoutine;
    private Coroutine bobRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    private void Start()
    {
        ActivateCamera(firstPersonCam); // default
        defaultFOV = GetActiveCam().Lens.FieldOfView;
    }

    private void Update()
    {
        //toggle the camera view
        if(Input.GetKey(KeyCode.C) && GetActiveCam() == firstPersonCam)
        {
            ActivateCamera(thirdPersonCam, 3f);

        } else if (Input.GetKey(KeyCode.C) && GetActiveCam() == thirdPersonCam)
        {
            ActivateCamera(firstPersonCam, 3f);
        }
        //zoom test
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //check to see if zooming in or not
            if (!isZoomedIn)
            {
                isZoomedIn=true;
                ZoomIn(40f, 0.5f);
            }
            else
            {
                isZoomedIn=false;
                ZoomOut();
            }
            
        }

        //shake test
        if (Input.GetKey(KeyCode.X))
        {
            Shake(2f, 3f, 0.5f);
        }

    }

    // ---- CAMERA SWITCHING ----
    public void ActivateCamera(CinemachineCamera cam, float blendTime = 1f)
    {
        firstPersonCam.Priority = 0;
        thirdPersonCam.Priority = 0;
        zoomCam.Priority = 0;
        cam.Priority = 10;
    }

    // ---- ZOOM ----
    public void ZoomIn(float targetFOV, float duration)
    {
        if (zoomRoutine != null) StopCoroutine(zoomRoutine);
        zoomRoutine = StartCoroutine(ZoomCoroutine(targetFOV, duration));
    }

    public void ZoomOut()
    {
        if (zoomRoutine != null) StopCoroutine(zoomRoutine);
        zoomRoutine = StartCoroutine(ZoomCoroutine(defaultFOV, zoomOutTime));
    }

    private IEnumerator ZoomCoroutine(float targetFOV, float duration)
    {
        var cam = GetActiveCam();
        float startFOV = cam.Lens.FieldOfView;
        float t = 0;
        while (t < duration)
        {
            cam.Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        cam.Lens.FieldOfView = targetFOV;
    }

    // ---- SHAKE ----
    public void Shake(float amplitude, float frequency, float duration)
    {
        var cam = GetActiveCam();
        currentNoise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        if (currentNoise == null) return;

        StartCoroutine(ShakeCoroutine(amplitude, frequency, duration));
    }

    private IEnumerator ShakeCoroutine(float amplitude, float frequency, float duration)
    {
        currentNoise.AmplitudeGain = amplitude;
        currentNoise.FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        currentNoise.AmplitudeGain = 0;
        currentNoise.FrequencyGain = 0;
    }

    // ---- BOB ----
    public void EnableBobbing(bool enable, float intensity = 0.05f, float speed = 2f)
    {
        if (bobRoutine != null) StopCoroutine(bobRoutine);
        if (enable)
            bobRoutine = StartCoroutine(BobbingCoroutine(intensity, speed));
    }

    private IEnumerator BobbingCoroutine(float intensity, float speed)
    {
        var cam = GetActiveCam().transform;
        Vector3 startPos = cam.localPosition;
        while (true)
        {
            float bob = Mathf.Sin(Time.time * speed) * intensity;
            cam.localPosition = startPos + new Vector3(0, bob, 0);
            yield return null;
        }
    }

    // ---- UTIL ----
    private CinemachineCamera GetActiveCam()
    {
        if (firstPersonCam.Priority > 0) return firstPersonCam;
        if (thirdPersonCam.Priority > 0) return thirdPersonCam;
        if (zoomCam.Priority > 0) return zoomCam;
        return firstPersonCam;
    }
}
