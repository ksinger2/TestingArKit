using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum TargetDevices { AutoDetect, PC, Hololens, AppStore, Vive, Oculus };

delegate void SetupMvp(Matrix4x4 m, Matrix4x4 v, Matrix4x4 p);

public class CameraView
{
    private SetupMvp setupMVP;

    public int cameraId
    {
        get; private set;
    }
    public float viewportWidth
    {
        get; private set;
    }
    public float viewportHeight
    {
        get; private set;
    }

    public bool isGameCamera
    {
        get; private set;
    }

    public bool isStereoscopic
    {
        get; private set;
    }

    public StereoTargetEyeMask targetEye
    {
        get; private set;
    }

    public Matrix4x4 MVP
    {
        get; private set;
    }

    private bool leftHhandedness = true;
    private float flipFactor = 1.0f;

    private SVFCameraView _svfCamView;
    public SVFCameraView InteropCamView
    {
        get
        {
            return _svfCamView;
        }
    }

    public CameraView(int id, Camera cam, TargetDevices tgt, bool flipHand)
    {
        cameraId = id;

        viewportWidth = (float)cam.pixelWidth;
        viewportHeight = (float)cam.pixelHeight;
        isGameCamera = (cam.cameraType == CameraType.Game);

        // Handle handedness flip
        if (flipHand)
        {
            leftHhandedness = !isGameCamera;
            flipFactor = -1.0f;
        }
        else
        {
            leftHhandedness = isGameCamera;
            flipFactor = 1.0f;
        }

        if (tgt == TargetDevices.AutoDetect)
        {
            AutoDetectDevice(flipHand);
        }
        else
        {
            ConfigureSetupMVP(tgt, flipHand);
        }

        isStereoscopic = cam.stereoEnabled;
        if (cam.stereoEnabled)
        {
            if (cam.stereoTargetEye != StereoTargetEyeMask.Left && cam.stereoTargetEye != StereoTargetEyeMask.Right)
            {
                Logger.LogError("ERROR: stereo camera must be either Left or Right, cannot be 'None' or 'Both'");
                throw new Exception("Fix the camera settings");
            }
        }
        targetEye = (false == cam.stereoEnabled) ? StereoTargetEyeMask.Left : cam.stereoTargetEye;
        MVP = new Matrix4x4();
        // When isGameCamera is true the plugin does backface culling and front face culling when false.  
        _svfCamView = new SVFCameraView() { cameraId = cameraId, isGameCamera = leftHhandedness, isStereoscopic = isStereoscopic, viewportWidth = viewportWidth, viewportHeight = viewportHeight, targetEye = targetEye };
    }

    public void AutoDetectDevice(bool flipHand)
    {
        TargetDevices tgt = TargetDevices.AutoDetect;
        if (UnityEngine.XR.XRSettings.enabled)
        {
            string stDevice = UnityEngine.XR.XRSettings.loadedDeviceName.ToLower();
            if (stDevice == "oculus")
            {
                tgt = TargetDevices.Oculus;
            }
            else if (stDevice == "openvr")
            {
                tgt = TargetDevices.Vive;
            }
        }
        ConfigureSetupMVP(tgt, flipHand);
    }

    public void ConfigureSetupMVP(TargetDevices tgt, bool flipHand)
    {
        switch (tgt)
        {
            case TargetDevices.Vive:
                leftHhandedness = !flipHand;
                setupMVP += SetupMVPVive;
                break;

            case TargetDevices.Oculus:
                leftHhandedness = !flipHand;
                setupMVP += SetupMVPOculus;
                break;

            case TargetDevices.AutoDetect:
            case TargetDevices.AppStore:
            case TargetDevices.Hololens:
            case TargetDevices.PC:
            default:
                setupMVP += SetupMVPDefault;
                break;
        }
    }

    public void Update(Camera cam, Matrix4x4 m)
    {
        viewportWidth = (float)cam.pixelWidth;
        viewportHeight = (float)cam.pixelHeight;
        _svfCamView.viewportWidth = viewportWidth;
        _svfCamView.viewportHeight = viewportHeight;

        Matrix4x4 v = cam.worldToCameraMatrix;
        Matrix4x4 pCam = cam.projectionMatrix;
        Matrix4x4 p = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);

        //**********************************************************************
        // Apparently there is an issue with Unity when using a custom shader where
        // the output is being treated as a left-handed coordinate system. This is
        // an known bug when using certain rendering effects, but appears to be the same
        // issue when using custom vertex shaders. Flipping the coordinate system (matrix q)
        // so that the final image is flipped upside down seems to fix the issue for
        // scene and preview cameras.
        //**********************************************************************
        setupMVP(m, v, p);

        _svfCamView.m00 = MVP.m00; _svfCamView.m01 = MVP.m01; _svfCamView.m02 = MVP.m02; _svfCamView.m03 = MVP.m03;
        _svfCamView.m10 = MVP.m10; _svfCamView.m11 = MVP.m11; _svfCamView.m12 = MVP.m12; _svfCamView.m13 = MVP.m13;
        _svfCamView.m20 = MVP.m20; _svfCamView.m21 = MVP.m21; _svfCamView.m22 = MVP.m22; _svfCamView.m23 = MVP.m23;
        _svfCamView.m30 = MVP.m30; _svfCamView.m31 = MVP.m31; _svfCamView.m32 = MVP.m32; _svfCamView.m33 = MVP.m33;
    }

    public void UpdateWithVR(UnityEngine.XR.XRNode vrnode, Matrix4x4 m)
    {
        Vector3 vrPos = UnityEngine.XR.InputTracking.GetLocalPosition(vrnode);
        Quaternion vrRot = UnityEngine.XR.InputTracking.GetLocalRotation(vrnode);
        Vector3 vrScale = new Vector3(1, 1, 1);
        Matrix4x4 vrTransform = Matrix4x4.TRS(vrPos, vrRot, vrScale);

        MVP = vrTransform * m;

        _svfCamView.m00 = MVP.m00; _svfCamView.m01 = MVP.m01; _svfCamView.m02 = MVP.m02; _svfCamView.m03 = MVP.m03;
        _svfCamView.m10 = MVP.m10; _svfCamView.m11 = MVP.m11; _svfCamView.m12 = MVP.m12; _svfCamView.m13 = MVP.m13;
        _svfCamView.m20 = MVP.m20; _svfCamView.m21 = MVP.m21; _svfCamView.m22 = MVP.m22; _svfCamView.m23 = MVP.m23;
        _svfCamView.m30 = MVP.m30; _svfCamView.m31 = MVP.m31; _svfCamView.m32 = MVP.m32; _svfCamView.m33 = MVP.m33;
    }

    private void SetupMVPDefault(Matrix4x4 m, Matrix4x4 v, Matrix4x4 p)
    {
        Matrix4x4 q = new Matrix4x4();
        Matrix4x4 q3 = new Matrix4x4();
        q = Matrix4x4.identity;
        q3 = Matrix4x4.identity;
        q3[0, 0] = flipFactor;
        if (!isGameCamera)
        {
            q[1, 1] = -1.0f;
        }
        MVP = q * p * v * q3 * m;
    }

    private void SetupMVPVive(Matrix4x4 m, Matrix4x4 v, Matrix4x4 p)
    {
        Matrix4x4 q = new Matrix4x4();
        Matrix4x4 q3 = new Matrix4x4();
        q = Matrix4x4.identity;
        q3 = Matrix4x4.identity;
        q[1, 1] = -1.0f;
        q3[0, 0] = -1.0f * flipFactor;
        MVP = q * p * v * q3 * m;
    }

    private void SetupMVPOculus(Matrix4x4 m, Matrix4x4 v, Matrix4x4 p)
    {
        Matrix4x4 q = new Matrix4x4();
        Matrix4x4 q3 = new Matrix4x4();
        q = Matrix4x4.identity;
        q3 = Matrix4x4.identity;
        q[1, 1] = -1.0f;
        q3[0, 0] = -1.0f * flipFactor;
        MVP = q * p * v * q3 * m;
    }
}

public class CameraViews : Dictionary<Camera, CameraView>
{
    public CameraView Find(Camera cam, TargetDevices tgt, bool flipHand)
    {
        CameraView view = null;
        if (false == this.ContainsKey(cam))
        {
            view = new CameraView(this.Count(), cam, tgt, flipHand);
            Add(cam, view);
        }
        view = base[cam];
        return view;
    }

    public void FlushAll()
    {
        foreach (var kvp in this)
        {
            kvp.Key.RemoveAllCommandBuffers();
        }
        Clear();
    }
}
