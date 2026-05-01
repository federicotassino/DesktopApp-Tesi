using SQLite4Unity3d;
using UnityEngine;

[System.Serializable]
public class Artifact  {

    //[PrimaryKey, AutoIncrement]
    public int id;

    public string name;

    public string textDescription;

    public int shelvingUnit;


    public override string ToString()
    {
        return string.Format("[Artifact: Id={0}, Name={1},  TextDescription={2},  ShelvingUnit={3}]", id, name, textDescription, shelvingUnit);
    }
}
