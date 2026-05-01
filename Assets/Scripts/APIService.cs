using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class APIService
{
    private string baseUrl = "http://127.0.0.1:5000/dati";

    // =========================
    // GET ALL
    // =========================
    public async Task<List<Artifact>> GetAllArtifactsAsync()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(baseUrl))
        {
            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Errore: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"URL: {request.url}");
                return null;
            }

            string json = request.downloadHandler.text;

            string wrappedJson = "{ \"items\": " + json + " }";

            ArtifactListWrapper result =
                JsonUtility.FromJson<ArtifactListWrapper>(wrappedJson);

            return result.items;
        }
    }

    // =========================
    // GET BY ID
    // =========================
    public async Task<Artifact> GetArtifactByIdAsync(int id)
    {
        string url = baseUrl + "/" + id;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Errore: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"URL: {request.url}");
                return null;
            }

            string json = request.downloadHandler.text;

            Artifact artifact = JsonUtility.FromJson<Artifact>(json);

            return artifact;
        }
    }

    [System.Serializable]
    private class ArtifactListWrapper
    {
        public List<Artifact> items;
    }
}