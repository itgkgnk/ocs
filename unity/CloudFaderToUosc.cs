using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using uOSC;

public class CloudFaderToUosc : MonoBehaviour
{
    public string relayUrl = "wss://YOUR-WORKER.workers.dev";
    public string roomCode = "CHANGE-ME";
    public string addressPrefix = "/webfader/";
    public uOscClient oscClient;
    ClientWebSocket ws; CancellationTokenSource cts;
    readonly ConcurrentQueue<Message> queue = new();
    [Serializable] class Message { public string type; public int channel; public float value; }
    async void Start(){cts=new CancellationTokenSource();await ConnectLoop(cts.Token);}
    async Task ConnectLoop(CancellationToken token){while(!token.IsCancellationRequested){try{ws?.Dispose();ws=new ClientWebSocket();var baseUrl=relayUrl.TrimEnd('/').Replace("https://","wss://").Replace("http://","ws://");await ws.ConnectAsync(new Uri($"{baseUrl}/room/{roomCode}?role=unity"),token);await ReceiveLoop(token);}catch(Exception e){Debug.LogWarning(e.Message);}if(!token.IsCancellationRequested)await Task.Delay(5000,token);}}
    async Task ReceiveLoop(CancellationToken token){var b=new byte[2048];while(ws.State==WebSocketState.Open&&!token.IsCancellationRequested){var r=await ws.ReceiveAsync(new ArraySegment<byte>(b),token);if(r.MessageType==WebSocketMessageType.Close)break;var m=JsonUtility.FromJson<Message>(Encoding.UTF8.GetString(b,0,r.Count));if(m!=null&&m.type=="fader")queue.Enqueue(m);}}
    void Update(){while(queue.TryDequeue(out var m))if(oscClient)oscClient.Send(addressPrefix+m.channel,Mathf.Clamp01(m.value));}
    void OnDestroy(){cts?.Cancel();ws?.Dispose();cts?.Dispose();}
}
