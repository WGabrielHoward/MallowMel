using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMotSwap : MonoBehaviour
{
    private SliderJoint2D sj;
    private JointMotor2D jm;
    private float startSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        sj = GetComponent<SliderJoint2D>();
        jm = sj.motor;
        startSpeed = jm.motorSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.position.x == sj.anchor.x && this.transform.position.y== sj.anchor.y)
        {
            jm.motorSpeed= startSpeed;
            Debug.Log("LowerLimit");
        }
        else if(this.transform.position.x == sj.connectedAnchor.x && this.transform.position.y == sj.connectedAnchor.y)
        {
            jm.motorSpeed =-startSpeed;
            Debug.Log("UpperLimit");
        }
        Debug.Log("unchanged");
    }
}
