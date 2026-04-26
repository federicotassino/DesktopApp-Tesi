using SQLite4Unity3d;
using UnityEngine;

public class Artifact  {

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public string TextDescription { get; set; }
    public int ShelvingUnit { get; set; }


    public override string ToString()
    {
        return string.Format("[Artifact: Id={0}, Name={1},  TextDescription={2},  ShelvingUnit={3}]", Id, Name, TextDescription, ShelvingUnit);
    }
}
