/*****************************************************************************
* Project: Pipesgame
* File   : AudioManager.cs
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
*   23.05.2021	JA	Created
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Beinhaltet Sounds und spielt dies ab.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public AudioClip[] audioClips;
    AudioSource mAudioSource;
    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
        //playAudio(2);
    }
    public void playAudio(int audioNumber)
    {
        mAudioSource.PlayOneShot(audioClips[audioNumber]);
    }
}
