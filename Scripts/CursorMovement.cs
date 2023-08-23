/*****************************************************************************
* Project: Pipesgame
* File   : CursorMovement.cs
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
*   16.05.2021	JA	Created
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainGame
{
    /// <summary>
    /// Verwaltet den Spielerinput im Spiel
    /// </summary>
    public class CursorMovement : MonoBehaviour
    {
        MapManager mapManager;
        PipeGiver pipeGiver;

        AudioManager audioS;

        public Vector3Int cursorPosition = new Vector3Int(5, 5, 3);

        bool timerPressed = false;
        bool timerJustPressed = false;
        const float TIMEBETWEENSTEPS = .3f;
        const float TIMEAFTERFIRSTSTEP = .3f;

        eKeys eActiveKey;
        List<eKeys> pressedKeys = new List<eKeys>();

        enum eKeys { UP, DOWN, LEFT, RIGHT }

        void Start()
        {
            GetReferences();
        }
        void GetReferences()
        {
            mapManager = Database.sMapManager;
            pipeGiver = Database.sPipeGiver;
            audioS = Database.sAudioManager;
        }

        void Update()
        {
            DoAction();
            AddMovementKeys();
            DeleteMovementKeys();
            TimedMovement();
            SendCursorPosition();
        }
        /// <summary>
        /// Soll den Cursor erst ein Feld bewegen und nach gedrückthalten 
        /// schneller. Ist buggy deswegen sind beide Timer gleich lang.
        /// </summary>
        void TimedMovement()
        {
            if (Input.anyKeyDown)
            {
                timerJustPressed = true;
                DoMovement();
                Invoke("SetJustPressedFalse", TIMEAFTERFIRSTSTEP);
            }
            if (Input.anyKey && !timerJustPressed && !timerPressed)
            {
                DoMovement();
                timerPressed = true;
                Invoke("SetTimerPressedFalse", TIMEBETWEENSTEPS);
            }
            else if (!Input.anyKey)
            {
                timerJustPressed = false;
            }
        }
        void SetJustPressedFalse()
        {
            timerJustPressed = false;
        }
        void SetTimerPressedFalse()
        {
            timerPressed = false;
        }
        /// <summary>
        /// Fügt gedrückte Movekeys einer Liste hinzu damit der Cursor in die 
        /// letzte gedrückte Richtung fortfährt wenn man eine Taste kurz drückt 
        /// und wieder loslässt. Lässt sich vielleicht auch anders lösen.
        /// </summary>
        void AddMovementKeys()
        {
            if (Input.anyKey)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    pressedKeys.Add(eKeys.UP);
                if (Input.GetKeyDown(KeyCode.DownArrow))
                    pressedKeys.Add(eKeys.DOWN);
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    pressedKeys.Add(eKeys.LEFT);
                if (Input.GetKeyDown(KeyCode.RightArrow))
                    pressedKeys.Add(eKeys.RIGHT);
            }
        }
        /// <summary>
        /// Entfernt Movekeys von der Liste beim loslassen.
        /// </summary>
        void DeleteMovementKeys()
        {
            if (Input.GetKeyUp(KeyCode.UpArrow))
                pressedKeys.Remove(eKeys.UP);
            if (Input.GetKeyUp(KeyCode.DownArrow))
                pressedKeys.Remove(eKeys.DOWN);
            if (Input.GetKeyUp(KeyCode.LeftArrow))
                pressedKeys.Remove(eKeys.LEFT);
            if (Input.GetKeyUp(KeyCode.RightArrow))
                pressedKeys.Remove(eKeys.RIGHT);
        }
        /// <summary>
        /// Führt das Movement aus. 
        /// </summary>
        void DoMovement()
        {
            sbyte tilePosX = 0;
            sbyte tilePosY = 0;
            if (pressedKeys.Count != 0)
            {
                eActiveKey = pressedKeys[pressedKeys.Count - 1];
                switch (eActiveKey)
                {
                    case eKeys.UP:
                            tilePosY += 1;
                            break;
                    case eKeys.DOWN:
                            tilePosY += -1;
                            break;
                    case eKeys.LEFT:
                            tilePosX += -1;
                            break;
                    case eKeys.RIGHT:
                            tilePosX += 1;
                            break;
                    default: break;
                }
                cursorPosition.x = Mathf.Clamp(cursorPosition.x + 
                                              tilePosX, 1, MapManager.gridX-2);
                cursorPosition.y = Mathf.Clamp(cursorPosition.y + 
                                              tilePosY, 1, MapManager.gridY-2);
            }
        }
        /// <summary>
        /// Eine Funktion für Aktionen. Space setzt eine Pipe.
        /// </summary>
        void DoAction()
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //audioS.playAudio(0);
                    Vector3Int pos = new Vector3Int(cursorPosition.x, cursorPosition.y, 0);
                    pipeGiver.GivePipe(pos);
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                    SceneManager.LoadScene(0);
            }
        }
        void SendCursorPosition()
        {
            mapManager.DrawCursor(cursorPosition);
        }
    }
}