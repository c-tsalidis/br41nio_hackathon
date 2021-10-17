using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Communication : MonoBehaviour {
    private const int ListenPort = 1000;
    private IPEndPoint _groupEp;
    private Socket socket;

    private void Start() => StartListener();

    private void Update() {
        // Check out the Unicorn UDP receiver --> https://github.com/unicorn-bi/Unicorn-Suite-Hybrid-Black/blob/master/Unicorn%20.NET%20API/UnicornUDP/UnicornUDPReceiver/Program.cs
        byte[] receiveBufferByte = new byte[1024];
        float[] receiveBufferFloat = new float[receiveBufferByte.Length / sizeof(float)];
        int numberOfBytesReceived = socket.Receive(receiveBufferByte);
        if (numberOfBytesReceived > 0) {
            byte[] messageByte = new byte[numberOfBytesReceived];
            Array.Copy(receiveBufferByte, messageByte, numberOfBytesReceived);
            string message = System.Text.Encoding.ASCII.GetString(messageByte);
            Debug.Log(message);
        }
    }

    private void StartListener() {
        _groupEp = new IPEndPoint(IPAddress.Any, ListenPort);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) {Blocking = false};
        socket.Bind(_groupEp);
    }

    private void OnDisable() => socket.Close();
    private void OnDestroy() => socket.Close();
    private void OnApplicationQuit() => socket.Close();
}