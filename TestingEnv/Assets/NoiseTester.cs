using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTester : MonoBehaviour
{
    [SerializeField] NoiseSampler sampler;
    [SerializeField] NoiseSampler sampler2;
    [SerializeField] NoiseSampler sampler3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x,sampler.SampleAt(new Vector2(transform.position.x, 0)),0);
    }
}
