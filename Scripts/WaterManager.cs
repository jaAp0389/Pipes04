//29.5
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


namespace MainGame
{
    public class WaterManager : MonoBehaviour
    {
        //CountdownSlider
        //StartWater at StartPosition
        //Get Next WaterPosition, Save LastWaterPosition
        //Draw Water : Get eTilType switch + Direction
        //Repeat
        

        SliderManager sliderManager;
        MapManager mapManager;
        AnimationFramerate animFrame;
        GameManager gameManager;
        GameObject objSliderManager;

        Vector3Int lastWaterPosition,
                   currentWaterPosition;

        float sliderSpeed;
        int waterflowRate;



        bool leak = false;
        bool levelExit = false;
        bool isStepsDone = false;

        int waterStepsToWin = 7;


        void Start()
        {
            GetReferences();
        }
        public void GetStartPosition(Vector3Int waterStart)
        {
            currentWaterPosition = waterStart;
            lastWaterPosition = waterStart;
        }
        void testtiles()
        {

            mapManager.CreateAnimatedTile(new Vector3Int(1, 2, 3),
                              eAnimType.WATER, eAnimTileType.WCORNER,
                                     eRotation._90, false, 8, false);

            mapManager.CreateAnimatedTile(new Vector3Int(2, 2, 3),
                              eAnimType.WATER, eAnimTileType.WCORNER,
                                     eRotation._90, false, 4, true);
            mapManager.CreateAnimatedTile(new Vector3Int(3, 2, 3),
                              eAnimType.WATER, eAnimTileType.WSTRAIGHT,
                                     eRotation._90, false, 2, false);

            mapManager.CreateAnimatedTile(new Vector3Int(4, 2, 3),
                              eAnimType.WATER, eAnimTileType.WSTRAIGHT,
                                     eRotation._90, false, 1, true);
        }
        /// <summary>
        /// Gets references.
        /// </summary>
        void GetReferences()
        {
            waterflowRate = Database.s_waterflowRate;
            sliderSpeed = Database.s_waterstartSliderSpeed;
            animFrame = Database.sAnimFrame;
            sliderManager = Database.sSliderManager;
            mapManager = Database.sMapManager;
            gameManager = Database.sGameManager;
            objSliderManager = Database.s_objSliderManager;
        }
        /// <summary>
        /// Class for updating the waterStartslider. 
        /// Starts first Waterstep.
        /// </summary>
        void UpdateSlider()
        {
            if (sliderManager.DecreaseSlider(sliderSpeed))
            {
                animFrame.animationFrame.RemoveListener(UpdateSlider);
                objSliderManager.SetActive(false);
                WaterStep();
                //DrawWater(mapManager.pipeGrid[currentWaterPosition.x, currentWaterPosition.y]);
            }
        }
        /// <summary>
        /// Class to reset everything reused in the next level. 
        /// </summary>
        public void ResetWaterManager()
        {
            objSliderManager.SetActive(true);
            sliderManager.ResetSlider();
            animFrame.animationFrame.AddListener(UpdateSlider);
            leak = false;
            isStepsDone = false;
            levelExit = false;
        }
        /// <summary>
        /// Checks if the Exit isn't reached. 
        /// Checks the next position. Draws water if there was no leak 
        /// or notifys GameManager to end the level. The Animationfunction
        /// from mapmanager returns to here after the animation is finished.
        /// </summary>
        public void WaterStep()
        {
            if (!levelExit)
            {
                if (!leak)
                {
                    
                    TileType currentTType = mapManager.pipeGrid
                                  [currentWaterPosition.x, currentWaterPosition.y];
                    if (currentTType != null)
                    {
                        GetNextPosition(currentTType);
                        if (!leak)
                        {
                            DrawWater(currentTType);
                            if (mapManager.createdSwitches
                                        [lastWaterPosition.x,
                                        lastWaterPosition.y] != null)
                                mapManager.SwitchSwitch(lastWaterPosition);
                        }
                        else gameManager.EndLevel();
                    }
                    else
                    {
                        leak = true;
                        gameManager.EndLevel();
                    }
                }///
            }
            else gameManager.EndLevel(true);
                
        }
        /// <summary>
        /// Draws Water at Position and marks the spot nonplaceable.
        /// </summary>
        void DrawWater(TileType tType)
        {
            mapManager.nonPlaceableGrid
                             [tType.ownPosition.x, tType.ownPosition.y] = true;
            mapManager.CreateAnimatedTile(tType.ownPosition,
                              eAnimType.WATER, GetWaterType(tType.ownTileType),
                         (eRotation)tType.checkdir, false, waterflowRate, 
                                                        tType.isWaterReversed);
        }
        /// <summary>
        /// Translates Tiletype to WaterType. Adds score by type.
        /// </summary>
        eAnimTileType GetWaterType(eTileType tType)
        {
            if (!isStepsDone)
                isStepsDone = gameManager.DecreaseWaterDistance();
            gameManager.ChangeScore(100);
            switch (tType)
            {
                case eTileType.CORNER:
                    return eAnimTileType.WCORNER;
                case eTileType.CROSS:
                case eTileType.INORMAL:
                    return eAnimTileType.WSTRAIGHT;
                case eTileType.IFAST:
                    return eAnimTileType.WFAST;
                case eTileType.IBONUS:
                    gameManager.ChangeScore(400);
                    return eAnimTileType.WSTRAIGHT;
                case eTileType.ISLOW:
                    return eAnimTileType.WSLOW;
                case eTileType.START:
                    return eAnimTileType.WSTART;
                case eTileType.EXIT:
                    levelExit = true;
                    return eAnimTileType.WEND;
                case eTileType.WALL:
                    return eAnimTileType.EXPLOSION;
                case eTileType.TELE: //----
                    return eAnimTileType.WSTART;
                default: return eAnimTileType.EXPLOSION;
            }
        }
        /// <summary>
        /// Checks for leak. Gets Nextposition from Tiletype at currentpos.
        /// </summary>
        void GetNextPosition(TileType tType)
        {
            Vector3Int lPos = lastWaterPosition;

            leak = !tType.CheckDirection(lPos);
            lastWaterPosition = currentWaterPosition;
            currentWaterPosition = tType.NextPosition();
        }
    }
}
