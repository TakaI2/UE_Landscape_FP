# UE5 Landscape テキストブック: Chapter 2 - Grass Quick Start

この章では、Landscape Grass System を使って、ランドスケープ上に草などの植生を自動的に配置する方法を学びます。手動でFoliageを配置するのではなく、マテリアルのペイントレイヤーに連動して草が**プロシージャルに生成される**仕組みを習得します。

> **対象バージョン:** Unreal Engine 5.7
> **参考:** [Grass Quick Start - UE5.7 公式ドキュメント](https://dev.epicgames.com/documentation/en-us/unreal-engine/grass-quick-start-in-unreal-engine)

---

## システムの概要

### 動作の仕組み

```
Landscape Material
  ↓ Landscape Grass Output ノード
Landscape Grass Type アセット（草のメッシュ・密度・カリング設定）
  ↓
HISM（Hierarchical Instanced Static Mesh）として動的にスポーン
```

### 重要な制約

- Grass System は **Landscape Terrain Actor にのみ対応**している
- 他のActorタイプには使用不可

---

## Step 1 — レベルとランドスケープの作成

### 手順

1. **新規レベル**を作成（Blank テンプレートを使用）
2. Modes から **Landscape** を選択し、Landscapeパネルを表示
3. **Create** ボタンをクリックしてランドスケープを生成
4. （推奨）Landscapeが真っ平らだと草の見栄えが悪いため、**Noise** ツールで起伏を追加する

### Noise ツールの適用

| 操作 | 内容 |
|------|------|
| Sculpt タブ → Noise | ノイズツールを選択 |
| ブラシをLandscape全体に広げる | ビューポートでブラシサイズを拡大 |
| 左クリック × 3〜4回 | 自然な起伏を追加 |

---

## Step 2 — Landscape マテリアルの作成

### マテリアルの新規作成

Content Browser で右クリック → **Material & Textures → Material** を選択し、`MAT_GT_Grass` と命名。

### 必要なノード一覧

| ノード名 | 役割 |
|---------|------|
| **Landscape Layer Blend** | 複数テクスチャ（草・岩など）をレイヤーでブレンド |
| **Landscape Layer Sample** | ペイントされたレイヤー値を取得 |
| **Landscape Grass Output** | 草の自動スポーンを駆動する重要ノード |
| **Landscape Layer Coords** | テクスチャのタイリング制御 |

### ノード接続の手順

1. **Landscape Layer Coords** を追加し、すべてのテクスチャの UV 入力に接続（統一タイリング）
2. **Landscape Layer Blend** を追加し、Details パネルの Layers に `Grass` と `Rock` の2つを追加（+ アイコンを2回クリック）
3. 各テクスチャを対応するLayer入力に接続
4. Layer Blend の出力を **Base Color** に接続
5. **Landscape Grass Output** ノードを追加（後のStepで設定）

### Height Blend（任意・推奨）

岩や土のレイヤーにリアルな境界を出すには、Landscape Layer Blend の Details パネルでそれらのレイヤータイプを `Weight Blend` から **`Height Blend`** に変更する。

---

## Step 3 — Landscape Grass Type アセットの作成

Landscape Grass Type は、草のメッシュ・密度・カリング距離などをまとめたアセット。

### 作成手順

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

## Step 4 — マテリアルに Grass Output ノードを接続

### 接続手順

1. `MAT_GT_Grass` を開く
2. **Landscape Grass Output** ノードの Grass Type スロットに `LGT_FieldGrass` を設定
3. **Landscape Layer Sample** ノードを追加し、Parameter Name を `Grass` に設定
4. Layer Sample の出力を Grass Output の入力に接続

### 複数の草タイプを追加する場合

Grass Output ノードの Details パネルで **+ アイコン**をクリックし要素を追加。別の Landscape Grass Type（例: 花用）を割り当てて、対応する Landscape Layer Sample を接続する。

### 保存

マテリアルを **Apply → Save** してコンパイルする。

---

## Step 5 — ランドスケープへのマテリアル適用とレイヤーペイント

1. ビューポートでランドスケープを選択
2. Details パネルの **Landscape Material** に `MAT_GT_Grass` を設定
3. **Landscape Paint Mode** に切り替え、草レイヤーを選択してペイント

> ペイントした領域に草が自動でスポーンされる。
> 別のレイヤー（土・岩）をペイントすると草が消え、そのレイヤーに対応する Grass Type が表示される。

---

## Step 6 — スムーズフェードの設定（草のポップイン防止）

カリング距離で草が突然消えるのを防ぐには、草メッシュ用マテリアルに **`PerInstanceFadeAmount`** ノードを使用する。

### 設定方法

1. 草メッシュのマテリアルを開く
2. **PerInstanceFadeAmount** ノードを追加（0〜1の値を出力）
3. Opacity 入力に接続してフェードアウトを実現

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

### エディターが重くなる・フリーズする
- 作業中は **Grass Density を低い値**（例: 10〜50）に下げて作業し、完成時に戻す

### 草が地面に沈む・浮く
- **Align to Surface** を有効にする
- メッシュのピボットポイントが地面に設置されているか確認

---

## 参考リンク

- [Grass Quick Start - UE5.7 公式ドキュメント](https://dev.epicgames.com/documentation/en-us/unreal-engine/grass-quick-start-in-unreal-engine)
- [UE5 Landscape Grass Source Analysis](https://wh0.is/posts/a-look-under-the-hood-at-unreal-engine-landscape-grass-en)
- [Complete Grass Output Node Guide](https://www.worldofleveldesign.com/categories/ue4/grass-output-node-guide.php)
