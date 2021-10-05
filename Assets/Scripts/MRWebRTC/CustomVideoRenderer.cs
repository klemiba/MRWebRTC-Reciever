using System;
using Microsoft.MixedReality.WebRTC.Unity;
using UnityEngine;

namespace AR9.ARGO.MRWebRTC {
    public class CustomVideoRenderer : VideoRenderer
    {
        private void OnEnable() {
            Debug.Log("Trying to reset texture");
        }
    }
}
