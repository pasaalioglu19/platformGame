using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour
{
    public Transform target;
    private float smoothSpeed = 0.00925f; 
    private bool firstScreenCheck = false;
    private float timer = 0f;
    private float delay = 10f;
    private bool middleCamera = false;
    private bool fixCamera = false;
    private Vector3 mcInfo;
    private float offSet;
    private bool mcFlag = true;

    void Start()
    {
        if (target == null)
        {
            GameObject newTarget = GameObject.FindWithTag("Player");
            if (newTarget != null)
            {
                target = newTarget.transform;
            }
            else
            {
                Debug.LogError("Player not found");
            }
        }
    }
    void Update()
    {
        if (target != null)
        {
            if (target.position.x >= -3 && !firstScreenCheck)
            {
                firstScreenCheck = true;
            }


            if (middleCamera)
            {
                if (mcFlag)
                {
                    transform.position = mcInfo;
                    mcFlag = false;
                }
            }

            else if (fixCamera)
            {
                transform.position = new Vector3(target.position.x + offSet, target.position.y, transform.position.z); 
            }

            else if (firstScreenCheck)
            {
                if (timer <= delay)
                {
                    Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
                    Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                    transform.position = smoothedPosition;
                    smoothSpeed *= 1.006F;
                    timer += Time.deltaTime;
                    if (transform.position.x >= target.position.x - 0.01 && timer > 1.0)
                    {
                        timer = delay + 1;
                    }
                }
                else
                {
                    transform.position = new Vector3(target.position.x, target.position.y, transform.position.z); 
                }
            }
        }
    }
    public void setMiddleCamera(Vector3 coordinates)
    {
        mcInfo = coordinates;
        fixCamera = false;
        middleCamera = true;
    }

    public void setFixCamera(bool type)
    {
        if (!middleCamera)
        {
            fixCamera = true;
            if (type)
            {
                offSet = 0.1835F;
            }
            else
            {
                offSet = -0.1835F;
            }
        }    
    }

    public void setTarget()
    {
        target = null;
    }
}
