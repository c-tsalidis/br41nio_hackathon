using System;
using System.Collections;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEditor.UIElements;

public class Communication : MonoBehaviour {
    
    [Header("UDP networking")]
    private const int ListenPort = 1000;
    private IPEndPoint _groupEp;
    private Socket _socket;
    
    [Space]
    
    [Header("Frequency bands")]
    public float [] frequencyBandsAverages = new float[7];
    public float [] frequencyBandsBaselines = new float[7];
    public float [] frequencyBandsDifferences = new float[7];

    public bool baselineWaitingTimeFinished = false;
    public int _baseLineCounter = 0;

    /*
    Unicorn EEG powerband .NET API averaged values
    1: delta channel 1
    [...]
    57: delta channel 1-8 averaged
    58: theta channel 1-8 averaged
    59: alpha channel 1-8 averaged
    60: beta low channel 1-8 averaged
    61: beta mid channel 1-8 averaged
    62: beta high channel 1-8 averaged
    63: gamma channel 1-8 averaged
     */

    private void Start() {
        StartListener();
        StartCoroutine(GetValuesForBaseline());
    }

    private void Update() {
        // Check out the Unicorn UDP receiver --> https://github.com/unicorn-bi/Unicorn-Suite-Hybrid-Black/blob/master/Unicorn%20.NET%20API/UnicornUDP/UnicornUDPReceiver/Program.cs
        try {
            byte[] receiveBufferByte = new byte[1024];
            int numberOfBytesReceived = _socket.Receive(receiveBufferByte);
            if (numberOfBytesReceived > 0) {
                byte[] messageByte = new byte[numberOfBytesReceived];
                Array.Copy(receiveBufferByte, messageByte, numberOfBytesReceived);
                string message = Encoding.ASCII.GetString(messageByte);
                var split = message.Split(',');
                var counter = 0;
                for (int i = 56; i < 63; i++) {
                    if(baselineWaitingTimeFinished) {
                        frequencyBandsAverages[counter] = float.Parse(split[i]);
                        CalculateDifferenceToBaseline();
                    }
                    else frequencyBandsBaselines[counter] += float.Parse(split[i]);
                    counter++;
                }
                if(!baselineWaitingTimeFinished) _baseLineCounter++;
            }
        }
        catch (Exception e) {
            // Debug.Log(e);
        }
        
    }

    private void StartListener() {
        _groupEp = new IPEndPoint(IPAddress.Any, ListenPort);
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) {Blocking = false};
        _socket.Bind(_groupEp);
    }

    private IEnumerator GetValuesForBaseline() {
        yield return new WaitForSeconds(5.0f);
        CalculateBaseline();
        baselineWaitingTimeFinished = true;
    }

    private void CalculateBaseline() {
        for (int i = 0; i < frequencyBandsBaselines.Length; i++) {
            frequencyBandsBaselines[i] = frequencyBandsBaselines[i] / _baseLineCounter;
        }
    }

    private void CalculateDifferenceToBaseline() {
        for (int i = 0; i < frequencyBandsDifferences.Length; i++) {
            frequencyBandsDifferences[i] = Mathf.Abs(frequencyBandsBaselines[i] - frequencyBandsAverages[i]);
        }
    }

    private void OnDisable() => _socket.Close();
    private void OnDestroy() => _socket.Close();
    private void OnApplicationQuit() => _socket.Close();
}