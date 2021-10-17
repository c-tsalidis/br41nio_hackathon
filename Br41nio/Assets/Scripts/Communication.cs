/*
using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Communication : MonoBehaviour {
    private const int ListenPort = 11000;
    private UdpClient _listener;
    private IPEndPoint _groupEp;

    private void Start() => StartListener();

    private void Update() {
        byte[] bytes = _listener.Receive(ref _groupEp);
        byte[] receiveBufferByte = new byte[1024];
        float[] receiveBufferFloat = new float[receiveBufferByte.Length / sizeof(float)];
        try {
            if (bytes.Length > 0) {
                //convert byte array to float array
                for (int i = 0; i < bytes.Length / sizeof(float); i++) {
                    receiveBufferFloat[i] = BitConverter.ToSingle(receiveBufferByte, i * sizeof(float));
                    //if (i + 1 < bytes.Length / sizeof(float))
                    //    Debug.Log(receiveBufferFloat[i].ToString("n2"));
                    //else
                        Debug.Log(receiveBufferFloat[i].ToString("n2"));
                }
            }

            Debug.Log("Received broadcast from " + _groupEp);
            Debug.Log(Encoding.ASCII.GetString(bytes, 0, bytes.Length));
        }
        catch (SocketException e) {
            Debug.LogError(e);
            Application.Quit();
        }

        // Check out the Unicorn UDP receiver --> https://github.com/unicorn-bi/Unicorn-Suite-Hybrid-Black/blob/master/Unicorn%20.NET%20API/UnicornUDP/UnicornUDPReceiver/Program.cs
    }

    private void StartListener() {
        _listener = new UdpClient(ListenPort);
        _groupEp = new IPEndPoint(IPAddress.Any, ListenPort);
        _listener.Client.Blocking = false;
    }

    private void OnDisable() => _listener.Close();
    private void OnDestroy() => _listener.Close();

    private void OnApplicationQuit() => _listener.Close();
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class Communication : MonoBehaviour {
    private const int listenPort = 11000;
    private UdpClient listener;
    private IPEndPoint groupEP;
    
    private void Start() {
        StartListener();
    }

    private void Update() {
        try {
            // Debug.Log("Waiting for broadcast");
            byte[] bytes = listener.Receive(ref groupEP);
            var receivedMessage = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            print(receivedMessage);
            byte[] receiveBufferByte = new byte[1024];
            float[] receiveBufferFloat = new float[receiveBufferByte.Length / sizeof(float)];
            try {
                if (bytes.Length > 0) {
                    //convert byte array to float array
                    for (int i = 0; i < bytes.Length / sizeof(float); i++) {
                        receiveBufferFloat[i] = BitConverter.ToSingle(receiveBufferByte, i * sizeof(float));
                        //if (i + 1 < bytes.Length / sizeof(float))
                        //    Debug.Log(receiveBufferFloat[i].ToString("n2"));
                        //else
                        Debug.Log(receiveBufferFloat[i].ToString("n2"));
                    }
                }

                Debug.Log("Received broadcast from " + groupEP);
                Debug.Log(Encoding.ASCII.GetString(bytes, 0, bytes.Length));
            }
            catch (SocketException e) {
                Debug.LogError(e);
                Application.Quit();
            }
            Debug.Log($"Received broadcast from {groupEP} :");
            Debug.Log($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
        }
        catch (SocketException e) {
            // Debug.LogError(e);
        }
    }

    private void StartListener() {
        listener = new UdpClient(listenPort);
        groupEP = new IPEndPoint(IPAddress.Any, listenPort);
        listener.Client.Blocking = false;
    }

    private void OnDisable() => listener.Close();
    private void OnDestroy() => listener.Close();

    private void OnApplicationQuit() => listener.Close();
}