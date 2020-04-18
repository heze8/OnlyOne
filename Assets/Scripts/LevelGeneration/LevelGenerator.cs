using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour {

	public Texture2D map;
	public Tilemap tileMap;
	public Grid grid;
	public int chunkSize = 128;
	public bool active = true;
	private ColorToPrefab[] colorMappings;
	private List<Vector3> temp = new List<Vector3>();

	// Use this for initialization
	void Start () {
		if(active)
		{
			// if(map.width % chunkSize == 0)
			// 	Debug.Log("chunkSize will chip map length");
				
			// colorMappings = GameManager.Instance.colorMappings;
			GenerateLevel();
			Debug.Log("Starting");
		}
		
	}

	void GenerateLevel ()
	{
		for (int x = 0; x < map.width; x ++)
		{
			for (int y = 0; y < map.height; y ++)
			{
				GenerateTile(x, y);
			}
		}
		EnemyManager.Instance.r(temp);
	}

	void GenerateChunk(int r, int c)
	{
		for (int x = 0; x < chunkSize; x ++)
		{
			for (int y = 0; y < chunkSize; y ++)
			{
				GameObject child = new GameObject();
				child.transform.parent = grid.transform;
				Tilemap tm = child.gameObject.AddComponent<Tilemap>() as Tilemap;
			}
		}
	}

	void GenerateTile (int x, int y)
	{
		Color pixelColor = map.GetPixel(x, y);
		if (pixelColor.Equals(Color.red))
		{
			Vector3 temp2 = new Vector3();
			temp2.x = x;
			temp2.y = y;
			temp.Add(temp2);
		}
		return;
		/**
		if (pixelColor.a == 0)
		{
			// The pixel is transparrent. Let's ignore it!
			return;
		}
		
		foreach (ColorToPrefab colorMapping in colorMappings)
		{
			if (colorMapping.color.Equals(pixelColor))
			{
				if (colorMapping.tile != null)
				{ 
					Vector3Int position = new Vector3Int(x, y, 0);
					tileMap.SetTile(position, colorMapping.tile);
					return;
				}
				// else
				// {
				// 	Vector3Int position = new Vector3Int(x, y, 0);
				// 	Instantiate(colorMapping.prefab, position, Quaternion.identity, transform);
				// }
			}
		} */
	}
	
}
