# OSC Fader PWA v1.2

4chのWebフェーダーです。iOSでは物理振動の代わりに操作音と視覚フィードバックを使います。

## GitHub Pages

リポジトリの Settings → Pages で `main` / `(root)` を公開します。クラウドモードではCloudflare Workerを経由してUnityへ接続します。

## 同一LANモード（インターネット不要）

1. PCへNode.js LTSをインストールします。
2. `local-server/start-local.bat`を実行します。初回のみ `ws` をインストールします。
3. 表示された `http://192.168.x.x:8080/` を同じWi-FiのiPhoneで開きます。
4. 設定 → 接続モードを「同一LAN」にして保存します。同じページから開いた場合、URLは空欄でも接続できます。
5. Nodeサーバーは `/webfader/1`〜`/webfader/4` をfloat 0〜1で `127.0.0.1:9000` へOSC送信します。

変更例（PowerShell）:

```powershell
$env:OSC_HOST="127.0.0.1"
$env:OSC_PORT="9000"
$env:HTTP_PORT="8080"
node local-server/server.js
```

UnityではuOSCの `uOscServer` をポート9000で待ち受けます。

## クラウドモード

`worker/`をCloudflare Workersへデプロイし、PWAにWorker URLとルームコードを入力します。Unityでは `unity/CloudFaderToUosc.cs` とuOSCの `uOscClient`を同じGameObjectへ設定します。

## iPhoneで音が出ない場合

- iPhoneの消音モードを解除します。
- 音量を上げます。
- 設定の「操作音」をONにします。
- ページを開いた後、一度フェーダーへ触れます。最初のタッチでWeb Audioを有効化します。

## 注意

GitHub Pages（HTTPS）からローカルの `ws://` へは接続できません。同一LANモードでは必ずPCの `http://PCのIP:8080/` を開いてください。Cloudflareモードとローカルモードは設定で切り替えられます。
