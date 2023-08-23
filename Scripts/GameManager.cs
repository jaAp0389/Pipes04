//3.6
/*****************************************************************************
* Project: Pipesgame
* File   : AudioManager.cs
* Date   : 03.06.2021
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
using TMPro;

//https://www.fontsquirrel.com/fonts/blazium?q%5Bterm%5D=computer&q%5Bsearch_check%5D=Y
namespace MainGame
{
    /// <summary>
    /// GameManager um das Spielgeschehen zu verwalten.
    /// </summary>
    public class GameManager: MonoBehaviour
    {
        bool isExit = false, isExitReached = false, isSteps = true;
        int stepsToMake,
            playerScore = 0, 
            gameLevel = 0,
            waterDistance = 5,
            bonusMax = 0, 
            slowMax = 0,
            wallMin = 2,
            switchesMax = 1,
            difficultyIncreaseInterval;
        MapManager mapManager;
        WaterManager waterManager;
        PipeGiver pipeGiver;
        public GameObject objScoreNumber, objLevelText, objDistanceText;
        TextMeshProUGUI textScore, textLevel, textDistance;
        public GameObject objBlackscreen;
        SpriteFader spriteFaderBS;

        void Start()
        {
            GetReferences();
            NewLevel();
        }
        /// <summary>
        /// returns number of unpressed switches
        /// </summary>
        int GetSwitchValue()
        {
            int value=0;
            foreach(int i in Database.s_switchList)
            {
                value += i;
            }
            return value;
        }
        /// <summary>
        /// gets references
        /// </summary>
        void GetReferences()
        {
            spriteFaderBS = objBlackscreen.GetComponent<SpriteFader>();
            textScore = objScoreNumber.GetComponent<TextMeshProUGUI>();
            textLevel = objLevelText.GetComponent<TextMeshProUGUI>();
            textDistance = objDistanceText.GetComponent<TextMeshProUGUI>();
            mapManager = Database.sMapManager;
            waterManager = Database.sWaterManager;
            pipeGiver = Database.sPipeGiver;
            difficultyIncreaseInterval = Database.s_difficultyIncreaseInterval;
        }
        /// <summary>
        /// old 
        /// </summary>
        public void LevelEnd(bool isVictory)
        {
            if(isVictory)
                SceneManager.LoadScene(1);
            else SceneManager.LoadScene(0);
        }
        /// <summary>
        /// Checks victory conditions. Starts next level or returns 
        /// to main menu. Changes difficulty settings.
        /// </summary>
        /// <param name="byExit">Level ends by Exit?</param>
        public void EndLevel(bool byExit = false)
        {
            isExitReached = byExit;
            int winConditions = (isSteps ? 1 : 0) + (isExit ? 1 : 0);
            if (isSteps)
                if (stepsToMake <= 0)
                    winConditions -= 1;
            if (isExit && isExitReached)
                winConditions -= 1;
            if (GetSwitchValue() > 0)
                winConditions += 1;
            if (winConditions <= 0)
            {
                if (gameLevel % difficultyIncreaseInterval == 0)
                {
                    wallMin = Mathf.Clamp(wallMin + 1, 0, 8);
                    slowMax = Database.randMap.Next(3);
                    bonusMax = Database.randMap.Next(2) + 1;
                    switchesMax = Mathf.Clamp(switchesMax + 1, 0, 4);
                    waterDistance = Mathf.Clamp(waterDistance + 2, 0, 20);
                    isExit = true;
                }
                else isExit = false;
                NewLevel();
                //spriteFaderBS.StartFade(1, false);
            }
            else SceneManager.LoadScene(0);//Loosegame highscore
        }
        /// <summary>
        /// Faderhelper to start next function after fade is dinished.
        /// </summary>
        /// <param name="nextStep"></param>
        public void FinishedFade(bool nextStep)
        {
            switch(nextStep)
            {
                case true:
                    waterManager.ResetWaterManager();
                    break;
                case false:
                    NewLevel();
                    break;

            }
        }
        /// <summary>
        /// changes levelnumber and text.
        /// </summary>
        void ChangeLevel()
        {
            gameLevel += 1;
            textLevel.text = $"Lvl {AddZeros(gameLevel, 2)}";
        }
        /// <summary>
        /// decreases waterdistance left by -1. Returns true if watersteps are 0.
        /// </summary>
        /// <returns></returns>
        public bool DecreaseWaterDistance()
        {
            stepsToMake -= 1;
            if (stepsToMake < 0) return true;
            textDistance.text = $"Dist {AddZeros(stepsToMake, 2)}";
            return false;
        }
        /// <summary>
        /// changes scorenumber and text.
        /// </summary>
        public void ChangeScore(int points)
        {
            playerScore += points;
            textScore.text = AddZeros(playerScore, 9);
        }
        /// <summary>
        /// adds zeros in front of numberstring to fill up length
        /// </summary>
        string AddZeros(int number, int length)
        {
            string stringNumber = number.ToString();
            string stringZeros = "";
            for(int i = 0; i < length - stringNumber.Length; i++)
            {
                stringZeros += '0';
            }
            return stringZeros + stringNumber;
        }

        /// <summary>
        /// Executes anything that has to do with generating a new level.
        /// </summary>
        public void NewLevel()
        {
            print("newLev"+ gameLevel);
            stepsToMake = waterDistance;
            textDistance.text = $"Dist {AddZeros(stepsToMake, 2)}";
            mapManager.ClearAll();
            mapManager.CreateBorder();
            mapManager.FloodWithWalls();
            mapManager.GenerateSpecialTiles(isExit, bonusMax, slowMax, 
                                                         wallMin, switchesMax);
            pipeGiver.NewPipeArray();
            ChangeLevel();
            //spriteFaderBS.StartFade(0, true);
            waterManager.ResetWaterManager();
        }

        public void ReachedVictoryCondition()
        {

        }
    }
}
