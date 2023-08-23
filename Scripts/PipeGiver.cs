//30.5
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
using UnityEngine.Tilemaps;

namespace MainGame
{
    /// <summary>
    /// Pipe vending machine but free
    /// </summary>
    public class PipeGiver : MonoBehaviour
    {
        //ref mapmanger
        //create array of pipes
        //loadeddice
        //return pipe
        //move pipes down slow
        //create new pipe

        [SerializeField]
        Tilemap pipeGiverTilemap;
        MapManager mapManager;
        AnimationFramerate animFrame;
        AudioManager audioManager;
        Tile[] pipes;
        TileDataPG[] pipeArray = new TileDataPG[6];
        bool isReady = true;
        float moveRate;
        public Vector3 ownPosition;
        private enum ePipes { I, L, Cr, Fa } //Straight, Corner, Cross, Slow

        void Start()
        {
            ownPosition = pipeGiverTilemap.transform.position;
            GetReferences();
        }
        public void NewPipeArray()
        {
            CreatePipeArray();
            DrawPipesArray();
        }
        /// <summary>
        /// Moves the tMap down like its falling trough gravity.
        /// </summary>
        void MoveTMapStep()
        {
            if (pipeGiverTilemap.transform.position.y > ownPosition.y)
            {
                pipeGiverTilemap.transform.position =
                       Vector3.MoveTowards(pipeGiverTilemap.transform.position,
                       ownPosition, moveRate);
            }
            else
            {
                isReady = true;

                animFrame.animationFrame.RemoveListener(MoveTMapStep);
            }
        }
        /// <summary>
        /// gets references
        /// </summary>
        void GetReferences()
        {
            pipes = Database.tileSets[ePaths.PIPES];
            animFrame = Database.sAnimFrame;
            mapManager = Database.sMapManager;
            audioManager = Database.sAudioManager;
            moveRate = Database.s_pipeGiverFallSpeed;
        }
        /// <summary>
        /// sets tiles according to stored pipes array.
        /// </summary>
        void DrawPipesArray()
        {
            for (int i = 0; i < pipeArray.Length; i++)
            {
                Vector3Int pos = new Vector3Int(-1, i + 4, 0);
                mapManager.RotateTileAtPosition(pipeGiverTilemap,
                                                   pos, pipeArray[i].rotation);
                pipeGiverTilemap.SetTile(pos, pipes[(int)pipeArray[i].eType]);
            }
        }
        void CreatePipeArray()
        {
            for (int i = 0; i < pipeArray.Length; i++)
            {
                pipeArray[i] = GetNewPipe();
            }
        }
        /// <summary>
        /// moves Array one forward and deletes [0] spot, then populates last 
        /// spot with new pipedata.  
        /// </summary>
        void MovePipeArrayBackOne()
        {
            TileDataPG[] tempArray = new TileDataPG[pipeArray.Length];
            for (int i = 1; i < pipeArray.Length; i++)
            {
                tempArray[i - 1] = pipeArray[i];
            }
            pipeArray = tempArray;
            pipeArray[pipeArray.Length - 1] = GetNewPipe();
        }
        /// <summary>
        /// Returns first pipe in pipearray to player.
        /// </summary>
        public void GivePipe(Vector3Int position)
        {
            if (!mapManager.nonPlaceableGrid[position.x, position.y])
                if (isReady)
                {
                    audioManager.playAudio(0);
                    TileDataPG pipe = pipeArray[0];
                    MovePipeArrayBackOne();
                    DrawPipesArray();
                    animFrame.animationFrame.AddListener(MoveTMapStep);
                    pipeGiverTilemap.transform.position = ownPosition + Vector3.up;
                    isReady = false;
                    mapManager.PlaceTile
                              (new TileData(position, pipe.eType, pipe.rotation));
                }
        }
        int[] diceValues = { 3, 1, 0, 2, 3, 2, 0 };
        int[][] diceValuesJ = new int[][]
        {
            new int[] {  1, 2, 0, 3},
            new int[] { 0},
            new int[] { 1, 0},
            new int[] { 0, 1},
        };
        int trueRandomCounter = 0;
        /// <summary>
        /// Creates new slightly random pipe. Works with an [,] array with 
        /// the pipetype and rotation.
        /// </summary>
        /// <returns></returns>
        TileDataPG GetNewPipe()
        {
            bool isTrueRandom = false;
            if(trueRandomCounter > Database.randPipeChooser.Next(4)+3)
            {
                isTrueRandom = true;
                trueRandomCounter = 0;
            }
            trueRandomCounter += 1;
            int Pos = isTrueRandom? Database.randPipeChooser.Next(diceValues.Length): 0;
            int rollPArr = diceValues[Pos];
            MoveItemInArray(ref diceValues, 0, diceValues.Length - 
                Database.RollLoadedDice(Database.randPipeChooser, 2, 1, 2));
            int rollPDir = Database.RollLoadedDice(Database.randPipeChooser, 
                2, diceValuesJ[rollPArr].Length);
            int PDir = diceValuesJ[rollPArr][rollPDir];
            MoveItemInArray(ref diceValuesJ[rollPArr], rollPDir);
            return new TileDataPG(IntToTileType(rollPArr), (eRotation)PDir);
        } 

        eTileType IntToTileType(int i)
        {
            switch(i)
            {
                case 0:
                case 4:
                    return eTileType.CORNER;
                case 1:
                    return eTileType.CROSS;
                case 2:
                    return eTileType.INORMAL;
                case 3:
                    return eTileType.IFAST;
                default:
                    return eTileType.WALL;
            }
        }
        /// <summary>
        /// Moves item in array to last position or to a position specified.
        /// </summary>
        void MoveItemInArray(ref int[] array, int pos, int toPosition = -1)
        {
            if (pos == toPosition) return;
            int[] tempArray = new int[array.Length];
            if (toPosition < array.Length - 1 && toPosition > -1)
            {
                for (int i = 0; i < System.Math.Min(pos, toPosition); i++)
                {
                    tempArray[i] = array[i];
                }
                tempArray[System.Math.Min(pos, toPosition)] = pos > toPosition
                                                   ? array[pos] : array[pos + 1];
                for (int i = System.Math.Min(pos, toPosition) + 1;
                                     i < System.Math.Max(pos, toPosition); i++)
                {
                    tempArray[i] = array[i + (pos < toPosition ? 1 : -1)];
                }
                tempArray[System.Math.Max(pos, toPosition)] = pos < toPosition
                                                 ? array[pos] : array[pos - 1];
                for (int i = System.Math.Max(pos, toPosition) + 1;
                                                          i < array.Length; i++)
                {
                    tempArray[i] = array[i];
                }
                array = tempArray;
                return;
            }
            for (int i = 0; i < pos; i++)
            {
                tempArray[i] = array[i];
            }
            for (int i = pos + 1; i < array.Length; i++)
            {
                tempArray[i - 1] = array[i];
            }
            tempArray[array.Length - 1] = array[pos];
            array = tempArray;
        }
    }
}
