using NativeWebSocket;
using System.Net.WebSockets;
using System.Text;
using UnityEngine;

public class DesktopWebSocketClient : MonoBehaviour
{
    private NativeWebSocket.WebSocket ws;
    private string url = "ws://127.0.0.1:5000/ws";

    public MainScene mainScene;

    async void Start()
    {
        ws = new NativeWebSocket.WebSocket(url);

        ws.OnOpen += () =>
        {
            Debug.Log("Desktop WS connected");
        };

        ws.OnError += (e) =>
        {
            Debug.LogError("WS error: " + e);
        };

        ws.OnClose += (e) =>
        {
            Debug.Log("WS closed");
        };

        ws.OnMessage += (bytes) =>
        {
            string json = Encoding.UTF8.GetString(bytes);

            Debug.Log("Ricevuto: " + json);

            // parse base message
            WSMessage baseMsg = JsonUtility.FromJson<WSMessage>(json);

            if (baseMsg == null)
            {
                Debug.Log("WSMessage NULL");
                return;
            }

            Debug.Log("EVENT TYPE: " + baseMsg.eventType);

            switch (baseMsg.eventType)
            {
                //UPDATE
                case "update":

                    switch (baseMsg.entityType)
                    {
                        // ARTIFACT UPDATE
                        case "artifact":

                            Debug.Log("WebSocket artifact update");

                            Artifact updatedArtifact =
                                JsonUtility.FromJson<Artifact>(json);

                            UnityMainThreadDispatcher.Instance().Enqueue(() =>
                            {
                                mainScene.UpdateArtifact(updatedArtifact);
                            });

                            break;

                        // SHELF UPDATE
                        case "shelf":

                            Debug.Log("WebSocket shelf update");

                            StorageContainer updatedShelf =
                                JsonUtility.FromJson<StorageContainer>(json);

                            UnityMainThreadDispatcher.Instance().Enqueue(() =>
                            {
                                mainScene.UpdateShelf(updatedShelf);
                            });

                            break;
                    }

                    break;

                default:

                    Debug.LogWarning("Evento WS sconosciuto: " + baseMsg.eventType);

                    break;
            }

        };

        await ws.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        ws?.DispatchMessageQueue();
#endif
    }

    async void OnApplicationQuit()
    {
        if (ws != null)
            await ws.Close();
    }
}