using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Communication : MonoBehaviour {
    private const int ListenPort = 1000;
    private IPEndPoint _groupEp;
    private Socket _socket;
    private float _thetaAverage;
    private float _alphaAverage;

    public float ThetaAverage {
        get => _thetaAverage;
        set => _thetaAverage = value;
    }

    public float AlphaAverage {
        get => _alphaAverage;
        set => _alphaAverage = value;
    }

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

    private void Start() => StartListener();

    private void Update() {
        // Check out the Unicorn UDP receiver --> https://github.com/unicorn-bi/Unicorn-Suite-Hybrid-Black/blob/master/Unicorn%20.NET%20API/UnicornUDP/UnicornUDPReceiver/Program.cs
        byte[] receiveBufferByte = new byte[1024];
        int numberOfBytesReceived = _socket.Receive(receiveBufferByte);
        if (numberOfBytesReceived > 0) {
            byte[] messageByte = new byte[numberOfBytesReceived];
            Array.Copy(receiveBufferByte, messageByte, numberOfBytesReceived);
            string message = Encoding.ASCII.GetString(messageByte);
            var split = message.Split(',');
            _thetaAverage = float.Parse(split[58 - 1]); // get the theta averaged
            _alphaAverage = float.Parse(split[59 - 1]); // get the alpha averaged
            Debug.Log(split.Length + " | " + _thetaAverage + " | " + _alphaAverage);
        }
    }

    private void StartListener() {
        _groupEp = new IPEndPoint(IPAddress.Any, ListenPort);
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) {Blocking = false};
        _socket.Bind(_groupEp);
    }

    private void OnDisable() => _socket.Close();
    private void OnDestroy() => _socket.Close();
    private void OnApplicationQuit() => _socket.Close();
}