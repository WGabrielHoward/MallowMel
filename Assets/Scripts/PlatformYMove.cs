using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlatformMovementY : MonoBehaviour
{
    public float ySpeed = 2f;
    public float distanceY;
    private Vector3 startPoint;
    private Rigidbody2D rigidBod;
    private float yDir = 1f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBod = GetComponent<Rigidbody2D>();
        startPoint = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Bounds();
        Move();
    }

    void Move()
    {
        rigidBod.linearVelocity = new Vector2(0,yDir * ySpeed);
    }

    void Bounds()
    {
        if (distanceY > 0)
        {

            if (this.transform.position.y - startPoint.y > distanceY)
            {
                yDir = -1;
            }
            else if (this.transform.position.y - startPoint.y < 0)
            {
                yDir = 1;
            }
        }
        else if (distanceY < 0)
        {
            if (this.transform.position.y - startPoint.y < distanceY)
            {
                yDir = 1;
            }
            else if (this.transform.position.y - startPoint.y > 0)
            {
                yDir = -1;
            }
        }

        
        
    }
}
