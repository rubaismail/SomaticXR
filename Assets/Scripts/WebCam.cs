using UnityEngine;
using System.Collections;
using Meta.XR.ImmersiveDebugger;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class WebCam : MonoBehaviour
{
    [SerializeField] private string preferredCameraName = "1080P Pro Stream";
    [SerializeField] private int requestedWidth = 1280;
    [SerializeField] private int requestedHeight = 720;
    [SerializeField] private int requestedFPS = 30;

    // permission string that enabled the "Connected Cameras" permission
    private const string UsbCameraPermission = "horizonos.permission.USB_CAMERA";

    private WebCamTexture _webcamTexture;
    private Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
#if UNITY_ANDROID
        RequestPermission_Android();
#else
        InitializeCamera();
#endif
    }

#if UNITY_ANDROID
    // handle andriod permissions 
    private void RequestPermission_Android()
    {
        if (Permission.HasUserAuthorizedPermission(UsbCameraPermission))
        {
            StartCoroutine(DelayedInit());
            return;
        }

        var callbacks = new PermissionCallbacks();
        callbacks.PermissionGranted += OnPermissionGranted;
        callbacks.PermissionDenied  += OnPermissionDenied;
        Permission.RequestUserPermission(UsbCameraPermission, callbacks);
    }

    private void OnPermissionGranted(string permissionName)
    {
        StartCoroutine(DelayedInit());
    }

    private void OnPermissionDenied(string permissionName)
    {
        if (!Permission.ShouldShowRequestPermissionRationale(permissionName))
            Debug.LogWarning($"[WebCam] Permission permanently denied: {permissionName}. User must enable it manually in device settings.");
        else
            Debug.LogWarning($"[WebCam] Permission denied: {permissionName}. Will not re-request automatically.");
    }
#endif

    // wait to try to give the quest the time it needs before displaying the camera feed (might need more time)
    private IEnumerator DelayedInit()
    {
        yield return null;
        yield return null;
        yield return null;
        InitializeCamera();
    }

    private void InitializeCamera()
    {
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("[WebCam] No camera devices found.");
            return;
        }

        // log all the camera names to see of the external one shows up
        Debug.Log($"[WebCam] Total devices found: {WebCamTexture.devices.Length}");
        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            var device = WebCamTexture.devices[i];
            Debug.Log($"[WebCam] Device [{i}]: name=\"{device.name}\" | Front-facing: {device.isFrontFacing}");
        }

        // set the target camera to the specified name, or to the first discovered camera if one was not given
        string targetCamera = preferredCameraName;
        if (string.IsNullOrEmpty(targetCamera))
            targetCamera = WebCamTexture.devices[0].name;

        _webcamTexture = new WebCamTexture(targetCamera, requestedWidth, requestedHeight, requestedFPS);

        if (_renderer != null)
            _renderer.material.mainTexture = _webcamTexture;

        _webcamTexture.Play();
        Debug.Log($"[WebCam] Started: {targetCamera} at {requestedWidth}x{requestedHeight} with {requestedFPS}fps");

        StartCoroutine(WaitForFirstFrame());
    }

    // confirm frames are actually arriving after Play() is called
    private IEnumerator WaitForFirstFrame()
    {
        float timeout = 5f;
        float elapsed = 0f;

        while (!_webcamTexture.didUpdateThisFrame)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= timeout)
            {
                Debug.LogError("[WebCam] Timed out waiting for first frame. Camera may not be streaming.");
                yield break;
            }
            yield return null;
        }

        Debug.Log($"[WebCam] First frame received. Resolution: {_webcamTexture.width}x{_webcamTexture.height}");
    }

    // free resources if they are not being used
    private void OnDestroy()
    {
        if (_webcamTexture != null && _webcamTexture.isPlaying)
        {
            _webcamTexture.Stop();
            _webcamTexture = null;
        }
    }
}