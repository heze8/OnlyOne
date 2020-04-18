using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : Singleton<GameManager>
{
    public Tilemap tilemap;
    public ColorToPrefab[] colorMap = LevelGenerator.colorMappings;
    public string HandleTileCollision(Vector3 pos)
    {
        Vector3Int intPos = new Vector3Int((int)pos.x, (int) pos.y, (int) pos.z);
        Color color = tilemap.GetColor(intPos);
        Debug.Log("COlor Retrieved: " + color);
        string name = null;

        foreach(ColorToPrefab colorMapping in colorMap)
        {
            Debug.Log("Possible color: " + colorMapping.color);
            if (colorMapping.color.Equals(color))
			{
                Debug.Log("Color matched: " + colorMapping.color);
                Debug.Log("color in loop: " + color);
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
