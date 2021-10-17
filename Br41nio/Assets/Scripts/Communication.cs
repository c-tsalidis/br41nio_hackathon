using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Communication : MonoBehaviour {
    private const int ListenPort = 1000;
   // private UdpClient _listener;
    private IPEndPoint _groupEp;

    private Socket socket;

    private void Start() => StartListener();

    private void Update() {
        byte[] receiveBufferByte = new byte[1024];
        float[] receiveBufferFloat = new float[receiveBufferByte.Length / sizeof(float)];
        int numberOfBytesReceived = socket.Receive(receiveBufferByte);
        if (numberOfBytesReceived > 0) {
            //convert byte array to float array
            for (int i = 0; i < numberOfBytesReceived / sizeof(float); i++) {
                receiveBufferFloat[i] = BitConverter.ToSingle(receiveBufferByte, i * sizeof(float));
                Debug.Log(receiveBufferFloat[i].ToString("n2"));
            }
        }
        /*
        byte[] bytes = _listener.Receive(ref _groupEp);
        try {
            if (bytes.Length > 0) {
                //convert byte array to float array
                for (int i = 0; i < bytes.Length / sizeof(float); i++) {
                    receiveBufferFloat[i] = BitConverter.ToSingle(receiveBufferByte, i * sizeof(float));
                    Debug.Log(receiveBufferFloat[i].ToString("n2"));
                }
            }

            Debug.Log("Received broadcast from " + _groupEp);
            Debug.Log(Encoding.ASCII.GetString(bytes, 0, bytes.Length));
        }
        catch (SocketException e) {
            // Debug.LogError(e);
            Application.Quit();
        }
        */

        // Check out the Unicorn UDP receiver --> https://github.com/unicorn-bi/Unicorn-Suite-Hybrid-Black/blob/master/Unicorn%20.NET%20API/UnicornUDP/UnicornUDPReceiver/Program.cs
    }

    private void StartListener() {
        // _listener = new UdpClient(ListenPort);
        _groupEp = new IPEndPoint(IPAddress.Any, ListenPort);
        //_listener.Client.Blocking = false;
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) {Blocking = false};
        socket.Bind(_groupEp);
    }

    //private void OnDisable() => _listener.Close();
    //private void OnDestroy() => _listener.Close();

    //private void OnApplicationQuit() => _listener.Close();
}