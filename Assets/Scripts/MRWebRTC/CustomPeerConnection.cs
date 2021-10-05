using System;
using System.Collections.Generic;
using Microsoft.MixedReality.WebRTC;
using UnityEngine;
using PeerConnection = Microsoft.MixedReality.WebRTC.Unity.PeerConnection;

namespace AR9.ARGO.MRWebRTC
{
    public class CustomPeerConnection : PeerConnection
    {
    /*
        private PeerConnection _nativePeer;
        
        public new bool StartConnection()
        {
            // MediaLine manipulates some MonoBehaviour objects when managing senders and receivers
            EnsureIsMainAppThread();

            if (Peer == null)
            {
                throw new InvalidOperationException("Cannot create an offer with an uninitialized peer.");
            }

            // Batch all changes into a single offer
            AutoCreateOfferOnRenegotiationNeeded = false;

            // Add all new transceivers for local tracks. Since transceivers are only paired by negotiated mid,
            // we need to know which peer sends the offer before adding the transceivers on the offering side only,
            // and then pair them on the receiving side. Otherwise they are duplicated, as the transceiver mid from
            // locally-created transceivers is not negotiated yet, so ApplyRemoteDescriptionAsync() won't be able
            // to find them and will re-create a new set of transceivers, leading to duplicates.
            // So we wait until we know this peer is the offering side, and add transceivers to it right before
            // creating an offer. The remote peer will then match the transceivers by index after it applied the offer,
            // then add any missing one.

            // Update all transceivers, whether previously existing or just created above
            var transceivers = _nativePeer.Transceivers;
            
            int index = 0;
            foreach (var mediaLine in _mediaLines) {
                
                
                // Debug.Log("Medialine source state: " + mediaLine.Source);
                // Debug.Log("Medialine receiver state: " + mediaLine.Receiver);
                if(mediaLine.Receiver != null)
                  continue;
                // Ensure each media line has a transceiver
                Transceiver tr = mediaLine.Transceiver;
                if (tr != null)
                {
                    // Media line already had a transceiver from a previous session negotiation
                    Debug.Assert(tr.MlineIndex >= 0); // associated
                }
                else {
                    Debug.Log("Adding new transceivers.");
                    // Create new transceivers for a media line added since last session negotiation.

                    // Compute the transceiver desired direction based on what the local peer expects, both in terms
                    // of sending and in terms of receiving. Note that this means the remote peer will not be able to
                    // send any data if the local peer did not add a remote source first.
                    // Tracks are not tested explicitly since the local track can be swapped on-the-fly without renegotiation,
                    // and the remote track is generally not added yet at the beginning of the negotiation, but only when
                    // the remote description is applied (so for the offering side, at the end of the exchange when the
                    // answer is received).
                    bool wantsSend = (mediaLine.Source != null);
                    bool wantsRecv = (mediaLine.Receiver != null);
                    // var wantsDir = Transceiver.DirectionFromSendRecv(wantsSend, wantsRecv);
                    var wantsDir = Transceiver.DirectionFromSendRecv(wantsSend, false);
                    var streamIDList = new List<string> {"Test1337" + mediaLine.MediaKind};
                    var settings = new TransceiverInitSettings
                    {
                        StreamIDs = streamIDList,
                        Name = $"mrsw#{index}",
                        InitialDesiredDirection = wantsDir
                    };
                    tr = _nativePeer.AddTransceiver(mediaLine.MediaKind, settings);
                    try
                    {
                        mediaLine.PairTransceiver(tr);
                    }
                    catch (Exception ex)
                    {
                        LogErrorOnMediaLineException(ex, mediaLine, tr);
                    }

                    tr.DesiredDirection = wantsDir;
                    Debug.Log("Local track state: " + mediaLine.LocalTrack.Transceiver);
                }
                Debug.Assert(tr != null);
                Debug.Assert(transceivers[index] == tr);
                ++index;
                // Debug.Log("Number of stream ids: " + tr.StreamIDs.Length);
            }

            foreach (var tr in transceivers) {
                for (int i = 0; i < tr.StreamIDs.Length; i++) {
                    Debug.Log("This is the stream ID: " + tr.StreamIDs[i]);
                    if (tr.StreamIDs[i] == "") {
                        tr.StreamIDs[i] = _mediaLines[i].SenderTrackName;
                        Debug.Log("This is the stream ID after apply: " + tr.StreamIDs[i]);
                    }
                }
            }

            // Create the offer
            AutoCreateOfferOnRenegotiationNeeded = true;
            return _nativePeer.CreateOffer();
        }*/
    }
}
