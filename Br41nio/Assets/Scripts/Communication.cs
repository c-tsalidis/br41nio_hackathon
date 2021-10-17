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
    public float [] mappedBaselines = new float[7];
    public float [] frequencyBandsDifferences = new float[7];


    public float frequencyBandMinValue = -100, frequencyBandMaxValue = 100;
    public bool baselineWaitingTimeFinished = false;
    public int _baseLineCounter = 0;

    [Space]

    [Header("OSC communication")]
    [SerializeField] private OSC_Communication OSC_Communication;
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
                        // CalculateDifferenceToBaseline();
                        // print(GetMappedDifferences(0, 10));
                        // possibly multiply by a certain ratio
                        OSC_Communication.SendOSCMessage("/freqs", GetMappedDifferences(0, 100));
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
            frequencyBandsBaselines[i] = frequencyBandsBaselines[i] / _baseLineCounter; // get average for baseline
        }
    }

    // private void CalculateDifferenceToBaseline() {
    //     for (int i = 0; i < frequencyBandsDifferences.Length; i++) {
    //         frequencyBandsDifferences[i] = Mathf.Abs(frequencyBandsBaselines[i] - frequencyBandsAverages[i]);
    //     }
    // }

    public float [] GetMappedDifferences(float minValue, float maxValue) {
        float [] mappedDifferences = new float[7];
        // map  the baselines
        for (int i = 0; i < frequencyBandsBaselines.Length; i++) {
            mappedBaselines[i] = Map(frequencyBandsBaselines[i], frequencyBandMinValue, frequencyBandMaxValue, minValue, maxValue); // get average for baseline
        }
        // map the current frequency band differences
        for (int i = 0; i < frequencyBandsAverages.Length; i++) {
            mappedDifferences[i] = Mathf.Abs(mappedBaselines[i] - Map(frequencyBandsAverages[i], frequencyBandMinValue, frequencyBandMaxValue, minValue, maxValue)); // get average for baseline
        }

        return mappedDifferences;
    }
    
    /// <summary>
    /// Map a value from one interval to another interval.
    /// </summary>
    /// <param name="value">Value to map</param>
    /// <param name="min1">Minimum value of the first interval</param>
    /// <param name="max1">Maximum value of the second interval</param>
    /// <param name="min2">Minimum value of the second interval</param>
    /// <param name="max2">Maximum value of the second interval</param>
    public float Map(float value, float min1, float max1, float min2, float max2) {
        return min2 + (max2 - min2) * ((value - min1) / (max1 - min1));
    }

    private void OnDisable() => _socket.Close();
    private void OnDestroy() => _socket.Close();
    private void OnApplicationQuit() => _socket.Close();
}