//28.5

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
using System.Collections.Generic;

namespace MainGame
{
    /// <summary>
    /// Manages placing Tiles, Animating Tiles and Tilegrids
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        public Tilemap gameTilemap,
                       waterTilemap,
                       backgroundTilemap,
                       testTilemap,
                       switchTilemap;

        WaterManager waterManager;
        AnimationFramerate animFrame;
        CursorMovement cursorMov;

        static public int gridX = 12, 
                          gridY = 9;

        public TileType[,] pipeGrid { get; set; } = new TileType[gridX, gridY];
        public bool[,] nonPlaceableGrid { get; set; } = new bool[gridX, gridY];
        bool[,] createdPipes { get; set; } = new bool[gridX, gridY];
        public Switch[,] createdSwitches { get; set; } = new Switch[gridX, gridY];

        List <AnimatedTileData> animatedTiles = new List<AnimatedTileData>();

        int cursorAnimRate,
            cursorAnimPos = 0,
            switchIndex = 0;

        public float animationFramerate = 0.5f;
        float animationStep = 0;

        Tile[] cursorTiles;
        Vector3Int cursorPosition = new Vector3Int(5, 5, 0); //startwater
        int cursorTilenum = 0;

        private void Start()
        {
            GetReferences();
            CreateBackground();
            animFrame.animationFrame.AddListener(DoAnimationFrame);
            animFrame.animationFrame.AddListener(AnimateCursor);
        }
        /// <summary>
        /// Creates <number> Switches on the Map
        /// </summary>
        void CreateSwitch(int number = 1)
        {
            Database.s_switchList.Add(number);
            for(int i = 0; i < number; i++)
            {
                Vector3Int v3temp = GetFreestandingPosition() + Vector3Int.back;
                createdSwitches[v3temp.x, v3temp.y] = new Switch(switchIndex);
                switchTilemap.SetTile(v3temp + Vector3Int.back,
                                         Database.tileSets[ePaths.SWITCH][0]);
            }
            switchIndex += 1;
        }
        /// <summary>
        /// Switches the Switch at Position and changes its Tile
        /// </summary>
        /// <param name="pos"></param>
        public void SwitchSwitch(Vector3Int pos)
        {
            createdSwitches[pos.x, pos.y].PressSwitch();
            switchTilemap.SetTile(pos + (Vector3Int.back*2), 
                Database.tileSets[ePaths.SWITCH][1]);
        }
        /// <summary>
        /// Clears everything thats reused in next Level
        /// </summary>
        public void ClearAll()
        {
            gameTilemap.ClearAllTiles();
            waterTilemap.ClearAllTiles();
            switchTilemap.ClearAllTiles();
            animatedTiles.Clear();
            switchIndex = 0;
            Database.s_switchList.Clear();
            createdPipes = new bool[gridX, gridY];
            createdSwitches = new Switch[gridX, gridY];
            nonPlaceableGrid = new bool[gridX, gridY];
            pipeGrid = new TileType[gridX, gridY];

        }
        /// <summary>
        /// Gets References from Database class
        /// </summary>
        void GetReferences()
        {
            cursorAnimRate = Database.s_cursorAnimRate;
            animFrame = Database.sAnimFrame;
            waterManager = Database.sWaterManager;
            cursorMov = Database.sCursorMovement;
            cursorTiles = Database.tileSets[ePaths.CURSOR];
        }
        /// <summary>
        /// floods Backgroundmap wit background tiles
        /// </summary>
        void CreateBackground()
        {
            for(int x = 1; x < gridX-1; x++)
            {
                for (int y = 1; y < gridY-1; y++)
                {
                    backgroundTilemap.SetTile(new Vector3Int(x, y, 0),Database.
                        tileSets[ePaths.BACKGROUND][Database.randMap.Next(1)]);
                }
            }
        }
        /// <summary>
        /// Floods map with walls to help with leak detection. possibly unneeded
        /// </summary>
        public void FloodWithWalls()
        {
            for (int x = 1; x < gridX - 1; x++)
            {
                for (int y = 1; y < gridY - 1; y++)
                {
                    Vector3Int v3temp = new Vector3Int(x, y, 0);
                    pipeGrid[x, y] = new Wall(v3temp, eRotation._0);
                }
            }
        }
        /// <summary>
        /// Creates Mapborders
        /// </summary>
        public void CreateBorder()
        {
            drawBorder(0,0, eRotation._0, 1);
            drawBorder(gridX-1, 0, eRotation._270, 1);
            drawBorder(gridX-1, gridY-1, eRotation._180, 1);
            drawBorder(0, gridY-1, eRotation._90, 1);

            for (int i = 1; i < gridX-1; i++)
            {
                drawBorder(i, 0, eRotation._270, 0);
                drawBorder(i, gridY-1, eRotation._90, 0);
            }

            for (int k = 1; k < gridY-1; k++)
            {
                drawBorder(0, k, eRotation._0, 0);
                drawBorder(gridX-1, k, eRotation._180, 0);
            }
        }
        /// <summary>
        /// places MapborderTiles and Walls for leakdetection
        /// </summary>
        void drawBorder(int i, int k, eRotation rotation, int tileNumber)
        {
            Vector3Int v3temp = new Vector3Int(i, k, 0);
            pipeGrid[i, k] = new Wall(v3temp, eRotation._0);
            backgroundTilemap.SetTile(v3temp,
                                 Database.tileSets[ePaths.BORDER][tileNumber]);
            RotateTileAtPosition(backgroundTilemap, v3temp, rotation);
        }
        /// <summary>
        /// Generates all non placeable Tiles according to instructions from gamemanager.
        /// </summary>
        public void GenerateSpecialTiles(bool isExit, int bonusMax = 0, int slowMax = 0, 
            int wallMin = 2, int switches = 0)
        {
            Vector3Int wStart = GetFreestandingPosition();
            waterManager.GetStartPosition(wStart);
            cursorMov.cursorPosition = wStart + new Vector3Int(0, 0, 3);
            CreateSpecialTile(eTileType.START, (eRotation)
                          Database.randMap.Next(4), wStart);
            if(isExit)
            {
                Vector3Int wExit = GetFreestandingPosition();
                CreateSpecialTile(eTileType.EXIT, (eRotation)
                              Database.randMap.Next(4), wExit);
            }
            for (int i = 0; i < LoadedDiceHelper(wallMin, wallMin + 1, wallMin + 2); i++)
            {
                CreateSpecialTile(eTileType.WALL, eRotation._90, 
                    GetFreestandingPosition());
            }
            for (int i = 0; i < LoadedDiceHelper(bonusMax); i++)
            {
                CreateSpecialTile(eTileType.IBONUS,
                                  (eRotation)Database.randMap.Next(4),
                                                    GetFreestandingPosition());
            }
            for (int i = 0; i < LoadedDiceHelper(slowMax); i++)
            {
                CreateSpecialTile(eTileType.ISLOW,
                                  (eRotation)Database.randMap.Next(1),
                                                    GetFreestandingPosition());
            }
            if (switches > 0)
                CreateSwitch(LoadedDiceHelper(switches));
        }
        int LoadedDiceHelper(params int[] results)
        {
            return Database.RollLoadedDice(Database.randMap, 2, results);
        }

        /// <summary>
        /// returns position with now direct neighbours.
        /// </summary>
        /// <returns></returns>
        Vector3Int GetFreestandingPosition()
        {
            while (true)
            {
                int posX = Database.randMap.Next(2, gridX - 2),
                    posY = Database.randMap.Next(2, gridY - 2),
                    trys = 0;

                if (!nonPlaceableGrid[posX, posY])
                {
                    trys += 1;
                    if (trys >= 1000) return Vector3Int.zero;
                    for (int i = -1; i < 2; i += 2)
                    {
                        if (nonPlaceableGrid[posX + i, posY])
                            goto End;
                        if (nonPlaceableGrid[posX, posY + i])
                            goto End;
                    }
                }
                else continue;
                return new Vector3Int(posX, posY, 0);
            End:continue;
            }
        }
        /// <summary>
        /// Creates AnimatedTile with various properties.
        /// </summary>
        public void CreateAnimatedTile(Vector3Int position,
                    eAnimType animType, eAnimTileType type, eRotation rotation, 
                    bool isImmediate, int animationCountdown = 2, 
                    bool isReversed = false)
        {
            animatedTiles.Add(new AnimatedTileData (position, animType, type,
                       rotation, isImmediate, animationCountdown, isReversed));
            if (animType != eAnimType.WATER)
            {
                RotateTileAtPosition(gameTilemap, position, rotation,
                                                                   isReversed);
                //if(type != AnimatedTileData.eType.EXPLOSION)
                //    nonPlaceableGrid[position.x, position.y] = true;  ///-----
                if (type == eAnimTileType.BONUS)
                    pipeGrid[position.x, position.y] = TranslateTileType
                              (eTileType.IBONUS, rotation, position);
            }
        }
        /// <summary>
        /// Creates nonplaceable Tile with properties.
        /// </summary>
        public void CreateSpecialTile(eTileType eType, eRotation 
                                       rotation, params Vector3Int[] positions)
        {
            switch (eType)
            {
                case eTileType.ISLOW:
                    PlaceTile(new TileData(positions[0], eType, rotation));
                    createdPipes[positions[0].x, positions[0].y] = true;
                    break;
                case eTileType.IBONUS:
                    CreateAnimatedTile(positions[0], eAnimType.
                              ENDLESS, eAnimTileType.BONUS, rotation, true, 1);
                    createdPipes[positions[0].x, positions[0].y] = true;
                    break;
                case eTileType.START:
                    PlaceTile(new TileData(positions[0], eType, rotation));
                    break;
                case eTileType.EXIT:
                    PlaceTile(new TileData(positions[0], eType, rotation));
                    break;
                case eTileType.WALL:
                    PlaceTile(new TileData(positions[0], eType, rotation));
                    break;
                case eTileType.TELE:
                    PlaceTeleport(new TileData(positions[0], eType, rotation),
                                  new TileData(positions[1], eType, rotation));
                    break;
                default: return;
            }
            foreach(Vector3Int pos in positions)
                nonPlaceableGrid[pos.x, pos.y] = true;
        }

        /// <summary>
        /// Places and rotates placeable tile and marks it in createdPipeList.
        /// </summary>
        public void PlaceTile(TileData tData)
        {
            gameTilemap.SetTile(tData.position, Database.tileSets
                                    [ePaths.PIPES][(int)tData.eType]);
            RotateTileAtPosition(gameTilemap, tData.position, tData.rotation);
            pipeGrid[tData.position.x, tData.position.y] = TranslateTileType
                                 (tData.eType, tData.rotation, tData.position);
            createdPipes[tData.position.x, tData.position.y] = true;
        }
        /// <summary>
        /// Method for placing teleports. Unused.
        /// </summary>
        void PlaceTeleport(TileData tDataA, TileData tDataB)
        {
            Vector3Int posA = tDataA.position;
            nonPlaceableGrid[posA.x, posA.y] = true;
            gameTilemap.SetTile(posA, Database.tileSets[ePaths.PIPES]
                                                          [(int)tDataA.eType]);
            RotateTileAtPosition(gameTilemap, posA, tDataA.rotation);
            pipeGrid[posA.x, posA.y] = TranslateTileType(tDataA.eType,
                            tDataA.rotation, tDataA.position, tDataB.position);
        }
        /// <summary>
        /// Translates to TileType from parameters.
        /// </summary>
        TileType TranslateTileType(eTileType eType,
            eRotation rotation, params Vector3Int[] positions)
        {
            switch (eType)
            {
                case eTileType.CORNER: //bonus?
                    return new Corner(positions[0], rotation);
                case eTileType.CROSS:
                    return new Cross(positions[0], rotation);
                case eTileType.INORMAL:
                    return new Straight(positions[0], rotation);
                case eTileType.IFAST:
                    return new Straight(positions[0], rotation, 1);
                case eTileType.ISLOW:
                    return new Straight(positions[0], rotation, 2);
                case eTileType.IBONUS:
                    return new Straight(positions[0], rotation, 3);
                case eTileType.START:
                    return new Start(positions[0], rotation, false);
                case eTileType.EXIT:
                    return new Start(positions[0], rotation, true);
                case eTileType.WALL:
                    return new Wall(positions[0], rotation);
                case eTileType.TELE:
                    return new Teleport(positions[1], positions[0], rotation);
                default: return null;
            }
        }
        /// <summary>
        /// Slows animationframerate. Invoked by Animationframerate class
        /// </summary>
        void DoAnimationFrame()
        {
            if (animationStep <= 0)
            {
                AnimateTiles();
                animationStep = 1;
                return;
            }
            animationStep -= animationFramerate;
        }
        /// <summary>
        /// AnimatesTiles according to Type.
        /// </summary>
        void AnimateTiles()
        {
            for (int i = animatedTiles.Count - 1; i >= 0; i--)
            {
                AnimatedTileData aTData = animatedTiles[i];
                if(aTData.animationCountpos < aTData.animationCountdown){  
                    aTData.animationCountpos += 1;
                    continue;}
                aTData.animationCountpos = 0; // aTData.animationCountdown;
                if (aTData.animType == eAnimType.WATER){
                    waterTilemap.SetTile(aTData.position,
                                  TranslateAnimTile(aTData, aTData.animFrame));
                    RotateTileAtPosition(waterTilemap, aTData.position, 
                                            aTData.rotation, aTData.isReversed,
                                aTData.animTileType == eAnimTileType.WCORNER);}
                else{
                    gameTilemap.SetTile(aTData.position, TranslateAnimTile
                                                   (aTData, aTData.animFrame));
                    RotateTileAtPosition(gameTilemap, aTData.position,
                                                             aTData.rotation);}
                aTData.animFrame += 1;
                if (aTData.animFrame >= aTData.animframeLast)
                {
                    switch (aTData.animType){
                        case eAnimType.ONCEFREEZE:
                                animatedTiles.Remove(aTData);
                                continue;
                        case eAnimType.ONCEDESTROY:
                                animatedTiles.Remove(aTData);
                                gameTilemap.SetTile(aTData.position, null);
                                continue;
                        case eAnimType.ENDLESS:
                                aTData.animFrame = 0;
                                continue;
                        case eAnimType.WATER:
                                createdPipes[aTData.position.x, 
                                                    aTData.position.y] = false;
                                waterManager.WaterStep();
                                animatedTiles.Remove(aTData);
                                continue;
                    }
                }
            }
        }

        /// <summary>
        /// Translates AnimTileType to Tile
        /// </summary>
        Tile TranslateAnimTile(AnimatedTileData aData, int framePos)
        {
            switch (aData.animTileType)
            {
                case eAnimTileType.BONUS:
                    return Database.tileSets[ePaths.BONUS][framePos];
                case eAnimTileType.EXPLOSION:
                    return Database.tileSets[ePaths.EXPLOSION][framePos];
                case eAnimTileType.WCORNER:
                    return Database.tileSets[ePaths.WCORNER][framePos];
                case eAnimTileType.WSTRAIGHT:
                    return Database.tileSets[ePaths.WSTRAIGHT][framePos];
                case eAnimTileType.WSTART:
                    return Database.tileSets[ePaths.WSTART][framePos];
                case eAnimTileType.WEND:
                    return Database.tileSets[ePaths.WEND][framePos];
                case eAnimTileType.WFAST:
                    return Database.tileSets[ePaths.WFAST][framePos];
                case eAnimTileType.WSLOW:
                    return Database.tileSets[ePaths.WSLOW][framePos];
                default: return null;
            }
        }

        /// <summary>
        /// Rotates and/or reverses tiles.
        /// </summary>
        public void RotateTileAtPosition(Tilemap tMap, Vector3Int position,
                          eRotation rotation, bool isReversed = false, bool isCorner = false)
        {
            tMap.SetTransformMatrix(position, Matrix4x4.TRS(Vector3.zero, 
                                              Quaternion.Euler(0,0, getRotation
                         ((int)rotation +(isReversed ? isCorner ? 0 : 2 : 0))), 
                                      new Vector3(isReversed ? -1 : 1, 1, 0)));
        }
        /// <summary>
        /// Translates int to rotation.
        /// </summary>
        float getRotation(int rot)
        {
            switch(rot)
            {
                default:
                    return 0;
                case 1:
                    return -90;
                case 2:
                    return -180;
                case 3:
                    return -270;
            }
        }

        /// <summary>
        /// Funktionen um den Cursor zu anmieren. Der Cursorframe wird 
        /// unabh�ngig von der Bewegung ge�ndert.
        /// </summary>
        public void DrawCursor(Vector3Int position)
        {
            if (position != cursorPosition)
            {
                gameTilemap.SetTile(cursorPosition, null);
                cursorPosition = position;
            }
            gameTilemap.SetTile(position, 
                     Database.tileSets[ePaths.CURSOR][cursorTilenum]);
        }
        void AnimateCursor()
        {
            if(cursorAnimPos >= cursorAnimRate)
            {
                cursorAnimPos = 0;
                if (cursorTilenum < cursorTiles.Length-1)
                    cursorTilenum += 1;
                else cursorTilenum = 0;
            }
            cursorAnimPos += 1;
        }


        //void CreateTestTiles()
        //{

        //    for (int i = 0; i < Database.tileSets.Count; i++)
        //    {
        //        int x = 0;
        //        foreach (Tile ti in Database.tileSets[(ePaths)i])
        //        {
        //            testTilemap.SetTile(new Vector3Int(i, x, 0), ti);
        //            x += 1;
        //        }
        //    }
        //}
    }
}