using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.MixedReality.WebRTC;
using Microsoft.MixedReality.WebRTC.Unity;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;

namespace AR9.ARGO.MRWebRTC {
    public class CustomSignaler : Signaler {
        //private WebSocket _signaling;
        public string httpServerAddress = "https://demo.artrebel9.com:9000";
        private WebSocket _signaling;
        private JSONNode myID;
        private JSONNode sdpData;
        private JSONNode sdpAnswerData;
        private bool processSdpOffer = false;
        private bool processSdpAnswer = false;
        public WebRTCClient webRtcClient;
        
        public string localPeerName = "456";
        public string remotePeerName = "321";
        
        public void StartSignaler() {
            var connectToServerTask = ConnectToServerTask();
            initialSdpType = null;
        }
        
        public void StopSignaler() {
            myID = null;
            credentialsSent = false;
            try {
                Debug.Log("About to stop the signaler");
                _signaling.Close();
            }
            catch (Exception ee) {
                Debug.Log("Stopping singaler encountered following error: " + ee);
            }

        }

        protected override void Update() {
            if (myID != null && !credentialsSent) {
                credentialsSent = true;
                SendCredentials();
            }
            
            if (answerTask != null && answerTask.IsCompleted) {
                if(initialSdpType == "over") {
                    Debug.Log("Probably a safe time to start the streams.");
                    webRtcClient.StartLocalVideoStream();
                    webRtcClient.StartRemoteVideoStream();
                    initialSdpType = "over fr";
                }
            }

            base.Update();
        }
        

        public async Task ConnectToServerTask()
        {
            await Task.Run(() =>
            {
                _signaling = new WebSocket (httpServerAddress, "json");
            
                _signaling.SslConfiguration.EnabledSslProtocols = 
                    System.Security.Authentication.SslProtocols.Tls12;

                _signaling.OnError += (sender, e) => {
                    Debug.Log("OnError: " + e.Message);
                };

                _signaling.OnClose += (sender, e) => {
                    Debug.Log("OnClose: " + e.Reason + " code: " + e.Code);
                };

                _signaling.OnMessage += (sender, e) => {
                    // Debug.Log("OnMessage: " + (!e.IsPing ? e.Data : " Received a ping."));
                    try {
                        JSONNode initMessage = JSON.Parse(e.Data);
                        ProcessMessage(initMessage);
                    }
                    catch (Exception ee) {
                        Debug.LogError("OnMessage error: " + ee);
                    }

                    
                };

                _signaling.Connect();

                Debug.Log("Web socket connection is alive: " + _signaling.IsAlive);
            });
        }
        /*
        public async Task ConnectToServerWithWebsocket() {
            
            _signaling = new WebSocket (httpServerAddress, "json");
            
            _signaling.SslConfiguration.EnabledSslProtocols = 
                System.Security.Authentication.SslProtocols.Tls12;

            _signaling.OnError += (sender, e) => {
                Debug.Log("OnError: " + e.Message);
            };

            _signaling.OnClose += (sender, e) => {
                Debug.Log("OnClose: " + e.Reason + " code: " + e.Code);
            };

            _signaling.OnMessage += (sender, e) => {
                // Debug.Log("OnMessage: " + (!e.IsPing ? e.Data : " Received a ping."));
                JSONNode initMessage = JSON.Parse(e.Data);
                ProcessMessage(initMessage);
            };

            _signaling.Connect();

            Debug.Log("Web socket connection is alive: " + _signaling.IsAlive);
            
        }*/

        private void ProcessMessage(JSONNode initMessage) {
            
            switch (initMessage["type"] + "") {
                case "id":
                    // Save ID sent by server after it received credentials
                    myID = initMessage["id"];
                    break;
                    
                case "userlist":
                    // Get list of all active "callable" users
                    // TODO: Relevant for call window
                    break;
                case "video-offer":
                        
                    // Debug.Log("OnMessage: " + (!e.IsPing ? e.Data : " Received a ping."));
                    // Debug.Log("ID: " + initMessage["type"]);
                    sdpData = initMessage["sdp"]["sdp"];
                    _mainThreadWorkQueue.Enqueue(() => StartCoroutine(HandleSdpOffer(sdpData)));
                    // processSdpOffer = true;
                    break;
                case "video-answer":
                    // Debug.Log("OnMessage: " + (!e.IsPing ? e.Data : " Received a ping."));
                        
                    // Debug.Log("ID: " + initMessage["type"]);
                    sdpAnswerData = initMessage["sdp"]["sdp"];
                    _mainThreadWorkQueue.Enqueue(() => StartCoroutine(HandleSdpAnswer(sdpAnswerData)));
                    // processSdpAnswer = true;
                    break;

                case "new-ice-candidate":
                    // Save ICE candidates
                    _nativePeer.AddIceCandidate(ToIceCandidate(initMessage["candidate"]));
                    break;
            }
        }

        private IEnumerator HandleSdpOffer(JSONNode sdpmsg) {
            Debug.Log("Offer Message: " + sdpmsg);
            yield return null;
            // Apply the offer coming from the remote peer to the local peer
            var sdpOffer = new SdpMessage { Type = SdpMessageType.Offer, Content = sdpmsg };
            try {
                PeerConnection.HandleConnectionMessageAsync(sdpOffer).ContinueWith(_ =>
                    {
                        // If the remote description was successfully applied then immediately send
                        // back an answer to the remote peer to acccept the offer.
                        _nativePeer.CreateAnswer();
                        
                    }, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.RunContinuationsAsynchronously);
            }
            catch (Exception ee) {
                Debug.LogError("Encountered the following error while creating answer for offer: " + ee);
            }
            Debug.Log("Processed SDP Offer.");
        }

        private Task answerTask = null;
        private IEnumerator HandleSdpAnswer(JSONNode sdpmsg) {
            Debug.Log("Answer Message: " + sdpmsg);
            yield return null;
            // Apply the offer coming from the remote peer to the local peer
            // var sdpOffer = new SdpMessage { Type = SdpMessageType.Offer, Content = sdpmsg };
            var sdpAnswer = new SdpMessage { Type = SdpMessageType.Answer, Content = sdpmsg };
            answerTask = PeerConnection.HandleConnectionMessageAsync(sdpAnswer);
            Debug.Log("Processed SDP Answer.");
        }

        public IceCandidate ToIceCandidate(JSONNode data) {
            return new IceCandidate  {
                SdpMid = data["sdpMid"],
                SdpMlineIndex = data["sdpMLineIndex"],
                Content = data["candidate"]
            };
        }

        private bool credentialsSent = false;

        private void SendCredentials() {
            JSONObject jsonMessage = new JSONObject();
            jsonMessage.Add("name", localPeerName);
            Debug.Log("Timestamp: " + ConvertToUnixTimestamp(DateTime.Now));
            jsonMessage.Add("date", ConvertToUnixTimestamp(DateTime.Now));
            jsonMessage.Add("id", myID);
            jsonMessage.Add("type", "username");
            
            _signaling.Send(jsonMessage.ToString());
        }

        /// <summary>
        /// "answer" - Received an offer first - has to start a connection after stending the answer
        /// "offer" - Sent the offer first - needs to only respond to the expected offer
        /// "over" - Already answered and offered.
        /// null - Waiting to recieve or send a connection request
        /// </summary>
        [HideInInspector] public string initialSdpType = null;

        public override Task SendMessageAsync(SdpMessage message) {
            // Debug.Log("Sending SDP message.");
            
            JSONObject jsonMessage = new JSONObject();
            jsonMessage.Add("name", localPeerName);
            jsonMessage.Add("target", remotePeerName);
            JSONObject sdpMessage = new JSONObject();
            
            if (message.Type == SdpMessageType.Offer) {
                if (initialSdpType == null) initialSdpType = "offer";
                sdpMessage.Add("type", "offer");
                sdpMessage.Add("sdp", message.Content);
                jsonMessage.Add("type", "video-offer");
                jsonMessage.Add("sdp", sdpMessage);
            } else if (message.Type == SdpMessageType.Answer) {
                if (initialSdpType == null) initialSdpType = "answer";
                sdpMessage.Add("type", "answer");
                sdpMessage.Add("sdp", message.Content);
                jsonMessage.Add("type", "video-answer");
                jsonMessage.Add("sdp", sdpMessage);
            }
            Debug.Log("Sending data to server: " + jsonMessage.ToString());
            return SendMessageImplAsync(jsonMessage);
        }

        public override Task SendMessageAsync(IceCandidate candidate) {
            // Debug.Log("Sending ICE message. ");
            JSONObject jsonMessage = new JSONObject();
            JSONObject data = new JSONObject();
            data.Add("candidate", candidate.Content);
            data.Add("sdpMid", candidate.SdpMid);
            data.Add("sdpMLineIndex", candidate.SdpMlineIndex);
            
            jsonMessage.Add("candidate", data);
            jsonMessage.Add("target", remotePeerName);
            jsonMessage.Add("type", "new-ice-candidate");
            return SendMessageImplAsync(jsonMessage);
        }
        
        private Task SendMessageImplAsync(JSONObject message) {
            
            var tcs = new TaskCompletionSource<bool>();
            _mainThreadWorkQueue.Enqueue(() => { 
                _signaling.Send(message.ToString());
                
                if(initialSdpType == "answer") {
                    Debug.Log("Sending local offer after answering to remote offer.");
                    webRtcClient.StartConnection();
                    initialSdpType = "over";
                }
                
            });
            
            return tcs.Task;
        }
        
        public static double ConvertToUnixTimestamp(DateTime date) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        protected void OnApplicationQuit() {
            StopSignaler();
            Debug.Log("Is alive: " + _signaling.IsAlive);
        }
    }
}
