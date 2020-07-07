using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    public enum States { Forward, Right, Left, Backward, Aim, Back, Stand};

    States playerState = States.Stand;

    public States PlayerState
    {
        get
        {
            return playerState;
        }
    }

    public void SetForward()
    {
        playerState = States.Forward;
    }
    public void SetRight()
    {
        playerState = States.Right;
    }
    public void SetLeft()
    {
        playerState = States.Left;
    }
    public void SetBackward()
    {
        playerState = States.Backward;
    }
    public void SetAim()
    {
        playerState = States.Aim;
    }
    public void SetBack()
    {
        playerState = States.Back;
    }
    public void SetStand()
    {
        playerState = States.Stand;
    }

}
