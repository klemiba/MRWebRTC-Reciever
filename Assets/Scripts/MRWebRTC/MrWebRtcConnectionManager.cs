using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.WebRTC;
using Microsoft.MixedReality.WebRTC.Unity;
using UnityEngine;
using PeerConnection = Microsoft.MixedReality.WebRTC.Unity.PeerConnection;

public class MrWebRtcConnectionManager : MonoBehaviour {
    public GameObject mrwebrtcObject;
    public GameObject objectPrefabe;
    public PeerConnection pConnection;
    public void Restart() {
        StartCoroutine(Co_Restart());
    }

    public IEnumerator Co_Restart() {
        // mrwebrtcObject.GetComponent<WebRTCClient>().EndConnection();
        
        // Debug.Log("Disable signaler.");
        mrwebrtcObject.GetComponent<WebRTCClient>().cSignaler.initialSdpType = null;
        // Debug.Log("Calling stop signaler.");
        // mrwebrtcObject.GetComponent<WebRTCClient>().StopSignaler();
        // Debug.Log("Stopped signaler.");

        // Debug.Log("Disabling peer connection");
        mrwebrtcObject.GetComponent<WebRTCClient>().peerConnection.gameObject.SetActive(false);
        mrwebrtcObject.GetComponent<WebRTCClient>().StopSignaler();
        mrwebrtcObject.GetComponent<WebRTCClient>().cSignaler.gameObject.SetActive(false);
        
        Debug.Log("Stopped the connection");
        
        yield return new WaitForSeconds(3);
        
        // Debug.Log("Restarting the connection");
        mrwebrtcObject.GetComponent<WebRTCClient>().cSignaler.gameObject.SetActive(true);
        mrwebrtcObject.GetComponent<WebRTCClient>().StartSignaler();
        mrwebrtcObject.GetComponent<WebRTCClient>().peerConnection.gameObject.SetActive(true);
        // mrwebrtcObject.GetComponent<WebRTCClient>().StartLocalVideoStream();
        Debug.Log("Restarted the connection");
    }

}
