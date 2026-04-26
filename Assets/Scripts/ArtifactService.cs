using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ArtifactService
{
    DB dB;

    public ArtifactService()
    {
        dB = new DB();
    }

    public void CreateArtifactTable()
    {
        dB.GetConnection().DropTable<Artifact>();
        dB.GetConnection().CreateTable<Artifact>();
    }

    public int AddArtifact(Artifact artifact)
    {
        return dB.GetConnection().Insert(artifact);
    }

    public IEnumerable<Artifact> GetAllArtifacts()
    {
        return dB.GetConnection().Table<Artifact>();
    }

    public Artifact GetArtifactByName(string name)
    {
        return dB.GetConnection().Table<Artifact>().Where(x => x.Name == name).FirstOrDefault();
    }
}
