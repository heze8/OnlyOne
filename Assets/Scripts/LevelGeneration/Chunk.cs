using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk 
{
    public static int xSize = 128;
    public static int ySize = 128;
    private Vector2 location;

    public int [,] tiles = new int[xSize, ySize];

    public Chunk(int[,] voxels) 
    {
        this.tiles = voxels;
    }

    public void GenerateChunks() 
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if (tiles [i, j] != 0)
                {

                }
            }
        }
    }

}
