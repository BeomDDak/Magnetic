using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum GameState
    {
        Playing,
        GameOver,
    }

    public enum Player
    {
        One,
        Two,
        Count,
    }

    public enum PlayerState
    {
        PlayTime,
        Wait,
    }
}