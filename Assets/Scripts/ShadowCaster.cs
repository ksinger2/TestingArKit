using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(Camera))]
public class ShadowCaster : MonoBehaviour {

    [SerializeField]
    private int _shadowMapResolution = 1024;

    private Camera _shadowCamera;
    private Light _light;

    [SerializeField]
    private GameObject _quad;

    [SerializeField]
    private LayerMask _layerMask = 1 << 31;

    private void Start()
    {
        _light = GetComponent<Light>();

        if (_light.type != LightType.Spot)
        {
            Debug.LogWarningFormat("Warning: ShadowCaster only works with ppot lights. ShadowCaster {0} with set light type to Spot.", name);
        }
        _light.type = LightType.Spot;
        _light.shadows = LightShadows.Soft;
        _light.shadowNearPlane = 1.0f;

        _shadowCamera = GetComponent<Camera>();
        RenderTexture texture = new RenderTexture(_shadowMapResolution, _shadowMapResolution, 24);
        _shadowCamera.targetTexture = texture;
        _shadowCamera.cullingMask = _layerMask;
        _shadowCamera.backgroundColor = new Color(1.0f, 0.0f, 1.0f, 0.0f);
        _shadowCamera.fieldOfView = _light.spotAngle;
        _shadowCamera.stereoTargetEye = StereoTargetEyeMask.None;
        
        _quad.transform.SetParent(transform);
        _quad.transform.rotation = new Quaternion();
        _quad.transform.localPosition = Vector3.forward;
        MeshRenderer renderer = _quad.GetComponent<MeshRenderer>();
        renderer.material.mainTexture = texture;

        float scale = 2.0f * Mathf.Tan(Mathf.Deg2Rad * _light.spotAngle / 2.0f);
        _quad.transform.localScale = new Vector3(scale, scale, 1.0f);
    }
}
