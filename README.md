# Rhythm
[Google Play](https://play.google.com/store/apps/details?id=jp.renjaku.rhythm) と [App Store](https://itunes.apple.com/jp/app/id1225018600) に公開しているリズムゲームです。  
[Unity](https://unity3d.com) で制作しています。

## 全体的な設定
ゲームの全体的な設定を [Assets/GlobalSettings](https://github.com/maesin/rhythm/blob/master/Assets/GlobalSettings.asset) で行います。

設定できる項目は次の通り:
- 追加でダウンロードしたい譜面リストの URL
- AdMob 広告ユニット ID
- AdMob テスト端末 ID

これらの設定は省略しても動作します。  
譜面リストについては、[リソースサーバー](https://github.com/maesin/rhythm/tree/gh-pages)を参照ください。

## ノートの判定
ノートの判定について、[Assets/Ratings](https://github.com/maesin/rhythm/tree/master/Assets/Ratings) で設定できます。

設定できる項目は次の通り:
- 判定時の獲得スコア
- フレーム単位の判定幅
- 判定時に表示するスプライト
- 判定時のヒット音

## 譜面データ
譜面データは、[Assets/Resources/MusicScores](https://github.com/maesin/rhythm/tree/master/Assets/Resources/MusicScores) に配置します。  
[Assets/GlobalSettings](https://github.com/maesin/rhythm/blob/master/Assets/GlobalSettings.asset) で URL を設定すると追加で譜面をダウンロードできます。

### ヘッダーの書式
- #Title: タイトル
- #Priority: 優先度 (表示時の並び順)

### タイミングの書式
```
#小節数:タイミング:レーン
```
小節数は、0 から開始しますが、通常は 1 小節置いてから開始してください。

タイミングは、小節あたりの拍数を桁数で表すと同時にノートの種類も指定します。

番号とノートの種類は次の通り:
- 0: 休符
- 1: ノーマル (タップ)
- 2: ロング (開始後、同じレーンの後続ノートに接続)

レーンは、ノートの到着地点のことで 1 から 5 までの数字を指定します。
