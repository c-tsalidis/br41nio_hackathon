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
        try {
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
        }
        catch (SocketException e) {
            Debug.LogError(e);
        }

        /*
         // Check out the Unicorn UDP receiver --> https://github.com/unicorn-bi/Unicorn-Suite-Hybrid-Black/blob/master/Unicorn%20.NET%20API/UnicornUDP/UnicornUDPReceiver/Program.cs
         
         // initialize upd socket
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Bind(endPoint);
                byte[] receiveBufferByte = new byte[1024];
                float[] receiveBufferFloat= new float[receiveBufferByte.Length / sizeof(float)];

                //acquisition loop
                while (true)
                {
                    int numberOfBytesReceived = socket.Receive(receiveBufferByte);
                    if (numberOfBytesReceived > 0)
                    {
                        //convert byte array to float array
                        for (int i = 0; i < numberOfBytesReceived / sizeof(float); i++)
                        {
                            receiveBufferFloat[i] = BitConverter.ToSingle(receiveBufferByte, i * sizeof(float));
                            if(i+1< numberOfBytesReceived / sizeof(float))
                                Console.Write("{0},", receiveBufferFloat[i].ToString("n2"));
                            else
                                Console.WriteLine("{0}", receiveBufferFloat[i].ToString("n2"));
                        }
                    }
                }
         */
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