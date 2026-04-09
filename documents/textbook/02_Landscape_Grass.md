# UE5 Landscape テキストブック: Chapter 2 - Grass Quick Start

この章では、Landscape Grass System を使って、ランドスケープ上に草などの植生を自動的に配置する方法を学びます。手動でFoliageを配置するのではなく、マテリアルのペイントレイヤーに連動して草が**プロシージャルに生成される**仕組みを習得します。

> **対象バージョン:** Unreal Engine 5.7
> **参考:** [Grass Quick Start - UE5.7 公式ドキュメント](https://dev.epicgames.com/documentation/en-us/unreal-engine/grass-quick-start-in-unreal-engine)

---

## UE5.7 での変更点（重要）

UE5.7 では Landscape マテリアルの推奨方式が変わっています。

| 項目 | 旧方式（Legacy） | UE5.7 推奨方式 |
|------|----------------|---------------|
| レイヤーブレンド | `Landscape Layer Blend` | `SetMaterialAttributes` + `BlendMaterialAttributes` |
| レイヤー識別 | Layer Blend 内で定義 | `Landscape Layer Sample` で定義 |
| 最適化 | なし | `Landscape Layer Switch` |
| LayerInfo | Weight-Blend を選択 | デフォルトが Non-Weight-Blend（要設定変更） |

> **注意:** 旧方式（Landscape Layer Blend）はマテリアルエディターで **Legacy** と表示されます。このテキストでは UE5.7 推奨方式で進めます。

---

## システムの概要

### 動作の仕組み

```
Landscape Material
  ↓ Landscape Layer Sample（レイヤー識別）
  ↓ BlendMaterialAttributes（レイヤー合成）
  ↓ Landscape Grass Output ノード
Landscape Grass Type アセット（草のメッシュ・密度・カリング設定）
  ↓
HISM（Hierarchical Instanced Static Mesh）として動的にスポーン
```

### 重要な制約

- Grass System は **Landscape Terrain Actor にのみ対応**している
- 他のActorタイプには使用不可

---

## Step 0 — 草アセットの準備（Fab から入手）

草メッシュは **Fab**（旧 Quixel Bridge）から入手できます。

### Fab からのダウンロード手順

1. UE Editor で **Window → Fab** を開く
2. 草や植生アセットを検索（例: `Tundra Grass`）
3. ダウンロードしてプロジェクトに追加

### よくあるエラー

| エラー | 原因 | 対処 |
|--------|------|------|
| `Asset not available in UAsset format` | アセットがUAsset形式に未変換 | fab.com のブラウザから検索する |
| `Missing Plugin` | 必要なプラグインが無効 | 表示されたプラグインを Enable → エディター再起動 |

---

## Step 1 — レベルとランドスケープの作成

1. **新規レベル**を作成（Blank テンプレートを使用）
2. Modes から **Landscape** を選択
3. **Create** ボタンをクリックしてランドスケープを生成
4. （推奨）Sculpt タブ → **Noise** ツールで自然な起伏を追加

### Noise ツールの適用

| 操作 | 内容 |
|------|------|
| Sculpt タブ → Noise | ノイズツールを選択 |
| ブラシをLandscape全体に広げる | ビューポートでブラシサイズを拡大 |
| 左クリック × 3〜4回 | 自然な起伏を追加 |

---

## Step 2 — Landscape Grass Type アセットの作成

Landscape Grass Type は、草のメッシュ・密度・カリング距離などをまとめたアセット。

1. Content Browser で右クリック → **Miscellaneous → Landscape Grass Type**
2. `LGT_FieldGrass` と命名してダブルクリックで開く
3. **Grass Varieties** 配列の **+** アイコンをクリックして要素を追加
4. **Grass Mesh** 欄に使用するスタティックメッシュを設定

### Grass Variety の主要設定

| 設定項目 | 説明 | 推奨値 |
|---------|------|--------|
| **Grass Density** | 生成密度 | `400`（作業中は下げる） |
| **Start Cull Distance** | フェードアウト開始距離 | End より小さい値 |
| **End Cull Distance** | この距離を超えると非表示 | `4000`（約40m） |
| **Random Rotation** | ランダムな回転を有効化 | ✅ オン |
| **Align to Surface** | 地形の法線に沿って傾ける | ✅ 草・花・岩はオン / ❌ 木はオフ |
| **Scale X/Y/Z Min・Max** | サイズのランダム幅 | プロジェクトに応じて設定 |

> **パフォーマンスTips:** Scale を大きくすると Density を下げられ、メッシュ数を減らしながら見た目を維持できる。

---

## Step 3 — Landscape マテリアルの作成（UE5.7 推奨方式）

Content Browser で右クリック → **Material** を選択し、`MAT_Landscape` と命名。

### 必要なノード一覧

| ノード名 | 役割 |
|---------|------|
| **SetMaterialAttributes** | 各レイヤーのマテリアル属性（Base Color等）を定義 |
| **BlendMaterialAttributes** | 2つのレイヤーをブレンド |
| **Landscape Layer Sample** | ペイントされたレイヤー値（Alpha）を取得 |
| **Landscape Layer Switch** | 未使用レイヤーをスキップしてパフォーマンス最適化 |
| **Texture Sample** | テクスチャを読み込む |
| **Landscape Grass Output** | 草の自動スポーンを駆動 |

### マテリアルの設定

メインマテリアルノードを選択して Details パネルの **Use Material Attributes** にチェックを入れる。

### ノード接続（Grass レイヤー）

1. `SetMaterialAttributes` を追加 → Details の **+** で `Base Color` を追加
2. `Texture Sample`（草テクスチャ）→ `SetMaterialAttributes` の **Base Color** に接続
3. `Landscape Layer Sample` を追加 → Parameter Name を `Grass` に設定
4. `BlendMaterialAttributes` を追加して接続：
   - `SetMaterialAttributes` → **B** ピン
   - `Landscape Layer Sample` → **Alpha** ピン
5. `Landscape Layer Switch` を追加して接続：
   - `BlendMaterialAttributes` → **Layer Used** ピン
   - Parameter Name を `Grass` に設定

### ノード接続（Rock レイヤー）

Rock レイヤーも同様に構築し、以下で2つのレイヤーを繋げる：

- Grass の `Landscape Layer Switch` 出力 → Rock の `BlendMaterialAttributes` の **A** ピン
- Grass の `Landscape Layer Switch` 出力 → Rock の `Landscape Layer Switch` の **Layer Not Used** ピン
- Rock の `Landscape Layer Switch` 出力 → メインマテリアルノードの **Material Attributes** ピン

### 接続図

```
[TextureSample(Grass)] → [SetMaterialAttributes] ─┐
[LandscapeLayerSample(Grass)] ──────────────────→ [BlendMaterialAttributes] → [LandscapeLayerSwitch(Grass)] ─┐
                                                                                                               │
[TextureSample(Rock)]  → [SetMaterialAttributes] ─┐                                                          │ (A)
[LandscapeLayerSample(Rock)]  ──────────────────→ [BlendMaterialAttributes] → [LandscapeLayerSwitch(Rock)] → Output
                                                                               LayerNotUsed ←────────────────┘
```

### Landscape Grass Output の追加

1. `Landscape Grass Output` ノードを追加
2. Details パネルで Grass Type に `LGT_FieldGrass` を設定
3. `Landscape Layer Sample`（Grass）の出力 → `Landscape Grass Output` の入力に接続

### 保存

**Apply → Save** してコンパイルする。

---

## Step 4 — Project Settings の変更（UE5.7 必須）

UE5.7 ではデフォルト設定のままではペイントが機能しない。

1. **Edit → Project Settings** を開く
2. 検索欄に `Target Layer` と入力
3. **Target Layer Default Blend Method** を **Weight-Blending** に変更
4. エディターを**再起動**

> **補足:** Weight-Blending は Legacy 扱いだが、UE5.7 でペイントを動かすために必要。

---

## Step 5 — ランドスケープへの適用とレイヤーペイント

1. ビューポートでランドスケープを選択
2. Details パネルの **Landscape Material** に `MAT_Landscape` を設定
3. **Landscape Paint Mode** に切り替え
4. **Populate** ボタンをクリックしてレイヤーを読み込む
5. LayerInfo の保存先を指定して保存
6. レイヤーを選択してペイント → 草が自動でスポーンされる

---

## Step 6 — スムーズフェードの設定（草のポップイン防止）

カリング距離で草が突然消えるのを防ぐには、草メッシュ用マテリアルに **`PerInstanceFadeAmount`** ノードを使用する。

1. 草メッシュのマテリアルを開く
2. **PerInstanceFadeAmount** ノードを追加（0〜1の値を出力）
3. **Opacity** 入力に接続してフェードアウトを実現

> Start Cull Distance < End Cull Distance の設定時に有効化される。

---

## 最適化設定まとめ

| 設定 | 推奨値 |
|------|--------|
| Grass Density | ~400（作業中は低く設定） |
| End Cull Distance | 4000 UU（40m）|
| Start Cull Distance | End より小さい値 |
| Align to Surface | 草・花はオン、木はオフ |
| Random Rotation | オン |
| PerInstanceFadeAmount | 使用推奨 |

---

## よくある問題

### 草が表示されない
- Landscape Grass Output ノードに Landscape Grass Type が正しく設定されているか確認
- マテリアルが Apply・Save されているか確認
- Landscape Actor に正しくマテリアルが割り当てられているか確認

### ペイントが効かない / 何も変化しない（UE5.7）
- Project Settings → `Target Layer Default Blend Method` を **Weight-Blending** に変更してエディター再起動

### エディターが重くなる・フリーズする
- 作業中は **Grass Density を低い値**（例: 10〜50）に下げて作業し、完成時に戻す

### 草が地面に沈む・浮く
- **Align to Surface** を有効にする
- メッシュのピボットポイントが地面に設置されているか確認

---

## 参考リンク

- [Grass Quick Start - UE5.7 公式ドキュメント](https://dev.epicgames.com/documentation/en-us/unreal-engine/grass-quick-start-in-unreal-engine)
- [Landscape Materials in UE5.7](https://dev.epicgames.com/documentation/en-us/unreal-engine/landscape-materials-in-unreal-engine)
- [UE5 Landscape Grass Source Analysis](https://wh0.is/posts/a-look-under-the-hood-at-unreal-engine-landscape-grass-en)
