using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : Singleton<GameManager>
{
    public Tilemap tilemap;
    public ColorToPrefab[] colorMappings = LevelGenerator.colorMappings;

    public string HandleTileCollision(Vector3 pos)
    {
        Vector3Int intPos = new Vector3Int((int)pos.x, (int) pos.y, (int) pos.z);
        Color color = tilemap.GetColor(intPos);
        string name = null;
        
        foreach(ColorToPrefab colorMapping in colorMappings)
        {
            if (colorMapping.color.Equals(color))
			{
                name = colorMapping.tile.name;
                continue;
            }
        }

        switch(name)
        {
            case "FloorBlue":
                GameOver();
                break;

            case "FloorSticky":
                return "stick";
            default:
                break;
        }
        tilemap.SetTile(intPos, null);
        return null;
    }

    public void GameOver()
    {
        Debug.Log("GameOver");
    }
}
