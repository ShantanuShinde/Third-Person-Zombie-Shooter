using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    
    public void SetRotation(float amount)
    {
        float newAng = transform.eulerAngles.x - amount;
        if(newAng > 270 && newAng < 320) 
        {
            newAng = 320;
        }
        else if(newAng < 180 && newAng > 45 )
        {
            newAng = 45;
        }
        transform.eulerAngles = new Vector3(newAng, transform.eulerAngles.y, transform.eulerAngles.z);
        
    }

    public void ResetRotation()
    {
       transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public float GetAngle()
    {
        return CheckAngle(transform.eulerAngles.x);
    }

    public float CheckAngle(float value)
    {
       
       float angle = value - 180;
        if(angle > 0)
        {
            return angle - 180;
        }
        return angle + 180;
    }

    
}
