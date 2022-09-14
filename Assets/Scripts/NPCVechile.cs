using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class NPCVechile : MonoBehaviour
{
    // Start is called before the first frame update
    public PathCreator Pathcreator;
    public float speed;
    float distancetravelled;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distancetravelled += speed * Time.deltaTime;
        transform.position = Pathcreator.path.GetPointAtDistance(distancetravelled);
        transform.rotation = Pathcreator.path.GetRotationAtDistance(distancetravelled);
    }
}
