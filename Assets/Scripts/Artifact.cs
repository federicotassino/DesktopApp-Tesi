using SQLite4Unity3d;
using UnityEngine;

[System.Serializable]
public class Artifact  {

    //[PrimaryKey, AutoIncrement]
    public int id;

    public string name;

    public string textDescription;

    public int shelvingUnit = -1;

    public int lastShelvingUnit = -1;


    public override string ToString()
    {
        return string.Format("[Artifact: Id={0}, Name={1},  TextDescription={2},  ShelvingUnit={3}, LastShelvingUnit={4}]", 
            id, name, textDescription, shelvingUnit.ToString(), lastShelvingUnit.ToString());
    }
}
