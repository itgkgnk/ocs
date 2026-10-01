# OSC Console v2.0

uOSC運用向けのWebコントロールサーフェスです。

## OSC規約

- uOSC Serverは共通箇所へ1つ配置
- ギミック／トリガー: Port `3335`
- フェーダーなど高頻度通信: Port `3336`（初期値）
- Address: 小文字、`/`区切り、英数字とハイフンのみ
- 1 Addressにつき原則1値
- 不正なAddress・型・Portは警告して送信しません

## UI

- シンプル: 初見向け4ch
- カスタム: 最大32ch、4chごとのBANK、DCA A〜D
- DCAは基準値へ倍率を掛けます。100と50をDCA 50%にすると50と25を送信します。
- 数値表示をタップすると直接値を入力できます。確定ボタンを押すまで送信しません。
- ロックはタップで有効、解除は700ms長押しです。
- 追従のやわらかさは0〜600msで調整できます。標準値は操作遅延を抑えた60msです。
- 触覚フィードバックはフェーダー接触、10%刻み、0/50/100、CENTER、ロック、数値確定、接続成功で発生します（iOSは試験機能）。
- 0/50/100はデテントとして正確な値へスナップし、最大値は必ず100へ到達します。
- Address、送信先IP、Port、パッチ、DCA、UI設定はブラウザのlocalStorageに保存します。

## 同一LAN

1. PCへNode.js LTSをインストールします。
2. `local-server/start-local.bat` を実行します。
3. 表示された `http://192.168.x.x:8080/` を同じルーターのiPhoneで開きます。
4. 接続モードを「同一LAN」にします。
5. OSC送信先IP、Port、各CHのAddressをブラウザで設定します。
6. UnityのuOscServerを同じPort（通常3336）で待ち受けます。

ブラウザはUDPを直接送れないため、ローカルブリッジがWebSocketをOSC/UDPへ変換します。インターネットやCloudflareは使用しません。送信先は安全のため、localhostまたはプライベートLANアドレスだけを許可しています。

## クラウド中継

1. `worker/`をCloudflare Workersへデプロイします。
2. Unity PCへ `unity/CloudOscBridge.cs` を追加します。
3. Worker URLと同じルームコードを設定します。
4. 初期状態ではOSC送信先を `127.0.0.1:3336`へ固定します。LAN内の別PCへ送る場合だけ `allowLanTargets` を有効化します。

## GitHub Pages

Settings → Pagesで `main` / `(root)` を公開します。更新後、右下が `v2.0.0`になっていることを確認してください。

## iOS

ダブルタップ／ピンチズームを抑止しています。操作音を使う場合は消音モードを解除し、ページを開いた後に一度タッチしてください。
