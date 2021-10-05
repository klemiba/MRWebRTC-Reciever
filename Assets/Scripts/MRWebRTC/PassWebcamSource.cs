using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.WebRTC;
using UnityEngine;
using PeerConnection = Microsoft.MixedReality.WebRTC.Unity.PeerConnection;

public class PassWebcamSource : MonoBehaviour
{
    public Microsoft.MixedReality.WebRTC.Unity.VideoRenderer videoRenderer;
    public Microsoft.MixedReality.WebRTC.Unity.VideoTrackSource webcamsource;
    public Microsoft.MixedReality.WebRTC.Unity.VideoReceiver videoReceiver;
    public Microsoft.MixedReality.WebRTC.Unity.PeerConnection peerConnection;
    public void StartVideoStream()
    {
        videoRenderer.StartRendering(webcamsource.Source);
    }
 
    public void StopVideoStream()
    {
        videoRenderer.StopRendering(webcamsource.Source);
    }
    
    public void StartRemoteVideoStream() {
        videoRenderer.StartRendering(videoReceiver.VideoTrack);
    }
 
    public void StopRemoteVideoStream()
    {
        videoRenderer.StopRendering(videoReceiver.VideoTrack);
    }

    public void StartConnection() {
        peerConnection.StartConnection();
    }
}
