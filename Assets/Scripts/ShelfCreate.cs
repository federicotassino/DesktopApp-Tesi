using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShelfCreate
{
    public string name;
    public string worldTransform;
    public int parentShelfId;
    public bool isShelf = false;

    // indica se ha un trigger
    public bool isRoom = false;

    // riferimento al marker ArUco
    public int markerId;

    // dimensioni stanza per collider
    public float roomWidth;
    public float roomHeight;
    public float roomDepth;

    // coordinate centro stanza
    public string roomCenterPose;
}
