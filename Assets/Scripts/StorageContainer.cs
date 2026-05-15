using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StorageContainer
{
    public string shelfName;

    public string worldTransform;

    public int parentID;


    public override string ToString()
    {
        return string.Format("[Shelf: Name={0},,  worldTransform={1}  ParentID={2}]", shelfName, worldTransform, parentID);
    }
}
