//3.6
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame
{
    /// <summary>
    /// library to store values independant from scene
    /// </summary>
    static public class Library
    {
        static public int playerScore { get; set; }

        static public int s_randomSeed = 0;

        static public SortedList<int,string> ScoreList 
                                                = new SortedList<int,string>();

    }
}
