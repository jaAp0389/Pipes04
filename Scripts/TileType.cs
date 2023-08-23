/*****************************************************************************
* Project: Pipesgame
* File   : TileType.cs
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
*   24.05.2021	JA	Created
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame
{
    /// <summary>
    /// General TileClass with functions for checking water interaction.
    /// </summary>
    abstract public class TileType
    {
        //get lastposition
        //make relDirection 
        //make newDirection 
        //return newposition

        
        public eDirection[] directions= new eDirection[2]; //protected
        public Vector3Int ownPosition { get; protected set; }
        public eRotation ownRotation { get; set; }
        protected bool isTeleExit { get; private set; } = false;
        public bool isWaterReversed { get; protected set; } = false;
        //protected int checkedDirectionPos { private get; set; }
        public int checkedDirectionPos;
        //protected int countAmount { private get; set; }
        public int countAmount;
        public int animCountdown { get; protected set; } = 2;
        public eTileType ownTileType { get; protected set; }
        public TileType(Vector3Int _ownPosition, eRotation _rotation)
        {
            ownRotation = _rotation;
            ownPosition = _ownPosition;
            //f ((int)ownRotation != 0) 
            
        }

        public eDirection checkdir;
        /// <summary>
        /// Gibt die nächste Wasserposition zurück. Stellt die 
        /// Wasserfließrichtung fest. 
        /// </summary>
        public virtual Vector3Int NextPosition()
        {
            int newDirection = CountEnum
                         (checkedDirectionPos, countAmount, directions.Length);
            return ownPosition + DirectionToV3Int(directions[newDirection]);
        }
        /// <summary>
        /// Stellt fest ob die Pipe von der Seite der Einfließrichtung eine 
        /// Öffnung hat. Vergleicht dazu die Wasserrichtung mit einer Liste der
        /// Öffnungsrichtungen. Returns false if leak.
        /// </summary>
        public virtual bool CheckDirection(Vector3Int lastPosition)
        {
            eDirection direction = IntToDirection
                                          (GetRelativeDirection(lastPosition)); 
            for (int i = 0; i < directions.Length; i++)
                if (directions[i] == direction)
                {
                    checkdir = directions[i];
                    checkedDirectionPos = i;
                    return true;
                }
            return false;
        }
        /// <summary>
        /// Erstellt die relative Position der letzten Wasserposition und 
        /// wandelt diese in eine Zahl um. Der Fall -1 für den Teleporttyp falls 
        /// die letzte Position nicht neben der aktuellen Position ist.
        /// </summary>
        protected int GetRelativeDirection(Vector3Int lastPosition)
        {
            Vector3Int relativPosition = lastPosition - ownPosition;
            int xRelPos = relativPosition.x
               ,yRelPos = relativPosition.y;
            switch (xRelPos) 
            {
                case -1: 
                    return 3;//WEST
                case 1: 
                    return 1;//EAST
                default: break;
            }
            switch (yRelPos) 
            {
                case -1:
                    return 2;//SOUTH
                case 1: 
                    return 0;//NORTH
            }
            isTeleExit = true;
            return -1;
        }
        /// <summary>
        /// Wandelt die IntRichtung in eine EnumRichtung um. WIP
        /// </summary>
        protected virtual eDirection IntToDirection(int directionInt)
        {
            //if (directionInt > -1) 
                return (eDirection)directionInt;
            //return eDirection.NORTH;
        }
        /// <summary>
        /// Wandelt die enumDirection in eine Vector3 Direction um.
        /// </summary>
        protected Vector3Int DirectionToV3Int(eDirection eDirection)
        {
            switch (eDirection)
            {
                case eDirection.NORTH:
                    {
                        return Vector3Int.up;
                    }
                case eDirection.EAST:
                    {
                        return Vector3Int.right;
                    }
                case eDirection.SOUTH:
                    {
                        return Vector3Int.down;
                    }
                case eDirection.WEST:
                    {
                        return Vector3Int.left;
                    } 
            }
            return Vector3Int.zero;
        }
        /// <summary>
        /// Rotiert die Pipeöffnungen.
        /// </summary>
        protected virtual void RotateDirections()
        {
            for (int i = 0; i < directions.Length; i++)
                directions[i] = (eDirection)
                             CountEnum((int)directions[i],(int)ownRotation, 4);
        }
        /// <summary>
        /// Loopt durch einen Zahlenbereich.
        /// </summary>
        private int CountEnum(int enumPos, int amount, int length)
        {
            return ((enumPos + amount) % length);
        }
    }
}
