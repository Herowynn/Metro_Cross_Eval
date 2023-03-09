using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveGhostPositions
{
    public float x;
    public float y;
    public float z;

    public SaveGhostPositions(Vector3 position)
    {
        x = position.x;
        y = position.y;
        z = position.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
