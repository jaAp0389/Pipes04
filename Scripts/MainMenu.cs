/*****************************************************************************
* Project: Pipesgame
* File   : AudioManager.cs
* Date   : 04.06.2021
* Author : Jan Apsel (JA)
*
* These coded instructions, statements, and computer programs contain
* proprietary information of the author and are protected by Federal
* copyright law. They may not be disclosed to third parties or copied
* or duplicated in any form, in whole or in part, without the prior
* written consent of the author.
*
* History:
*   04.06.2021	JA	Finished
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Hauptmenu mit Start, Exit und Optionen. Optionen bekommen noch eine 
/// Einstellung für Spielsound und einen Seed für die Levelgeneration.
/// </summary>
public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartButton()
    {
        SceneManager.LoadScene(1);
    }
    public void ExitButton()
    {
        //Von der letzten GEB Vorlesung((
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                 Application.Quit();
#endif
        //))
    }
}
