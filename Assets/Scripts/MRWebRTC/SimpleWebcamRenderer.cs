using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWebcamRenderer : MonoBehaviour
{
    public GameObject videoScreen;
    private WebCamTexture backCamera;
    // Use this for initialization
    void Start () {
        backCamera = new WebCamTexture();
        videoScreen.GetComponent<Renderer>().material.mainTexture = backCamera;
        backCamera.Play();
    }
}
