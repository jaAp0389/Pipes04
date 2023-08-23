/*****************************************************************************
* Project: Pipesgame
* File   : TileTypeTemplates.cs
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
    /// Childclasses von TilyType. Pipeversionen mit eigenen Pipeöffnungen.
    /// </summary>


    public class Corner : TileType
    {
        //L
        public Corner(Vector3Int _ownPosition, eRotation _rotation) 
                                                : base(_ownPosition, _rotation)
        {
            countAmount = 1;
            directions[0] = eDirection.NORTH;
            directions[1] = eDirection.EAST;
            ownTileType = eTileType.CORNER;
            RotateDirections();
        }
        public override bool CheckDirection(Vector3Int lastPosition)
        {
            eDirection direction = IntToDirection
                                          (GetRelativeDirection(lastPosition));
            for (int i = 0; i < directions.Length; i++)
                if (directions[i] == direction)
                {
                    if (i != 0) isWaterReversed = true;
                    checkdir = directions[i];
                    checkedDirectionPos = i;
                    return true;
                }
            return false;
        }

    }
    public class Straight : TileType
    {
        //I
        //0=Normal 1=Fast 2=Slow 3=Bonus
        public int special { get; private set; }
        public Straight(Vector3Int _ownPosition, eRotation _rotation,
                              int _special = 0) : base(_ownPosition, _rotation)
        {
            countAmount = 1;
            directions[0] = eDirection.NORTH;
            directions[1] = eDirection.SOUTH;
            special = _special;
            SetTileType();
            RotateDirections();
        }

        private void SetTileType()
        {
            ownTileType = (eTileType)(special + 2);
        }
    }
    public class Cross : TileType
    {
        //isSecondpass damit später der 2te Durchgang an Wasser auf einer
        //anderen z Ebene gezeichnet wird. Für eine override
        //Wasserfunktion o.ä.
        public bool isSecondPass { get; private set; } = false;
        public Cross(Vector3Int _ownPosition, eRotation _rotation)
                                                : base(_ownPosition, _rotation)
        {
            countAmount = 2;
            System.Array.Resize(ref directions, 4);
            directions[0] = eDirection.NORTH;
            directions[1] = eDirection.EAST;
            directions[2] = eDirection.SOUTH;
            directions[3] = eDirection.WEST;
            ownTileType = eTileType.CROSS;
            //RotateDirections();
        }
        public override bool CheckDirection(Vector3Int lastPosition)
        {
            if (isSecondPass) ownPosition += Vector3Int.back;
            isSecondPass = true;
            checkedDirectionPos = (GetRelativeDirection(lastPosition));
            checkdir = (eDirection)checkedDirectionPos;
            //ownRotation = (eRotation)directions[checkedDirectionPos];
            return true;
        }

    }
    /// <summary>
    /// Soll als Paar mit Verweisen aufeinander erstellt werden. Stellt fest ob
    /// es der Teleportein- oder ausgang ist anhand der Entfernung der letzten 
    /// Position.
    /// </summary>
    public class Teleport : TileType
    {
        Vector3Int teleportExit { get; set; }
        eDirection ownDirection { get; set; } =  eDirection.NORTH;
        public Teleport(Vector3Int tExit, Vector3Int _ownPosition, 
                  eRotation _rotation) : base(_ownPosition, _rotation)
        {
            teleportExit = tExit;
            ownTileType = eTileType.TELE;
            RotateDirections();
        }
        public override bool CheckDirection(Vector3Int lastPosition)
        {
            return IntToDirection(GetRelativeDirection(lastPosition)) == 
                                                                  ownDirection;
        }
        protected override void RotateDirections()
        {
            ownDirection = (eDirection)(int)ownRotation;
            checkdir = ownDirection;
        }
        public override Vector3Int NextPosition()
        {
            isWaterReversed = !isTeleExit;
            return isTeleExit ? teleportExit : ownPosition 
                                              + DirectionToV3Int(ownDirection);
        }
    }
    /// <summary>
    /// Start-/Exittile template.
    /// </summary>
    public class Start : TileType
    {
        //V
        bool isExit { get; set; } = false;
        eDirection ownDirection { get; set; }
        public Start(Vector3Int _ownPosition, eRotation _rotation,
                                  bool _isExit) : base(_ownPosition, _rotation)
        {
            countAmount = 0;
            isExit = _isExit;
            ownTileType = isExit ? eTileType.EXIT : eTileType.START;
            ownDirection = (eDirection)(int)ownRotation;
            checkdir = ownDirection;
        }
        public override bool CheckDirection(Vector3Int lastPosition)
        {
            if (lastPosition == ownPosition) return true;
            return IntToDirection(GetRelativeDirection(lastPosition)) ==
                                                                  ownDirection;
        }
        public override Vector3Int NextPosition()
        {
            return isExit ? ownPosition : //end game
                ownPosition + DirectionToV3Int(ownDirection); 
        }
    }

    /// <summary>
    /// Simple walltile template.
    /// </summary>
    public class Wall : TileType
    {
        //L
        public Wall(Vector3Int _ownPosition, eRotation _rotation)
                                                : base(_ownPosition, _rotation)
        {
            ownTileType = eTileType.WALL;
        }
        public override bool CheckDirection(Vector3Int lastPosition)
        {
            return false;
        }
        protected override void RotateDirections()
        { }
    }
}