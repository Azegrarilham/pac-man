using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNode : MonoBehaviour
{
    int num = 28;
    public float offset = 0.3f;
    public float currentOffset;
    void Start()
    {
        
        if(gameObject.name == "Node")
        {
            currentOffset = offset;
            for (int i = 0; i < num; i++)
            {
                Instantiate(gameObject, new Vector3(transform.position.x , transform.position.y + currentOffset, 0), Quaternion.identity);
                currentOffset += offset;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
