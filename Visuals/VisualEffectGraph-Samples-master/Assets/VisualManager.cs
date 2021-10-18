using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualManager : MonoBehaviour
{

    [SerializeField] private Transform camera;
    [SerializeField] private GameObject visualEffectObject, light;
    [SerializeField] private float speed = 0.5f;
    public float spawnRateNeurons = 1.0f, spawnRateLights = 1.0f;
    [SerializeField] private float distanceToSpawn = 50.0f;
    private Dictionary<GameObject, float> ribbons = new Dictionary<GameObject, float>();
    [SerializeField] private float horizontalDistance = 1000;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRibbon());
        //StartCoroutine(SpawnLight());
    }

    // Update is called once per frame
    void Update()
    {
        camera.position += Vector3.forward * speed;

        //foreach (var item in ribbons.Values)
        //{
        //    if (camera.position.z < item)
        //    {
        //        var thing = ribbons.;
        //        ribbons.Remove(thing);
        //        Destroy(thing);
                
        //    }// particle z pos > camera positem.Value
        //}
    }

    // neuronRate (0.2-1) , float directionalLightTemperature (), float spawnDistance (100-300), float cameraSpeed
    public void SetParameters(float neuronRate, float directionalLightTemperature, float spawnDistance, float cameraSpeed)
    {
        spawnRateNeurons = neuronRate;
        light.GetComponent<Light>().colorTemperature = directionalLightTemperature;
        distanceToSpawn = spawnDistance;
        speed = cameraSpeed;
    }



    IEnumerator SpawnRibbon()
    {

        yield return new WaitForSeconds(1 / spawnRateNeurons);
        var pos = camera.transform.position + Vector3.forward * Random.Range(0, distanceToSpawn) + Vector3.left * Random.Range(-horizontalDistance, horizontalDistance) + Vector3.up * Random.Range(-horizontalDistance, horizontalDistance);
        var ribbon = Instantiate(visualEffectObject, pos, Quaternion.identity);
        ribbons.Add(ribbon, ribbon.transform.position.z);
        StartCoroutine(SpawnRibbon());
    }

    IEnumerator SpawnLight()
    {

        yield return new WaitForSeconds(1 / spawnRateLights);
        var pos = camera.transform.position + Vector3.forward * Random.Range(0, distanceToSpawn) + Vector3.left * Random.Range(-horizontalDistance, horizontalDistance) + Vector3.up * Random.Range(-horizontalDistance, horizontalDistance);
        var L = Instantiate(light, pos, Quaternion.identity);

        StartCoroutine(SpawnLight());
    }

}
