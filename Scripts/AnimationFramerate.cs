/*****************************************************************************
* Project: Pipesgame
* File   : WaterflowManager.cs
* Date   : 28.05.2021
* Author : Jan Apsel (JA)
*
* These coded instructions, statements, and computer programs contain
* proprietary information of the author and are protected by Federal
* copyright law. They may not be disclosed to third parties or copied
* or duplicated in any form, in whole or in part, without the prior
* written consent of the author.
*
* History:
*   18.05.2021	JA	Created
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MainGame
{
    /// <summary>
    /// Globale Framerate die sich wiederholende Funktionen invoked.
    /// </summary>
    public class AnimationFramerate : MonoBehaviour
    {
        //[System.NonSerialized]
        public UnityEvent animationFrame;
        float animFrameRate;

        void Start()
        {
            animFrameRate = Database.s_animFrameRate;
            StartCoroutine(InvokeInterval());
        }
        IEnumerator InvokeInterval()
        {
            while (true)
            {
                animationFrame.Invoke();
                yield return new WaitForSeconds(animFrameRate);
            }
        }
    }
}
