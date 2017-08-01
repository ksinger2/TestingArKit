using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

// ------------------------------------------------------------------------
class MemorySettings
{
    // 32 bit settings
    public static float MinGbLimitHW = 1.75f;
    public static float MaxGbLimitHW = 2.5f;
    public static float MinGbLimitSW = 1.0f;
    public static float MaxGbLimitSW = 2.2f;
}

// ------------------------------------------------------------------------
[Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct UnityConfig
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
    public string VertexShaderPath;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
    public string PixelShaderPath;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
    public string AtlasVertexShaderPath;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
    public string BBoxFrameTexturePath;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
    public string BBoxClipTexturePath;
    [MarshalAs(UnmanagedType.I1)]
    public bool EnableBBoxDrawing;
    [MarshalAs(UnmanagedType.I1)]
    public bool EnableAtlasDrawing;
}

// ------------------------------------------------------------------------
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct HCapSettingsInterop
{
    [MarshalAs(UnmanagedType.U4)]
    public uint defaultMaxVertexCount;
    [MarshalAs(UnmanagedType.U4)]
    public uint defaultMaxIndexCount;
    [MarshalAs(UnmanagedType.R4)]
    public float allocMemoryMinGb; // lower memory limit in cached mode: stop running our buffer cleaner if process takes less than this value
    [MarshalAs(UnmanagedType.R4)]
    public float allocMemoryMaxGb; // upper memory limit in cached mode: activate buffer cleaner if process takes more than this value
};

// ------------------------------------------------------------------------
[Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct SVFOpenInfo
{
    [MarshalAs(UnmanagedType.I1)]
    public bool AudioDisabled;
    [MarshalAs(UnmanagedType.I1)]
    public bool UseHWTexture;
    [MarshalAs(UnmanagedType.I1)]
    public bool UseHWDecode;
    [MarshalAs(UnmanagedType.I1)]
    public bool RenderViaClock;
    [MarshalAs(UnmanagedType.I1)]
    public bool OutputNormals;
    [MarshalAs(UnmanagedType.I1)]
    public bool PerformCacheCleanup;
    [MarshalAs(UnmanagedType.I1)]
    public bool StartDownloadOnOpen;
    [MarshalAs(UnmanagedType.I1)]
    public bool UseFrameCache;
    [MarshalAs(UnmanagedType.I1)]
    public bool AutoLooping;
    [MarshalAs(UnmanagedType.I1)]
    public bool lockHWTextures;
    [MarshalAs(UnmanagedType.I1)]
    public bool forceSoftwareClock;
    [MarshalAs(UnmanagedType.I1)]
    public bool RenderLastFramesTransparent;
    [MarshalAs(UnmanagedType.R4)]
    public float PlaybackRate;
    [MarshalAs(UnmanagedType.R4)]
    public float HRTFMinGain;
    [MarshalAs(UnmanagedType.R4)]
    public float HRTFMaxGain;
    [MarshalAs(UnmanagedType.R4)]
    public float HRTFGainDistance;
    [MarshalAs(UnmanagedType.R4)]
    public float HRTFCutoffDistance;
    [MarshalAs(UnmanagedType.LPWStr, SizeConst = 1024)]
    public string UserCacheLocation;
}

// ------------------------------------------------------------------------
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct SVFFileInfo
{
    [MarshalAs(UnmanagedType.I1)]
    public bool hasAudio;
    [MarshalAs(UnmanagedType.U8)]
    public ulong duration100ns;
    [MarshalAs(UnmanagedType.U4)]
    public uint frameCount;
    [MarshalAs(UnmanagedType.U4)]
    public uint maxVertexCount;
    [MarshalAs(UnmanagedType.U4)]
    public uint maxIndexCount;
    [MarshalAs(UnmanagedType.R4)]
    public float bitrateMbps;
    [MarshalAs(UnmanagedType.R8)]
    public double minX;
    [MarshalAs(UnmanagedType.R8)]
    public double minY;
    [MarshalAs(UnmanagedType.R8)]
    public double minZ;
    [MarshalAs(UnmanagedType.R8)]
    public double maxX;
    [MarshalAs(UnmanagedType.R8)]
    public double maxY;
    [MarshalAs(UnmanagedType.R8)]
    public double maxZ;
}

// ------------------------------------------------------------------------
public enum SVFReaderStateInterop
{
    Unknown = 0,        //!< Reader is in unknown state
    Initialized = 1,    //!< Reader was properly initialized
    OpenPending = 2,    //!< Open is pending
    Opened = 3,         //!< Open finished
    Prerolling = 4,     //!< File opened and pre-roll started
    Ready = 5,          //!< Frames ready to be delivered
    Buffering = 6,      //!< Reader is buffering frames (i.e. frames not available for delivery)
    Closing = 7,        //!< File being closed
    Closed = 8,         //!< Filed closed
    EndOfStream = 9,    //!< Reached end of current file
    ShuttingDown = 10,   //!< SVFReader is in process of shutting down
};

// ------------------------------------------------------------------------
public enum VRChannel
{
    Mono = 0,
    Left = 1,
    Right = 2,
    Center = 3,
    Head = 4
};

// ------------------------------------------------------------------------
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct SVFStatusInterop
{
    [MarshalAs(UnmanagedType.I1)]
    public bool isLiveSVFSource;
    [MarshalAs(UnmanagedType.U4)]
    public UInt32 lastReadFrame;
    [MarshalAs(UnmanagedType.U4)]
    public UInt32 unsuccessfulReadFrameCount;
    [MarshalAs(UnmanagedType.U4)]
    public UInt32 droppedFrameCount;
    [MarshalAs(UnmanagedType.U4)]
    public UInt32 errorHresult;
    [MarshalAs(UnmanagedType.I4)]
    public int lastKnownState; // cast of SVFReaderStateInterop
}

// ------------------------------------------------------------------------
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct SVFFrameInfo
{
    [MarshalAs(UnmanagedType.U4)]
    public uint frameId; // starts from 0
    [MarshalAs(UnmanagedType.U8)]
    public ulong frameTimestamp; // in 100ns MF time units
    [MarshalAs(UnmanagedType.U4)]
    public uint vertexFormatVersion; // vertex format flags, as defined in CStreamingMeshHeader::VersionType
    [MarshalAs(UnmanagedType.R8)]
    public double minX;
    [MarshalAs(UnmanagedType.R8)]
    public double minY;
    [MarshalAs(UnmanagedType.R8)]
    public double minZ;
    [MarshalAs(UnmanagedType.R8)]
    public double maxX;
    [MarshalAs(UnmanagedType.R8)]
    public double maxY;
    [MarshalAs(UnmanagedType.R8)]
    public double maxZ;
    [MarshalAs(UnmanagedType.U4)]
    public uint vertexCount; // per-frame vertex count
    [MarshalAs(UnmanagedType.U4)]
    public uint indexCount; // per-frame index count
    [MarshalAs(UnmanagedType.I4)]
    public int textureWidth;
    [MarshalAs(UnmanagedType.I4)]
    public int textureHeight;
    [MarshalAs(UnmanagedType.I1)]
    public bool isEOS;
    [MarshalAs(UnmanagedType.I1)]
    public bool isRepeatedFrame;
};

// ------------------------------------------------------------------------
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct Matrix4x4PluginInterop
{
    [MarshalAs(UnmanagedType.R4)]
    public float m00;
    [MarshalAs(UnmanagedType.R4)]
    public float m01;
    [MarshalAs(UnmanagedType.R4)]
    public float m02;
    [MarshalAs(UnmanagedType.R4)]
    public float m03;
    [MarshalAs(UnmanagedType.R4)]
    public float m10;
    [MarshalAs(UnmanagedType.R4)]
    public float m11;
    [MarshalAs(UnmanagedType.R4)]
    public float m12;
    [MarshalAs(UnmanagedType.R4)]
    public float m13;
    [MarshalAs(UnmanagedType.R4)]
    public float m20;
    [MarshalAs(UnmanagedType.R4)]
    public float m21;
    [MarshalAs(UnmanagedType.R4)]
    public float m22;
    [MarshalAs(UnmanagedType.R4)]
    public float m23;
    [MarshalAs(UnmanagedType.R4)]
    public float m30;
    [MarshalAs(UnmanagedType.R4)]
    public float m31;
    [MarshalAs(UnmanagedType.R4)]
    public float m32;
    [MarshalAs(UnmanagedType.R4)]
    public float m33;
}

// ------------------------------------------------------------------------
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct SVFCameraView
{
    [MarshalAs(UnmanagedType.I4)]
    public int cameraId;
    [MarshalAs(UnmanagedType.R4)]
    public float m00;
    [MarshalAs(UnmanagedType.R4)]
    public float m10;
    [MarshalAs(UnmanagedType.R4)]
    public float m20;
    [MarshalAs(UnmanagedType.R4)]
    public float m30;
    [MarshalAs(UnmanagedType.R4)]
    public float m01;
    [MarshalAs(UnmanagedType.R4)]
    public float m11;
    [MarshalAs(UnmanagedType.R4)]
    public float m21;
    [MarshalAs(UnmanagedType.R4)]
    public float m31;
    [MarshalAs(UnmanagedType.R4)]
    public float m02;
    [MarshalAs(UnmanagedType.R4)]
    public float m12;
    [MarshalAs(UnmanagedType.R4)]
    public float m22;
    [MarshalAs(UnmanagedType.R4)]
    public float m32;
    [MarshalAs(UnmanagedType.R4)]
    public float m03;
    [MarshalAs(UnmanagedType.R4)]
    public float m13;
    [MarshalAs(UnmanagedType.R4)]
    public float m23;
    [MarshalAs(UnmanagedType.R4)]
    public float m33;
    [MarshalAs(UnmanagedType.R4)]
    public float viewportWidth;
    [MarshalAs(UnmanagedType.R4)]
    public float viewportHeight;
    [MarshalAs(UnmanagedType.I1)]
    public bool isGameCamera;
    [MarshalAs(UnmanagedType.I1)]
    public bool isStereoscopic;
    [MarshalAs(UnmanagedType.I4)]
    public StereoTargetEyeMask targetEye;
};

// ------------------------------------------------------------------------
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct Vector3Interop
{
    [MarshalAs(UnmanagedType.R4)]
    public float x;
    [MarshalAs(UnmanagedType.R4)]
    public float y;
    [MarshalAs(UnmanagedType.R4)]
    public float z;
}

// ------------------------------------------------------------------------
public enum SVFPlaybackState
{
    Empty = 0,
    Initialized = 1,
    Opened = 2,
    Playing = 3,
    Paused = 4,
    Closed = 5,
    Broken = 6
}

// ------------------------------------------------------------------------
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct SVFColorParameter
{
    [MarshalAs(UnmanagedType.U4)]
    public uint id;
    [MarshalAs(UnmanagedType.R4)]
    public float minValue;
    [MarshalAs(UnmanagedType.R4)]
    public float maxValue;
    [MarshalAs(UnmanagedType.R4)]
    public float defaultValue;
    [MarshalAs(UnmanagedType.R4)]
    public float currentValue;
    [MarshalAs(UnmanagedType.I1)]
    public bool isEnabled;
}

// ------------------------------------------------------------------------
public struct ColorCorrection
{
    public float brightness;
    public float contrast;
    public float saturation;
    public float gamma;
    public float hue;
    public float alpha;
    public float tintR;
    public float tintG;
    public float tintB;
    public bool hasBrightness;
    public bool hasContrast;
    public bool hasSaturation;
    public bool hasGamma;
    public bool hasHue;
    public bool hasAlpha;
    public bool hasTintR;
    public bool hasTintG;
    public bool hasTintB;
}

// ------------------------------------------------------------------------
public enum ColorCorrectionParameter
{
    Brightness,
    Contrast,
    Saturation,
    Gamma,
    Hue,
    Alpha,
    TintR,
    TintG,
    TintB,
}

// ------------------------------------------------------------------------
class SVFPlaybackStateHelper
{
    public static SVFPlaybackState FromInt(int val)
    {
        if (false == Enum.IsDefined(typeof(SVFPlaybackState), val))
        {
            return SVFPlaybackState.Broken;
        }
        return (SVFPlaybackState)Enum.ToObject(typeof(SVFPlaybackState), val);
    }
}

// ------------------------------------------------------------------------
class SVFReaderStateHelper
{
    public static SVFReaderStateInterop FromInt(int val)
    {
        if (false == Enum.IsDefined(typeof(SVFReaderStateInterop), val))
        {
            return SVFReaderStateInterop.Unknown;
        }
        return (SVFReaderStateInterop)Enum.ToObject(typeof(SVFReaderStateInterop), val);
    }
}

// ------------------------------------------------------------------------
public class SVFUnityPluginInterop : IDisposable
{
    private static bool s_logstack = false;

    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetRenderEventFunc")]
    private static extern IntPtr InteropGetRenderEventFunc();
    public void IssuePluginEvent(SVFCameraView camView)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]IssuePluginEvent");
        if (instanceId != InvalidID)
        {
            InteropSetCameraView(instanceId, ref camView);
            int complexId = ((0xFFFF & camView.cameraId) << 16) | (0xFFFF & instanceId);
            GL.IssuePluginEvent(InteropGetRenderEventFunc(), complexId);
        }
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetUnitySpecificSettings")]
    private static extern void InteropSetUnitySpecificSettings(ref UnityConfig unityConfig);


    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "CreateHCapObject")]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern int InteropCreateHCapObject(ref HCapSettingsInterop hcapSettings);
    public void CreateHCapObject(HCapSettingsInterop hcapSettings)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]CreateHCapObject");
        if (instanceId != InvalidID)
        {
            throw new Exception("HCapObject already created");
        }
        instanceId = InteropCreateHCapObject(ref hcapSettings);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "DestroyHCapObject")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropDestroyHCapObject(int instanceID);
    public bool DestroyHCapObject()
    {
        if (s_logstack)
            Logger.Log("[INTEROP]DestroyHCapObject");
        if (instanceId != InvalidID)
        {
            bool res = InteropDestroyHCapObject(instanceId);
            if (res)
            {
                instanceId = InvalidID;
            }
            return res;
        }
        return true;
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "OpenHCapObject")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropOpenHCapObject(int instanceId, string filePath, ref SVFOpenInfo openInfo);
    public bool OpenHCapObject(string filePath, ref SVFOpenInfo openInfo)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]OpenHCapObject");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropOpenHCapObject(instanceId, filePath, ref openInfo);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "CloseHCapObject")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropCloseHCapObject(int instanceId);
    public bool CloseHCapObject()
    {
        if (s_logstack)
            Logger.Log("[INTEROP]CloseHCapObject");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropCloseHCapObject(instanceId);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "PlayHCapObject")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropPlayHCapObject(int instanceId);
    public bool PlayHCapObject()
    {
        if (s_logstack)
            Logger.Log("[INTEROP]PlayHCapObject");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropPlayHCapObject(instanceId);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "BurstPlayHCapObject")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropBurstPlayHCapObject(int instanceId);
    public bool BurstPlayHCapObject()
    {
        if (s_logstack)
            Logger.Log("[INTEROP]BurstPlayHCapObject");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropBurstPlayHCapObject(instanceId);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "PauseHCapObject")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropPauseHCapObject(int instanceId);
    public bool PauseHCapObject()
    {
        if (s_logstack)
            Logger.Log("[INTEROP]PauseHCapObject");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropPauseHCapObject(instanceId);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "RewindHCapObject")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropRewindHCapObject(int instanceId);
    public bool RewindHCapObject()
    {
        if (s_logstack)
            Logger.Log("[INTEROP]RewindHCapObject");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropRewindHCapObject(instanceId);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetHCapState")]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern int InteropGetHCapState(int instanceId);
    public SVFPlaybackState GetHCapState()
    {
        if (s_logstack)
            Logger.Log("[INTEROP]GetHCapState");
        if (instanceId == InvalidID)
        {
            return SVFPlaybackState.Broken;
        }
        int stateCode = InteropGetHCapState(instanceId);
        return SVFPlaybackStateHelper.FromInt(stateCode);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetHCapSVFStatus", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropGetHCapSVFStatus(int instanceId, ref SVFStatusInterop svfStatus);
    public bool GetHCapSVFStatus(ref SVFStatusInterop svfStatus, ref SVFReaderStateInterop svfInternalState)
    {
        if (instanceId == InvalidID)
        {
            return false;
        }
        if (true == InteropGetHCapSVFStatus(instanceId, ref svfStatus))
        {
            svfInternalState = SVFReaderStateHelper.FromInt(svfStatus.lastKnownState);
            return true;
        }
        return false;
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetCameraView")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropSetCameraView(int instanceId, ref SVFCameraView camView);
    public bool SetCameraView(ref SVFCameraView camView)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]SetCameraView");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropSetCameraView(instanceId, ref camView);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetColorCorrectionFloat")]
    private static extern void InteropSetColorCorrectionFloat(int instanceId, uint mask, [MarshalAs(UnmanagedType.I1)]bool enabled, float value);
    public void SetColorCorrection(ref ColorCorrection colorCorrection)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]SetColorCorrection");
        if (instanceId == InvalidID)
        {
            return;
        }
        // these masks correspond to unmanaged ColorCorrectionParameter enum
        const uint maskBrightness = 0x0001;
        const uint maskContrast = 0x0002;
        const uint maskSaturation = 0x0004;
        const uint maskGamma = 0x0008;
        const uint maskHue = 0x0010;
        const uint maskAlpha = 0x0020;
        const uint maskTintR = 0x0040;
        const uint maskTintG = 0x0080;
        const uint maskTintB = 0x0100;

        if(colorCorrection.hasBrightness)
            InteropSetColorCorrectionFloat(instanceId, maskBrightness, colorCorrection.hasBrightness, colorCorrection.brightness);

        if(colorCorrection.hasContrast)
            InteropSetColorCorrectionFloat(instanceId, maskContrast, colorCorrection.hasContrast, colorCorrection.contrast);

        if(colorCorrection.hasSaturation)
            InteropSetColorCorrectionFloat(instanceId, maskSaturation, colorCorrection.hasSaturation, colorCorrection.saturation);

        if(colorCorrection.hasGamma)
            InteropSetColorCorrectionFloat(instanceId, maskGamma, colorCorrection.hasGamma, colorCorrection.gamma);

        if (colorCorrection.hasHue)
            InteropSetColorCorrectionFloat(instanceId, maskHue, colorCorrection.hasHue, colorCorrection.hue);

        if(colorCorrection.hasAlpha)
            InteropSetColorCorrectionFloat(instanceId, maskAlpha, colorCorrection.hasAlpha, colorCorrection.alpha);

        if(colorCorrection.hasTintR)
            InteropSetColorCorrectionFloat(instanceId, maskTintR, colorCorrection.hasTintR, colorCorrection.tintR);

        if(colorCorrection.hasTintG)
            InteropSetColorCorrectionFloat(instanceId, maskTintG, colorCorrection.hasTintG, colorCorrection.tintG);

        if(colorCorrection.hasTintB)
            InteropSetColorCorrectionFloat(instanceId, maskTintB, colorCorrection.hasTintB, colorCorrection.tintB);
    }

    // ------------------------------------------------------------------------
    uint ColorCorrectionParameterToInterop(ColorCorrectionParameter color)
    {
        const uint maskBrightness = 0x0001;
        const uint maskContrast = 0x0002;
        const uint maskSaturation = 0x0004;
        const uint maskGamma = 0x0008;
        const uint maskHue = 0x0010;
        const uint maskAlpha = 0x0020;
        const uint maskTintR = 0x0040;
        const uint maskTintG = 0x0080;
        const uint maskTintB = 0x0100;
        switch(color)
        {
            case ColorCorrectionParameter.Alpha:
                return maskAlpha;
            case ColorCorrectionParameter.Brightness:
                return maskBrightness;
            case ColorCorrectionParameter.Contrast:
                return maskContrast;
            case ColorCorrectionParameter.Gamma:
                return maskGamma;
            case ColorCorrectionParameter.Hue:
                return maskHue;
            case ColorCorrectionParameter.Saturation:
                return maskSaturation;
            case ColorCorrectionParameter.TintB:
                return maskTintB;
            case ColorCorrectionParameter.TintG:
                return maskTintG;
            case ColorCorrectionParameter.TintR:
                return maskTintR;
        }
        return 0;
    }

    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetColorCorrectionParameter")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropGetColorCorrectionParameter(int instanceId, ref SVFColorParameter colorParam);
    public bool GetColorParameter(ColorCorrectionParameter color, out float MinVal, out float MaxVal, out float DefaultVal, out float CurVal, out bool enabled)
    {
        MinVal = 0.0f;
        MaxVal = 0.0f;
        DefaultVal = 0.0f;
        CurVal = 0.0f;
        enabled = false;

        if (s_logstack)
            Logger.Log("[INTEROP]GetColorParameter");
        if (instanceId == InvalidID)
        {
            return false;
        }
        uint colorid = ColorCorrectionParameterToInterop(color);
        if (0 == colorid)
            return false;
        SVFColorParameter colorParam = new SVFColorParameter() { id = colorid, minValue = 0.0f, maxValue = 0.0f, defaultValue = 0.0f, currentValue = 0.0f, isEnabled = false};
        bool fRes = InteropGetColorCorrectionParameter(instanceId, ref colorParam);
        if(fRes)
        {
            enabled = colorParam.isEnabled;
            MinVal = colorParam.minValue;
            MaxVal = colorParam.maxValue;
            DefaultVal = colorParam.defaultValue;
            CurVal = colorParam.currentValue;
        }
        return fRes;
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetColorCorrectionParameter")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropSetColorCorrectionParameter(int instanceId, uint colorParam, float colorValue, [MarshalAs(UnmanagedType.I1)]bool enabled);
    public bool SetColorParameter(ColorCorrectionParameter color, float colorValue, bool enabled)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]SetColorParameter");
        if (instanceId == InvalidID)
        {
            return false;
        }
        uint colorid = ColorCorrectionParameterToInterop(color);
        if (0 == colorid)
            return false;
        return InteropSetColorCorrectionParameter(instanceId, colorid, colorValue, enabled);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetHCapObjectFileInfo")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropGetHCapObjectFileInfo(int instanceId, ref SVFFileInfo info);
    public bool GetHCapObjectFileInfo(ref SVFFileInfo fileInfo)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]GetHCapObjectFileInfo");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropGetHCapObjectFileInfo(instanceId, ref fileInfo);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetHCapObjectFrameInfo")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropGetHCapObjectFrameInfo(int instanceId, ref SVFFrameInfo info);
    public bool GetHCapObjectFrameInfo(ref SVFFrameInfo frameInfo)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]GetHCapObjectFrameInfo");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropGetHCapObjectFrameInfo(instanceId, ref frameInfo);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetSeekRange")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropGetSeekRange(int instanceId, ref ulong frameStart, ref ulong frameEnd);
    public bool GetSeekRange(ref ulong frameStart, ref ulong frameEnd)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]GetSeekRange");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropGetSeekRange(instanceId, ref frameStart, ref frameEnd);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "SeekToFrame")]
    private static extern void InteropSeekToFrame(int instanceId, ulong frameId);
    public void SeekToFrame(ulong frameId)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]SeekToFrame");
        if (instanceId == InvalidID)
        {
            return;
        }
        InteropSeekToFrame(instanceId, frameId);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetHCapObjectAudio3DPosition")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropSetHCapObjectAudio3DPosition(int instanceId, float x, float y, float z);
    public bool SetHCapObjectAudio3DPosition(float x, float y, float z)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]SetHCapObjectAudio3DPosition");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropSetHCapObjectAudio3DPosition(instanceId, x, y, z);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetWireframeMode")]
    private static extern void InteropSetWireframeMode(int instanceId, [MarshalAs(UnmanagedType.I1)]bool isWireframe);
    public void SetWireframeMode(bool isWireframe)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]SetWireframeMode");
        if (instanceId == InvalidID)
        {
            return;
        }
        InteropSetWireframeMode(instanceId, isWireframe);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetCachedMode")]
    private static extern void InteropSetCachedMode(int instanceId, [MarshalAs(UnmanagedType.I1)]bool isCachedMode);
    public void SetCachedMode(bool isCachedMode)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]SetCachedMode");
        if (instanceId == InvalidID)
        {
            return;
        }
        InteropSetCachedMode(instanceId, isCachedMode);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetInternalHCapObjectStateFlags")]
    private static extern void InteropGetInternalHCapObjectStateFlags(int instanceId, [MarshalAs(UnmanagedType.U4)] ref int flags);
    public void GetInternalState(out bool isCachedMode, out bool isLiveMode, out bool isSVFReaderActive, out bool isDiscFlushing)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]GetInternalState");

        isCachedMode = false;
        isLiveMode = false;
        isSVFReaderActive = false;
        isDiscFlushing = false;
        if (instanceId == InvalidID)
        {
            return;
        }
        int flags = 0;
        InteropGetInternalHCapObjectStateFlags(instanceId, ref flags);
        isCachedMode = ((flags & 0x01) != 0);
        isLiveMode = ((flags & 0x02) != 0);
        isDiscFlushing = ((flags & 0x04) != 0);
        isSVFReaderActive = ((flags & 0x08) != 0);
    }


    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetShowNormals")]
    private static extern void InteropSetShowNormals(int instanceId, [MarshalAs(UnmanagedType.I1)]bool showNormals);
    public void SetShowNormals(bool showNormals)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]SetShowNormals");

        if (instanceId == InvalidID)
        {
            return;
        }
        InteropSetShowNormals(instanceId, showNormals);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetShowBBox")]
    private static extern void InteropSetShowBBox(int instanceId, [MarshalAs(UnmanagedType.I1)]bool showBBox);
    public void SetShowBBox(bool showBBox)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]SetShowBBox");

        if (instanceId == InvalidID)
        {
            return;
        }
        InteropSetShowBBox(instanceId, showBBox);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetShowAtlas")]
    private static extern void InteropSetShowAtlas(int instanceId, [MarshalAs(UnmanagedType.I1)]bool showAtlas);
    public void SetShowAtlas(bool showAtlas)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]SetShowAtlas");

        if (instanceId == InvalidID)
        {
            return;
        }
        InteropSetShowAtlas(instanceId, showAtlas);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetClockScale")]
    [return: MarshalAs(UnmanagedType.R4)]
    private static extern float InteropGetClockScale(int instanceId);
    public float GetClockScale()
    {
        if (instanceId == InvalidID)
        {
            return 1.0f;
        }
        return InteropGetClockScale(instanceId);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetClockScale")]
    private static extern void InteropSetClockScale(int instanceId, float scale);
    public void SetClockScale(float scale)
    {
        if (instanceId == InvalidID)
        {
            return;
        }
        InteropSetClockScale(instanceId, scale);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetZBufferInverted")]
    private static extern void InteropSetZBufferInverted([MarshalAs(UnmanagedType.I1)] bool bIsInverted);
    public void SetZBufferInverted(bool bIsInverted)
    {
        InteropSetZBufferInverted(bIsInverted);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "EnableTracing")]
    private static extern void InteropEnableTracing([MarshalAs(UnmanagedType.I1)] bool enable, int level);
    public void EnableTracing(bool enable, LogLevel loglevel)
    {
        if (s_logstack)
            Logger.Log("[INTEROP]EnableTracing");

        int nativeLoggingLevel = 0; // chatty
        switch (loglevel)
        {
            case LogLevel.Critical:
                nativeLoggingLevel = 3; // errors
                break;
            case LogLevel.Verbose:
                nativeLoggingLevel = 0; // chatty
                break;
            case LogLevel.Message:
                nativeLoggingLevel = 1; // normal
                break;
            case LogLevel.None:
                nativeLoggingLevel = 4; // never
                break;
            default:
                nativeLoggingLevel = 2; // errors and warnings
                break;
        }
        InteropEnableTracing(enable, nativeLoggingLevel);
    }

    // ------------------------------------------------------------------------
    [DllImport("SVFUnityPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetNextTraceLine")]
    [return: MarshalAs(UnmanagedType.LPStr)]
    private static extern string InteropGetNextTraceLine();
    private string GetNextTraceLine()
    {
        return InteropGetNextTraceLine();
    }

    public string[] GetTrace()
    {
        if (s_logstack)
            Logger.Log("[INTEROP]GetTrace");

        List<string> traceLines = new List<string>();
        while (true)
        {
            string line = GetNextTraceLine();
            if (false == string.IsNullOrEmpty(line))
            {
                traceLines.Add(line);
            }
            else
            {
                break;
            }
        }
        return traceLines.ToArray();
    }

#if UNITY_EDITOR
    // ------------------------------------------------------------------------
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern IntPtr LoadLibrary(string lpFileName);
#endif

    private const int InvalidID = -1;
    private int instanceId = InvalidID;
    public int InstanceId
    {
        get { return instanceId; }
    }


    // DATA
    private Dictionary<Camera, CommandBuffer> m_Cameras = null;
    // private Dictionary<int, KeyValuePair<Camera, CommandBuffer>> m_Cameras = null;

    // ------------------------------------------------------------------------
    public SVFUnityPluginInterop(UnityConfig unityConfig)
    {
        instanceId = InvalidID;
        m_Cameras = new Dictionary<Camera, CommandBuffer>();
        // m_Cameras = new Dictionary<int, KeyValuePair<Camera, CommandBuffer>>();

#if UNITY_EDITOR
        Logger.Log("Loading SVF");
        // Native dependent assemblies  have to be preloaded before loading SVFUnityPlugin
        LoadLibrary(System.IO.Path.Combine(Application.dataPath, @"Plugins\x64\api-ms-win-core-synch-l1-2-1.dll"));
        LoadLibrary(System.IO.Path.Combine(Application.dataPath, @"Plugins\x64\XAudio2_9.dll"));
        IntPtr lib = LoadLibrary(System.IO.Path.Combine(Application.dataPath, @"Plugins\x64\SVF.dll"));
        if (lib == IntPtr.Zero)
        {
            Logger.LogError("Loading SVF failed");
        }
        EnableTracing(true, LogLevel.Critical);
#endif
        InteropSetUnitySpecificSettings(ref unityConfig);
    }


    private static UnityEngine.Rendering.CameraEvent s_CameraEventToTriggerHCapRender = CameraEvent.BeforeForwardAlpha;
    // ------------------------------------------------------------------------       
    public void SetCommandBufferOnCamera(Camera cam, int cameraId)
    {
        if (s_logstack)
            Logger.Log("[STACK]SetCommandBufferOnCamera");

        if (this.instanceId == InvalidID)
        {
            return;
        }
        // Did we already add the command buffer on this camera? Nothing to do then.
        if (m_Cameras.ContainsKey(cam))
        {
            return;
        }
        // DEBUG
        if (cam.cameraType == CameraType.Game)
        {
            Logger.Log("Seeing game camera for the first time");
        }
        CommandBuffer buf = new CommandBuffer();
        buf.name = string.Format("Render HCap object {0}", this.instanceId);
        m_Cameras[cam] = buf;
        int complexId = ((0xFFFF & cameraId) << 16) | (0xFFFF & instanceId);

        buf.IssuePluginEvent(InteropGetRenderEventFunc(), complexId);

        cam.AddCommandBuffer(s_CameraEventToTriggerHCapRender, buf);
    }

    /*
    public void SetCommandBufferOnCamera(Camera cam, int cameraId)
    {
        if (this.instanceId == InvalidID)
        {
            return;
        }
        // Did we already add the command buffer on this camera? Nothing to do then.
        if (m_Cameras.ContainsKey(cameraId))
        {
            return;
        }
        CommandBuffer buf = new CommandBuffer();
        buf.name = string.Format("Render HCap object {0} channel {1}", this.instanceId, cameraId);
        KeyValuePair<Camera, CommandBuffer> kvp = new KeyValuePair<Camera, CommandBuffer>(cam, buf);
        m_Cameras[cameraId] = kvp;
        int complexId = ((0xFFFF & cameraId) << 16) | (0xFFFF & instanceId);
        buf.IssuePluginEvent(InteropGetRenderEventFunc(), complexId);
        cam.AddCommandBuffer(s_CameraEventToTriggerHCapRender, buf);
    }*/

    // ------------------------------------------------------------------------
    public void CleanupCommandBuffers()
    {
        foreach (var cam in m_Cameras)
        {
            if (null != cam.Key)
            {
                cam.Key.RemoveCommandBuffer(s_CameraEventToTriggerHCapRender, cam.Value);
            }
        }
        m_Cameras.Clear();
    }


    // ------------------------------------------------------------------------
    private bool isInstanceDisposed = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!isInstanceDisposed)
        {
            if (this.instanceId != InvalidID)
            {
                CleanupCommandBuffers();
                DestroyHCapObject();
                this.instanceId = InvalidID;
            }
            isInstanceDisposed = true;
        }
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    ~SVFUnityPluginInterop()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(false);
    }
}
