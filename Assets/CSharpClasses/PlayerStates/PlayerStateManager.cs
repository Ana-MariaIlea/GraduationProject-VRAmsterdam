using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStateManager : MonoBehaviour
{
    public UnityEvent part2Start;
    public UnityEvent part3Start;

    public enum PlayerState
    {
        Part1,
        Part2,
        Part3
    }
  
    public void ChangeStateTo(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Part1:
                break;
            case PlayerState.Part2:
                part2Start?.Invoke();
                break;
            case PlayerState.Part3:
                part3Start?.Invoke();
                break;
        }
    }
}
