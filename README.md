# OSC Fader PWA

iPhone/iPadのホーム画面に追加できる4chフェーダーです。ブラウザ → WebSocketリレー → Unity → OSCで値を送ります。

## 1. GitHub Pagesへ公開

1. このフォルダーの中身をGitHubの新規リポジトリへアップロードします。
2. リポジトリの **Settings → Pages → Source** を **GitHub Actions** にします。
3. `main`へpushすると `.github/workflows/pages.yml` が公開します。
4. 表示された `https://ユーザー名.github.io/リポジトリ名/` をiPhoneのSafariで開きます。
5. 共有ボタン → **ホーム画面に追加** を選びます。

## 2. CloudflareのWebSocketリレーを公開

GitHub PagesだけではWebSocketサーバーを動かせないため、`worker/`をCloudflare Workersへ配置します。

```bash
cd worker
npx wrangler login
npx wrangler deploy
```

表示された `https://osc-fader-relay....workers.dev` をPWAの接続設定へ入力します。必要に応じてCloudflareのWorker変数 `ALLOWED_ORIGIN` にGitHub PagesのOriginを設定してください。

> `wrangler`、Durable Objectsの料金・無料枠・利用条件はCloudflareの最新情報を確認してください。

## 3. Unity側

1. `unity/OscFaderReceiver.cs` をUnityプロジェクトへコピーします。
2. 空のGameObjectへ追加します。
3. `Relay Url`、`Room Code`、OSCの宛先IP／ポートを設定します。
4. iPhone側にも同じRoom Codeを入力します。

送信OSCアドレスは初期状態で以下です。

- `/webfader/1`〜`/webfader/4`
- 引数は0.0〜1.0のfloat

## セキュリティ

ルームコードを知っている端末は参加できます。本番運用では長く推測しにくいコード、接続承認、認証、レート制限を追加してください。OSC UDPポートをインターネットへ直接公開しないでください。

## iOSの制限

iOSのブラウザ/PWAは一般的なVibration APIを利用できません。このUIでは目盛り通過時のクリック音と視覚効果を使います。最初の音はユーザー操作後に有効になります。

## ローカル確認

Service WorkerはHTTP/HTTPSで確認します。

```bash
python3 -m http.server 8080
```

`http://localhost:8080` を開いてください。

OSC Fader PWA
