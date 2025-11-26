using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Hacky:
/* only works on x-axis
 * must have y-constraints locked
 * no visual for setup, just a float for distance
 * 
 * 
 * 
*/


public class PlatformMovementX : MonoBehaviour
{
    public float xSpeed = 2f;
    public float distanceX;
    private Vector3 startPoint;
    private Rigidbody2D rigedBod;
    private float xDir = 1f;

    // Start is called before the first frame update
    void Start()
    {
        rigedBod = GetComponent<Rigidbody2D>();
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
        rigedBod.linearVelocity = new Vector2(xDir * xSpeed,0);
    }

    void Bounds()
    {
        if (distanceX > 0)
        {

            if (this.transform.position.x - startPoint.x > distanceX)
            {
                xDir = -1;
            }
            else if (this.transform.position.x - startPoint.x < 0)
            {
                xDir = 1;
            }
        }
        else if (distanceX < 0)
        {
            if (this.transform.position.x - startPoint.x < distanceX)
            {
                xDir = 1;
            }
            else if (this.transform.position.x - startPoint.x > 0)
            {
                xDir = -1;
            }
        }
        
    }
}
