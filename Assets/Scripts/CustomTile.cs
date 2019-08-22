using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName ="Custom Tile")]
public class CustomTile : Tile
{
    TileType _type;
    public TileType type {
        get {
            return _type;
        }
        set {
            sprite = Cache.instance.GetSprite(value.img,setSprite);
            //Debug.Log(sprite);
            _type = value;
        }
    }

    void setSprite(Sprite s) {
        sprite = s;
    }


    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = sprite;
        tileData.color = color;
        tileData.transform = transform;
        
        tileData.flags = flags;

        tileData.colliderType = colliderType;
    }
}
