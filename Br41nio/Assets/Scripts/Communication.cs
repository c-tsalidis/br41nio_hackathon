using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Communication : MonoBehaviour {
    private const int ListenPort = 1000;
    private UdpClient _listener;
    private IPEndPoint _groupEp;

    // initialize upd socket
    private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

    private byte [] _receiveBufferByte;
    private float [] _receiveBufferFloat;

    private void Start() => StartListener();

    private void Update() {
        try {
            /*
            // Debug.Log("Waiting for broadcast");
            byte[] bytes = _listener.Receive(ref _groupEp);
            var receivedMessage = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            switch (receivedMessage) {
                case "happy": {
                    print("happy");
                    break;
                }
                default: break;
            }

            Debug.Log($"Received broadcast from {_groupEp} :");
            Debug.Log($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
            */
            int numberOfBytesReceived = socket.Receive(_receiveBufferByte);
            if (numberOfBytesReceived > 0) {
                //convert byte array to float array
                for (int i = 0; i < numberOfBytesReceived / sizeof(float); i++) {
                    _receiveBufferFloat[i] = BitConverter.ToSingle(_receiveBufferByte, i * sizeof(float));
                    if (i + 1 < numberOfBytesReceived / sizeof(float))
                        Debug.Log(_receiveBufferFloat[i].ToString("n2"));
                    else
                        Debug.Log(_receiveBufferFloat[i].ToString("n2"));
                }
            }
        }
        catch (SocketException e) {
            Debug.LogError(e);
        }

        // Using .NET udp services --> https://docs.microsoft.com/en-us/dotnet/framework/network-programming/using-udp-services

        // Check out the Unicorn UDP receiver --> https://github.com/unicorn-bi/Unicorn-Suite-Hybrid-Black/blob/master/Unicorn%20.NET%20API/UnicornUDP/UnicornUDPReceiver/Program.cs

        //acquisition loop
        while (true) { }
    }

    private void StartListener() {
        _listener = new UdpClient(ListenPort);
        _groupEp = new IPEndPoint(IPAddress.Any, ListenPort);
        _listener.Client.Blocking = false;

        socket.Bind(_groupEp);
        _receiveBufferByte = new byte[1024];
        _receiveBufferFloat = new float[_receiveBufferByte.Length / sizeof(float)];
    }

    private void OnDisable() => _listener.Close();
    private void OnDestroy() => _listener.Close();

    private void OnApplicationQuit() => _listener.Close();
}