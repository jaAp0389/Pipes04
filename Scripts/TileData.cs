//28.5
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
using UnityEngine;
using System.Collections.Generic;

namespace MainGame
{
    /// <summary>
    /// short Tiledata, just with position Type and rotation.
    /// </summary>
    public class TileData
    {
        
        public Vector3Int position { get; set; }
        public eTileType eType { get; private set; }
        public eRotation rotation { get; private set; }
        
        public TileData(Vector3Int _position, eTileType _eType,
                                 eRotation _rotation)
        {
            position = _position;
            eType = _eType;
            rotation = _rotation;
        }
    }
    /// <summary>
    /// short TileData without position for PipeGiver class.
    /// </summary>
    public struct TileDataPG
    {
        public eTileType eType { get; private set; }
        public eRotation rotation { get; private set; }

        public TileDataPG(eTileType _eType, eRotation _rotation)
        {
            eType = _eType;
            rotation = _rotation;
        }
    }
    /// <summary>
    /// AnimatedTileData with everything that is nneeded for the animation.
    /// </summary>
    public class AnimatedTileData
    {
        //puddle?
        public Vector3Int position { get; private set; }
        public eRotation rotation { get; private set; }
        public int animFrame { get; set; }
        public int animframeLast { get; private set; }
        public eAnimType animType { get; private set; }
        public eAnimTileType animTileType { get; private set; }
        public bool isReversed { get; private set; }
        public int animationCountdown { get; set; }
        public int animationCountpos { get; set; }

        public AnimatedTileData(Vector3Int _position, eAnimType _animType, 
                              eAnimTileType _animTileType, eRotation _rotation, 
                  bool _isImmediate, int _animationCountdown, bool _isReversed)
        {
            position = _position;
            animFrame = 0;
            animframeLast = Database.tileSets[(ePaths)((int)_animTileType + 4)].Length;
            animType = _animType;
            animTileType = _animTileType;
            rotation = _rotation;
            isReversed = _isReversed;
            animationCountdown = _animationCountdown;
            animationCountpos = _isImmediate ? _animationCountdown : 0;
        }
        
    }
}