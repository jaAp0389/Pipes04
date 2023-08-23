using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

namespace MainGame
{
    public class _testscript : MonoBehaviour
    {
        public float mx, my, mz, sx, sy, sz, rx, ry, rz;
        public Tilemap tilemap;
        public Tile tile, tile2;
        public Tile[] animTiles;
        public Vector3Int position, animtposi;
        bool rotate = true;
        AnimationFramerate animframe;
        void Start()
        {
            Invoke("Set", 1f);
        }
        void Set()
        {
            animframe = Database.sAnimFrame;
            animTiles = Database.tileSets[ePaths.WCORNER];
            animframe.animationFrame.AddListener(AnimateTile);

        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                print("z");
                tilemap.SetTile(position, tile);
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                print("z");
                tilemap.SetTile(position, tile2);
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                rotate = !rotate;
                print(rotate);
            }
            if(rotate)
                RotateTileAtPosition();

        }

        public void RotateTileAtPosition()
        {
            tilemap.SetTransformMatrix(position, Matrix4x4.TRS(new Vector3(mx, my, mz),
                                              Quaternion.Euler(rx, ry, rz),
                                                new Vector3(sx, sy, sz)));
        }

        //AnimatedTileData animTile;
        int tpos = 0;

        public void AnimateTile()
        {
            tilemap.SetTile(animtposi, animTiles[tpos]);
            tpos += 1;
            if (tpos >= animTiles.Length) tpos = 0;
        }
    }
}
