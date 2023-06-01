using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerafollow : MonoBehaviour
{
    public Transform Mario;
    public Transform leftbound;
    public Transform rightbound;
    private float camWidth, camHeight, levelMinX, levelMaxX;
    public float smoothdampTime = 1.5f;
    private Vector3 smoothdampVelocity = Vector3.zero;
    void Start()
    {
        camHeight = Camera.main.orthographicSize * 2;
        camWidth = camHeight * Camera.main.aspect;
        float leftboundWidth = leftbound.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;
        float rightboundWidth = rightbound.GetComponentInChildren<SpriteRenderer>().bounds.size.x/2;
        levelMinX = leftbound.position.x + leftboundWidth + (camWidth / 2);
        levelMaxX = rightbound.position.x + rightboundWidth + (camWidth / 2);
    }

    // Update is called once per frame
    void Update()
    {
        if(Mario)
        {
            float MarioX = Mathf.Max(levelMinX, Mathf.Min(levelMaxX, Mario.position.x));
            float x = Mathf.SmoothDamp(transform.position.x, MarioX, ref smoothdampVelocity.x, smoothdampTime);
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
    }
}
