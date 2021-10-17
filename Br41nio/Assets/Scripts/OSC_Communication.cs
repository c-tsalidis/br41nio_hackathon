using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSC_Communication : MonoBehaviour {
    [SerializeField] private OSC osc;

    /*
    private void Update() {
        OscMessage message = new OscMessage();
         * Keep this while testing
        message.address = "/UpdateXYZ";
        message.values.Add(transform.position.x);
        message.values.Add(transform.position.y);
        message.values.Add(transform.position.z);
        osc.Send(message);
        
    }
    */
    public void SendOSCMessage(string address, float [] messageValues) {
        var message = new OscMessage();
        message.address = address;
        foreach(var m in messageValues) message.values.Add(m);
        osc.Send(message);
        print("Sent new data Via OSC --> " + message.values);
    }

    
}
