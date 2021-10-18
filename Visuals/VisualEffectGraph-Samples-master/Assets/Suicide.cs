using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suicide : MonoBehaviour
{
    [SerializeField] private Transform camera;

    private void Start()
    {
        camera = Camera.main.transform;
    }
    private void Update()
    {
        if(transform.position.z < camera.position.z - 20)
        {
            Destroy(this.gameObject);
        }
    }
}
