using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Runtime.InteropServices;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class HoloVideoObject : MonoBehaviour, IDisposable
{
    public TargetDevices targetDevice;
    public bool flipHandedness = false;

    // settings available in Unity editor
    public SVFOpenInfo Settings = new SVFOpenInfo()
    {
        AudioDisabled = false,
        AutoLooping = true,
        RenderViaClock = true,
        OutputNormals = true,
        PerformCacheCleanup = false,
        StartDownloadOnOpen = false,
        UseFrameCache = false,
        PlaybackRate = 1.0f,
        UserCacheLocation = null,
        UseHWDecode = true,
        UseHWTexture = true,
        lockHWTextures = false,
        forceSoftwareClock = false,
        RenderLastFramesTransparent = true,
        HRTFCutoffDistance = float.MaxValue,
        HRTFGainDistance = 1.0f,
        HRTFMinGain = -10.0f,
        HRTFMaxGain = 12.0f
    };
    public uint DefaultMaxVertexCount = 15000; // we usually read max number of vertices from SVF file, as part of SVFFileInfo. However, if we fail to load it from file, we default to this value
    public uint DefaultMaxIndexCount = 45000; // see comment above

    private bool hasCentroid = false;
    private Bounds maxBounds = new Bounds();
    public Vector3 Centroid
    {
        get
        {
            if (false == hasCentroid)
            {
                return transform.position;
            }
            return maxBounds.center;
        }
    }

    protected SVFUnityPluginInterop pluginInterop = null;
    protected bool isInitialized = false;
    protected SVFOpenInfo openInfo;
    public SVFFileInfo fileInfo = new SVFFileInfo();
    protected SVFFrameInfo lastFrameInfo = new SVFFrameInfo();
    public string Url = "";

    protected CameraViews cameraViews = new CameraViews();

    public delegate void OnOpenEvent(HoloVideoObject sender, string url);
    public delegate void OnFrameInfoEvent(HoloVideoObject sender, SVFFrameInfo frameInfo);
    public delegate void OnRenderEvent(HoloVideoObject sender);
    public delegate void OnEndOfStream(HoloVideoObject sender);
    public delegate void OnFatalErrorEvent(HoloVideoObject sender);

    public OnOpenEvent OnOpenNotify = null;
    public OnFrameInfoEvent OnUpdateFrameInfoNotify = null;
    public OnRenderEvent OnRenderCallback = null;
    public OnFatalErrorEvent OnFatalError = null;
    public OnEndOfStream OnEndOfSthreamNotify = null; // derived class should register for this event

    private bool isEditorPlaying = true;

    private bool isClipPlay = false;
    private bool isClipPaused = false;

    // ------------------------------------------------------------------------
    void OnRenderObject()
    {
        UpdateCamera();
        HandleOnRender();
    }

    void OnPostRender()
    {
    }

    public void OnEnable()
    {
        if (null != pluginInterop)
        {
            pluginInterop.CleanupCommandBuffers();
        }
#if UNITY_EDITOR
        EditorApplication.playmodeStateChanged += EditorStateChange;
#endif
    }

#if UNITY_EDITOR
    void EditorStateChange()
    {
        isEditorPlaying = EditorApplication.isPlaying && !EditorApplication.isPaused;
    }
#endif

    public void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.playmodeStateChanged -= EditorStateChange;
#endif
        if (null != pluginInterop)
        {
            pluginInterop.CleanupCommandBuffers();
        }
    }

    public void UpdateCamera()
    {
        var cam = Camera.current;
        if (null == cam)
        {
            return;
        }
        CameraView camView = cameraViews.Find(cam, targetDevice, flipHandedness);
        if (null != camView)
        {
            if (UnityEngine.XR.XRSettings.enabled &&
                cam.cameraType == CameraType.Game &&
                cam.stereoEnabled &&
                (cam.stereoTargetEye == StereoTargetEyeMask.Left || cam.stereoTargetEye == StereoTargetEyeMask.Right))
            {
                UnityEngine.XR.XRNode eye = (cam.stereoTargetEye == StereoTargetEyeMask.Left) ? UnityEngine.XR.XRNode.LeftEye : UnityEngine.XR.XRNode.RightEye;
                camView.UpdateWithVR(eye, transform.localToWorldMatrix);
            }
            else
            {
                camView.Update(cam, transform.localToWorldMatrix);
            }

            if (null != pluginInterop)
            {
                SVFCameraView svfCamView = cameraViews[cam].InteropCamView;
                pluginInterop.SetCameraView(ref svfCamView);
            }
        }
        return;
    }

    // Whenever any camera will render us, add a command buffer to do the work on it
    public void OnWillRenderObject()
    {
        var cam = Camera.current;
        if (null == cam)
        {
            return;
        }
        CameraView camView = cameraViews.Find(cam, targetDevice, flipHandedness);
        if (null != camView && null != pluginInterop)
        {
            pluginInterop.SetCommandBufferOnCamera(cam, camView.cameraId);
        }
    }

    // ------------------------------------------------------------------------
    public void Update()
    {
        if (null != pluginInterop)
        {

            if (pluginInterop.GetHCapObjectFrameInfo(ref lastFrameInfo))
            {
                if ((lastFrameInfo.maxX - lastFrameInfo.minX) > 0.0f ||
                    (lastFrameInfo.maxY - lastFrameInfo.minY) > 0.0f ||
                    (lastFrameInfo.maxZ - lastFrameInfo.minZ) > 0.0f)
                {
                    maxBounds.Encapsulate(new Vector3((float)lastFrameInfo.minX, (float)lastFrameInfo.minY, (float)lastFrameInfo.minZ));
                    maxBounds.Encapsulate(new Vector3((float)lastFrameInfo.maxX, (float)lastFrameInfo.maxY, (float)lastFrameInfo.maxZ));
                    hasCentroid = true;
                }
                HandleOnUpdateFrameInfo(lastFrameInfo);
            }
            else
            {
                Logger.Log("GetHCapObjectFrameInfo returned false");
            }

            if (lastFrameInfo.isEOS)
            {
                if (null != OnEndOfSthreamNotify)
                {
                    OnEndOfSthreamNotify(this);
                }
            }
#if UNITY_EDITOR
            /*
            string[] trace = pluginInterop.GetTrace();
            if(null != trace)
            {
                foreach(var line in trace)
                {
                    Logger.Log(line);
                }
            }
            */
#endif
        }
    }

    // ------------------------------------------------------------------------
    public void SetClockScale(float scale)
    {
        if (null != pluginInterop)
        {
            pluginInterop.SetClockScale(scale);
        }
    }

    // ------------------------------------------------------------------------
    public float GetClockScale()
    {
        if (null != pluginInterop)
        {
            return pluginInterop.GetClockScale();
        }
        return 1.0f;
    }
    // ------------------------------------------------------------------------
    virtual protected void HandleOnUpdateFrameInfo(SVFFrameInfo frameInfo)
    {
        if (null != OnUpdateFrameInfoNotify)
        {
            OnUpdateFrameInfoNotify(this, frameInfo);
        }
    }

    // ------------------------------------------------------------------------
    virtual protected void HandleOnOpen(string url)
    {
        if (null != OnOpenNotify)
        {
            OnOpenNotify(this, url);
        }
    }

    virtual protected void HandleOnRender()
    {
        if (null != OnRenderCallback)
        {
            OnRenderCallback(this);
        }
    }

    // ------------------------------------------------------------------------
    public bool Open(string urlPath)
    {
        if (!isInitialized)
        {
            if (false == Initialize())
            {
                return false;
            }
        }
        bool res = pluginInterop.OpenHCapObject(urlPath, ref Settings);
        if (res == true)
        {
            Url = urlPath;

            if (true == pluginInterop.GetHCapObjectFileInfo(ref fileInfo))
            {
                if ((fileInfo.maxX - fileInfo.minX) > 0.0f ||
                    (fileInfo.maxY - fileInfo.minY) > 0.0f ||
                    (fileInfo.maxZ - fileInfo.minZ) > 0.0f)
                {
                    maxBounds.Encapsulate(new Vector3((float)fileInfo.minX, (float)fileInfo.minY, (float)fileInfo.minZ));
                    maxBounds.Encapsulate(new Vector3((float)fileInfo.maxX, (float)fileInfo.maxY, (float)fileInfo.maxZ));
                    hasCentroid = true;
                }
            }
            HandleOnOpen(urlPath);
        }
        return res;
    }

    // ------------------------------------------------------------------------
    public bool Play(bool hard = true)
    {
        if (null == pluginInterop)
        {
            return false;
        }
        if (hard)
        {
            isClipPlay = true;
            isClipPaused = false;
        }
        return pluginInterop.PlayHCapObject();
    }

    // ------------------------------------------------------------------------
    public bool Pause(bool hard = true)
    {
        if (null == pluginInterop)
        {
            return false;
        }
        if (hard)
        {
            isClipPaused = true;
        }
        return pluginInterop.PauseHCapObject();
    }

    // ------------------------------------------------------------------------
    public bool Rewind()
    {
        if (null == pluginInterop)
        {
            return false;
        }
        return pluginInterop.RewindHCapObject();
    }

    // ------------------------------------------------------------------------
    public bool Close()
    {
        isClipPlay = isClipPaused = false;
        if (null == pluginInterop)
        {
            return false;
        }
        return pluginInterop.CloseHCapObject();
    }

    // ------------------------------------------------------------------------
    public void Cleanup()
    {
        if (null != pluginInterop)
        {
            pluginInterop.Dispose();
        }
        pluginInterop = null;
        isInitialized = false;
    }

    // ------------------------------------------------------------------------
    public SVFPlaybackState GetCurrentState()
    {
        if (null == pluginInterop)
        {
            return SVFPlaybackState.Empty;
        }
        return pluginInterop.GetHCapState();
    }

    // ------------------------------------------------------------------------
    private bool Initialize()
    {
        try
        {

            // TODO: virtualize this! 
            UnityConfig unityConfig = new UnityConfig();
            unityConfig.VertexShaderPath = Application.streamingAssetsPath + "/Shaders/DefaultVS.cso\0";
            unityConfig.PixelShaderPath = Application.streamingAssetsPath + "/Shaders/DefaultPS.cso\0";
            unityConfig.AtlasVertexShaderPath = Application.streamingAssetsPath + "/Shaders/FlatQuadVS.cso\0";
            unityConfig.BBoxFrameTexturePath = Application.streamingAssetsPath + "/BBoxFrameTexture.png\0";
            unityConfig.BBoxClipTexturePath = Application.streamingAssetsPath + "/BBoxClipTexture.png\0";
            unityConfig.EnableBBoxDrawing = false;
            unityConfig.EnableAtlasDrawing = false;
            if (null != pluginInterop)
            {
                pluginInterop.Dispose();
            }
            this.pluginInterop = new SVFUnityPluginInterop(unityConfig);

            HCapSettingsInterop hcapSettings = new HCapSettingsInterop()
            {
                defaultMaxVertexCount = DefaultMaxVertexCount,
                defaultMaxIndexCount = DefaultMaxVertexCount,
                allocMemoryMinGb = MemorySettings.MinGbLimitHW,
                allocMemoryMaxGb = MemorySettings.MaxGbLimitHW
            };

            pluginInterop.CreateHCapObject(hcapSettings);

            pluginInterop.CleanupCommandBuffers();
            foreach (var cam in Camera.allCameras)
            {
                CameraView camView = cameraViews.Find(cam, targetDevice, flipHandedness);
                pluginInterop.SetCommandBufferOnCamera(cam, camView.cameraId);
            }

#if UNITY_5_5_OR_NEWER
            // Needed for compatibility with Unity 5.5 and above
            pluginInterop.SetZBufferInverted(true);
#endif

            isInitialized = true;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            isInitialized = false;
            return false;
        }
        return true;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus && isEditorPlaying && isClipPlay && !isClipPaused)
        {
            Play(false);
        }
        else
        {
            Pause(false);
        }
    }

    private void OnApplicationQuit()
    {
        Close();
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause && isEditorPlaying && isClipPlay && !isClipPaused)
        {
            Play(false);
        }
        else
        {
            Pause(false);
        }
    }

    private bool isInstanceDisposed = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!isInstanceDisposed)
        {
            isInstanceDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~HoloVideoObject()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(false);
    }
}
