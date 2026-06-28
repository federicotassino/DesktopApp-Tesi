using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StorageContainer
{
    public string shelfName;

    public string worldTransform;

    public int parentID;

    public bool isShelf = false;

    // indica se è una stanza (con marker)
    public bool isRoom = false;

    // riferimento al marker ArUco
    public int markerId;

    // dimensioni stanza per collider
    public float roomWidth;
    public float roomHeight;
    public float roomDepth;

    // coordinate centro stanza
    public string roomCenterPose;


    public override string ToString()
    {
        return string.Format("[Shelf: Name={0},,  worldTransform={1}  ParentID={2}]", shelfName, worldTransform, parentID);
    }
}
