using System;
using System.Collections;
using System.Collections.Generic;
using AR9.ARGO.MRWebRTC;
using Microsoft.MixedReality.WebRTC.Unity;
using UnityEngine;

public class WebRTCClient : MonoBehaviour {

    public Microsoft.MixedReality.WebRTC.Unity.VideoRenderer localVideoRenderer;
    public VideoRenderer remoteVideoRenderer;
    public Microsoft.MixedReality.WebRTC.Unity.VideoTrackSource webcamsource;
    public Microsoft.MixedReality.WebRTC.Unity.VideoReceiver videoReceiver;
    public Microsoft.MixedReality.WebRTC.Unity.PeerConnection peerConnection;
    public CustomSignaler cSignaler;
    public void StartLocalVideoStream() {
        Debug.Log("Starting local video stream.");
        localVideoRenderer.StartRendering(webcamsource.Source);
    }
 
    public void StopLocalVideoStream() {
        Debug.Log("Stopping local video stream.");
        localVideoRenderer.StopRendering(webcamsource.Source);
    }
    
    public void StartRemoteVideoStream() {
        Debug.Log("Starting remote video stream.");
        remoteVideoRenderer.StartRendering(videoReceiver.VideoTrack);
    }
 
    public void StopRemoteVideoStream() {
        remoteVideoRenderer.StopRendering(videoReceiver.VideoTrack);
        Debug.Log("Stopping remote video stream.");
        /*
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = remoteVideoRenderer.gameObject.GetComponent<RenderTexture>();
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = rt;*/
    }

    public void StartConnection() {
        // peerConnection.Peer.PreferredVideoCodec = "H264";
        Debug.Log("Starting connection");
        peerConnection.StartConnection();
        // StartCoroutine(CO_StartStreamsAfterSeconds(3));
    }

    public IEnumerator CO_StartStreamsAfterSeconds(int seconds) {
        yield return new WaitForSeconds(seconds);
        StartLocalVideoStream();
        StartRemoteVideoStream();
        Debug.Log("Started all the stream!!!");
    }

    public void EndConnection() {
        Debug.Log("Ending connection. Uninitializing peer.");
        peerConnection.UninitializePeer();
        // StopRemoteVideoStream();
        // StopLocalVideoStream();
    }

    private void OnDisable() {
        Debug.Log("Disabling webrtc client.");
        peerConnection.UninitializePeer();
        // StopRemoteVideoStream();
        // StopLocalVideoStream();
        Debug.Log("Disabled webrtc client.");
    }

    private void OnEnable() {
        // peerConnection.
    }

    public void StartSignaler() {
        cSignaler.StartSignaler();
    }
    public void StopSignaler() {
        cSignaler.StopSignaler();
    }
    
    protected void OnApplicationQuit() {
        cSignaler.StopSignaler();
        peerConnection.UninitializePeer();
    }

    private void OnDestroy() {
        cSignaler.StopSignaler();
        peerConnection.UninitializePeer();
    }

    public void Start() {
        StartSignaler();
    }
}
