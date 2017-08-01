using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using UnityEngine.VR.WSA;

public class PresentationScript : MonoBehaviour
{
    public HoloVideoObject holoVideoObject = null;
    public GameObject sceneRoot = null;

    public enum clipIndex { Breakers, AcousticMax, BassMax };
    public clipIndex selectedClip = clipIndex.AcousticMax;
    private string[] stClips = new string[] { "Breakers.mp4", "max-acoustic-vocals.mp4", "max-bass.mp4" };

    public double delaySec = 15.0;
    public float clockScale = 1.0f;

    private BoxCollider HvCollider = null;
    private double startTime = 0.0;
    private TextMesh textTimer = null;
    private TextMesh textRate = null;
    private bool startedHV = false;

    public ulong ClipLength = 0;
    public ulong TimeStamp = 0;
    public ulong TimeRemaining = 0;

    public uint FrameId;

    // Use this for initialization
    void Start()
    {
        startTime = Time.realtimeSinceStartup;
   

        TimeStamp = 0;
        ClipLength = 0;

        if (holoVideoObject != null)
        {
            HvCollider = holoVideoObject.gameObject.GetComponent<BoxCollider>();
            holoVideoObject.OnUpdateFrameInfoNotify -= OnFrameInfoEvent;
            holoVideoObject.OnUpdateFrameInfoNotify += OnFrameInfoEvent;
        }

        if (HvCollider == null)
        {
            Logger.LogError("ERROR: cannot find HoloVideoObject BoxCollider object");
        }

        StartCoroutine("DelayedPlayback");
    }



    IEnumerator DelayedPlayback()
    {
        yield return new WaitForSeconds((float)delaySec);
        Debug.Log("Got into delayed playback");
        if (null == holoVideoObject)
        {
            Debug.Log("ERROR: HoloVideo object is NULL");
        }
        else
        {
            Debug.Log("Trying to open " + Application.streamingAssetsPath + "/" + stClips[(int)selectedClip]);
            holoVideoObject.Open(Application.streamingAssetsPath + "/" + stClips[(int)selectedClip]);
            holoVideoObject.SetClockScale(clockScale);

            holoVideoObject.Play();
            startedHV = true;
            ClipLength = holoVideoObject.fileInfo.duration100ns;
        }
    }

    public void OnFrameInfoEvent(HoloVideoObject sender, SVFFrameInfo frameInfo)
    {
        if (HvCollider == null || sender == null)
        {
            return;
        }

        var state = sender.GetCurrentState();
        if (state == SVFPlaybackState.Playing)
        {
            TimeStamp = frameInfo.frameTimestamp;
            TimeRemaining = ClipLength - TimeStamp;
            FrameId = frameInfo.frameId;
            float boundsX = (float)(frameInfo.maxX - frameInfo.minX);
            float boundsY = (float)(frameInfo.maxY - frameInfo.minY);
            float boundsZ = (float)(frameInfo.maxZ - frameInfo.minZ);
            HvCollider.size = new Vector3(boundsX, boundsY, boundsZ);

            float centerX = (float)(frameInfo.maxX + frameInfo.minX) * .5f;
            float centerY = (float)(frameInfo.maxY + frameInfo.minY) * .5f;
            float centerZ = (float)(frameInfo.maxZ + frameInfo.minZ) * .5f;
            HvCollider.center = new Vector3(centerX, centerY, centerZ);
        }
    }
}
