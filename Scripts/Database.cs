/*****************************************************************************
* Project: Pipesgame
* File   : Database.cs
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
*   20.05.2021	JA	Created
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace MainGame
{
    public enum eTileType {CORNER, CROSS, INORMAL, IFAST, ISLOW, IBONUS, START, 
                                                              EXIT, WALL, TELE}
    public enum eRotation { _0, _90, _180, _270}
    public enum eDirection { NORTH, EAST, SOUTH, WEST }
    public enum ePaths{ PIPES, SPECIAL, CURSOR, BACKGROUND, BONUS, EXPLOSION,
               WCORNER, WSTART, WEND,  WSTRAIGHT, WFAST, WSLOW, BORDER, SWITCH}
    public enum eAnimType { ONCEFREEZE, ONCEDESTROY, ENDLESS, WATER }
    public enum eAnimTileType { BONUS, EXPLOSION, WCORNER, WSTART, WEND, 
                                                      WSTRAIGHT, WFAST, WSLOW }
    /// <summary>
    /// Globale Datenbank die Referenzen speichert um wiederholte
    /// GetComponent Zugriffe zu vermeiden und um Tileordner einzulesen.
    /// </summary>
    public class Database : MonoBehaviour
    {
        static public Dictionary<ePaths, Tile[]> tileSets = new Dictionary
                                                           <ePaths, Tile[]>();
        static public List<int> s_switchList = new List<int>();
        static public AnimationFramerate sAnimFrame;
        static public GameManager sGameManager;
        static public SliderManager sSliderManager;
        static public MapManager sMapManager;
        static public WaterManager sWaterManager;
        static public PipeGiver sPipeGiver;
        static public AudioManager sAudioManager;
        static public CursorMovement sCursorMovement; 
        static public System.Random randMap,
                                    randPipeChooser;
        static public GameObject s_objAnimationFramerate,
                                 s_objGameManger,
                                 s_objSliderManager,
                                 s_objCursorMovement,
                                 s_objMapManager,
                                 s_objWaterManager,
                                 s_objPipeGiver,
                                 s_objAudioManger;
        [SerializeField]
        GameObject        objAnimationFramerate,
                          objGameManager,
                          objSliderManager,
                          objCursorMovement,
                          objMapManager,
                          objWaterManager,
                          objPipeGiver,
                          objAudioManger;
        int randomSeed;
        public int           cursorAnimRate = 1,
                             waterflowRate = 1,
                             difficultyIncreaseInterval = 3;
        public float         waterstartSliderSpeed = 0.02f,
                             animFrameRate = .1f,
                             pipeGiverFallSpeed = .2f;

        static public int    s_cursorAnimRate,
                             s_waterflowRate,
                             s_difficultyIncreaseInterval;
        static public float  s_waterstartSliderSpeed,
                             s_animFrameRate,
                             s_pipeGiverFallSpeed;

        static public Slider cSliderWaterstart;

        string[] paths = { "Pipes", "Special", "Cursor", "Background", "Bonus",
                                      "Explosion", "Wcorner", "Wstart", "Wend",
                             "Wstraight","Wfast", "Wslow", "Border", "Switch"};

        void Start()
        {
            SetObjects();
            GetComponents();
            CreateTilesetDictionary();
            ActivateObjects();
        }
        void CreateTilesetDictionary()
        {
            if (tileSets.Count == 0)
                for (int i = 0; i < paths.Length; i++)
                {
                    AddTileSets(paths[i], (ePaths)i);
                }
        }
        /// <summary>
        /// Weiﬂt die im Editor einstellbaren public Gameobjects den statischen 
        /// Referenzen zu weil es anders nicht funktioniert hat.
        /// </summary>
        void SetObjects()
        {
            randomSeed = Library.s_randomSeed;
            randMap = randomSeed != 0 ? new System.Random(randomSeed) :
                                       new System.Random();
            randPipeChooser = randomSeed != 0 ? new System.Random(randomSeed) :
                                                new System.Random();
            s_objAnimationFramerate = objAnimationFramerate;
            s_objGameManger = objGameManager;
            s_objCursorMovement = objCursorMovement;
            s_objSliderManager = objSliderManager;
            s_objMapManager = objMapManager;
            s_objWaterManager = objWaterManager;
            s_objPipeGiver = objPipeGiver;
            s_objAudioManger = objAudioManger;
            s_cursorAnimRate = cursorAnimRate;
            s_waterflowRate = waterflowRate;
            s_pipeGiverFallSpeed = pipeGiverFallSpeed;
            s_waterstartSliderSpeed = waterstartSliderSpeed;
            s_animFrameRate = animFrameRate;
            s_difficultyIncreaseInterval= difficultyIncreaseInterval;
        }

        /// <summary>
        /// Aktiviert alle anderen Objekte auﬂer der Database nachdem diese 
        /// fertig geladen hat um zu vermeiden das diese vorher versuchen 
        /// auf die Referenzen zuzugreifen.
        /// </summary>
        void ActivateObjects()
        {
            s_objAnimationFramerate.SetActive(true);
            s_objSliderManager.SetActive(true);
            s_objCursorMovement.SetActive(true);
            s_objMapManager.SetActive(true);
            s_objWaterManager.SetActive(true);
            s_objPipeGiver.SetActive(true);
            s_objAudioManger.SetActive(true);
            s_objGameManger.SetActive(true);
        }

        /// <summary>
        /// Weiﬂt die Komponenten zu.
        /// </summary>
        void GetComponents()
        {
            sAnimFrame = objAnimationFramerate.GetComponent<AnimationFramerate>();
            sGameManager = objGameManager.GetComponent<GameManager>();
            sSliderManager = objSliderManager.GetComponent<SliderManager>();
            sMapManager = objMapManager.GetComponent<MapManager>();
            sWaterManager = objWaterManager.GetComponent<WaterManager>();
            sPipeGiver = objPipeGiver.GetComponent<PipeGiver>();
            sAudioManager = objAudioManger.GetComponent<AudioManager>();
            sCursorMovement = objCursorMovement.GetComponent<CursorMovement>();
        }

        /// <summary>
        /// L‰d alle Tiles in dem Recources path in ein Dictionary mit 
        /// Ordnernamenverweis.
        /// </summary>
        void AddTileSets(string path, ePaths ePath)
        {
            var tiles = Resources.LoadAll<Tile>(path);
            tileSets.Add(ePath, tiles);
        }

        /// <summary>
        /// Returns one of the results with decreasing propability.
        /// </summary>
        /// <param name="load">1/chance propability of first result</param>
        /// <param name="results">Possible Results.</param>
        /// <returns></returns>
        static public int RollLoadedDice(System.Random rand, int load,
                                                  params int[] results)
        {
            if (load < 2) load = 2;
            int field = 1000;
            int diceRoll = rand.Next(field);
            for (int i = 0; i < results.Length; i++)
            {
                if (diceRoll >= field / (i + load))
                    return results[i];
            }
            return results[0];
        }
        /// <summary>
        /// Returns one of the results with decreasing propability
        /// </summary>
        /// <param name="load">1/chance propability of first result</param>
        /// <param name="length">Non inclusive max Result. Counting up.</param>
        /// <returns></returns>
        static public int RollLoadedDice( System.Random rand, int load, 
                                                                    int length)
                                                                   
        {
            if (load < 2) load = 2;
            int field = 1000;
            int diceRoll = rand.Next(field);
            for (int i = 0; i < length; i++)
            {
                if (diceRoll >= field / (i + load))
                    return i;
            }
            return 0;
        }
    }
}

