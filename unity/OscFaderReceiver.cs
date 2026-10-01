using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class OscFaderReceiver : MonoBehaviour
{
    [Header("Web relay")]
    public string relayUrl = "wss://YOUR-WORKER.workers.dev";
    public string roomCode = "482913";
    [Header("OSC output")]
    public string oscHost = "127.0.0.1";
    public int oscPort = 9000;
    public string addressPrefix = "/webfader/";

    ClientWebSocket ws;
    CancellationTokenSource cts;
    readonly ConcurrentQueue<FaderMessage> queue = new();
    UdpClient udp;

    [Serializable] class FaderMessage { public string type; public int channel; public float value; public bool final; public long at; }

    async void Start()
    {
        udp = new UdpClient();
        cts = new CancellationTokenSource();
        await ConnectLoop(cts.Token);
    }

    async Task ConnectLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                ws?.Dispose(); ws = new ClientWebSocket();
                var uri = new Uri($"{relayUrl.TrimEnd('/')}/room/{roomCode}?role=unity");
                await ws.ConnectAsync(uri, token);
                Debug.Log("OSC Fader relay connected");
                await ReceiveLoop(token);
            }
            catch (Exception e) { Debug.LogWarning(e.Message); }
            if (!token.IsCancellationRequested) await Task.Delay(5000, token);
        }
    }

    async Task ReceiveLoop(CancellationToken token)
    {
        var buffer = new byte[2048];
        while (ws.State == WebSocketState.Open && !token.IsCancellationRequested)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), token);
            if (result.MessageType == WebSocketMessageType.Close) break;
            var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var msg = JsonUtility.FromJson<FaderMessage>(json);
            if (msg != null && msg.type == "fader") queue.Enqueue(msg);
        }
    }

    void Update()
    {
        while (queue.TryDequeue(out var msg))
            SendOsc(addressPrefix + msg.channel, Mathf.Clamp01(msg.value));
    }

    void SendOsc(string address, float value)
    {
        byte[] a = OscString(address), t = OscString(",f"), f = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian) Array.Reverse(f);
        var packet = new byte[a.Length + t.Length + 4];
        Buffer.BlockCopy(a, 0, packet, 0, a.Length);
        Buffer.BlockCopy(t, 0, packet, a.Length, t.Length);
        Buffer.BlockCopy(f, 0, packet, a.Length + t.Length, 4);
        udp.Send(packet, packet.Length, oscHost, oscPort);
    }

    static byte[] OscString(string value)
    {
        var raw = Encoding.UTF8.GetBytes(value + "\0");
        int size = (raw.Length + 3) & ~3;
        var padded = new byte[size]; Buffer.BlockCopy(raw, 0, padded, 0, raw.Length); return padded;
    }

    async void OnDestroy()
    {
        cts?.Cancel();
        if (ws?.State == WebSocketState.Open)
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None);
        ws?.Dispose(); udp?.Dispose(); cts?.Dispose();
    }
}