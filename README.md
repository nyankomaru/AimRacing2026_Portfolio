# AimRacing2026_Portfolio

G923ハンドルコントローラーとWIZMOモーションチェアを使用した体験型レーシングゲームです。

このリポジトリでは、チーム制作内で自分が担当・改修したスクリプトを中心に掲載しています。  
実行データはGitHub Releasesにございます。

## 概要

AimRacing2026は、G923ハンドルコントローラーとWIZMOモーションチェアを使用した体験型レーシングゲームです。

展示環境での安定動作を想定し、G923のFFB制御、体験者別FFBモード切り替え、WIZMO安全停止、実機デバッグ表示、クラッチ無効化、外付けシフトレバー単独操作対応などを担当しました。

## 担当内容

- G923のFFB制御
- G923_FFBControllerのManager常駐化
- Child / Standard / StrongShake のFFBモード切り替え
- USB切断・再接続時の復帰処理
- 衝突時FFBの中継処理
- WIZMOモーションチェアの安全停止処理
- WIZMO状態確認用デバッグ表示
- Unity Profilerを用いたGC Alloc削減
- クラッチ完全無効化
- 外付けシフトレバー単独操作対応

## 使用技術

- Unity
- C#
- Unity Input System
- Logitech G SDK
- G923 Racing Wheel
- WIZMO Motion Chair

## 実行データ

実行データはGitHub Releasesに配置予定です。

## 操作方法

| 操作 | 入力 |
|---|---|
| ステアリング | G923ハンドル |
| アクセル | G923アクセルペダル |
| ブレーキ | G923ブレーキペダル |
| シフト操作 | 外付けシフトレバー |
| クラッチ | 使用しない |

## FFBモード切り替え

| キー | モード | 用途 |
|---|---|---|
| F6 | Child | 子供・小柄な体験者向け |
| F7 | Standard | 通常展示用 |
| F8 | StrongShake | 強めの演出用 |

## WIZMO安全操作

| キー | 内容 |
|---|---|
| F12 | WIZMO緊急停止 |
| Shift + F12 | WIZMO復帰 |

## Sourceについて

このリポジトリでは、チーム制作内で自分が担当・改修したスクリプトを中心に掲載しています。

既存の車両制御、SDK、他メンバー担当箇所については、必要な連携先として参照した範囲のみ記載しています。

## 担当ファイル

### G923 / FFB

- `G923_FFBController.cs`
  - G923のFFB制御
  - メニュー画面でもFFBを維持するManager常駐化
  - FFBモード切り替え
  - USB切断・再接続対応
  - 速度に応じたハンドル反力・振動調整

- `G923CollisionFFBRelay.cs`
  - Vehicle側の衝突情報をManager常駐のG923_FFBControllerへ中継

### WIZMO / Safety

- `ChairSafetyController.cs`
  - F12によるWIZMO出力の緊急停止
  - Shift + F12による復帰処理

- `ChairDebugOverlay.cs`
  - WIZMO出力値、Chair状態、Emergency Stop状態の画面表示
  - 実機確認用デバッグ表示の軽量化

### Input / Clutch / Shift

- `VehicleController.cs`
  - 既存の車両制御に対してクラッチ完全無効化処理を追加
  - クラッチペダル入力を無視し、自動クラッチ扱いに固定
  - 展示向けにクラッチ操作なしで走行できるよう調整

- `VehicleController_ManualLever.cs`
  - Trackモード時のクラッチ要求を無効化
  - 外付けシフトレバーだけでギア変更できるよう対応

- `VehicleUnityEvent.cs`
  - G923 / 外付けシフトレバー入力経路の確認
  - クラッチ無効化に伴う入力経路確認

### Performance

- `Profiler_GC_Optimization.md`
  - Unity ProfilerでGC Allocを確認
  - デバッグ表示や不要なオブジェクトを整理
  - 展示向けに長時間安定動作しやすい状態へ調整

## 注意事項

- WIZMO Motion Chairを使用する場合は、必ず安全確認を行ってから起動してください。
- F12キーでWIZMO出力を停止できます。
- G923が認識されない場合は、G HUBとWindows側の認識状態を確認してください。
- USBポートを変更した場合、入力やFFBの認識状態が変わることがあります。
- SDK本体、認証コード、他メンバー担当コード、有料アセットは掲載していません。
