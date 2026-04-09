# UE_Landscape_FP

Unreal Engine 5.7 を使ったランドスケープ・Foliage 学習プロジェクト。

---

## 進捗

### 環境構築
- [x] UE5.7 First Person テンプレートでプロジェクト作成
- [x] UnrealMCP プラグイン導入（Cursor・Claude Code 両対応）
- [x] GitHub 管理開始（Git LFS でバイナリ管理）

### Chapter 2: Grass Quick Start
- [x] Fab から草メッシュ（3D Plant）を入手
- [x] Landscape Grass Type アセットの作成・設定
- [x] Landscape マテリアル作成（UE5.7 推奨方式）
  - `SetMaterialAttributes` + `BlendMaterialAttributes` + `Landscape Layer Sample` + `Landscape Layer Switch`
- [x] Landscape Grass Output ノードで草の自動スポーン設定
- [x] ランドスケープへのマテリアル適用
- [x] Paint モードで草・岩レイヤーのペイント動作確認

---

## ドキュメント

| ファイル | 内容 |
|---------|------|
| [documents/textbook/02_Landscape_Grass.md](documents/textbook/02_Landscape_Grass.md) | Grass Quick Start テキストブック（UE5.7対応） |
| [documents/textbook/02_Landscape_Grass.html](documents/textbook/02_Landscape_Grass.html) | 同上（HTML版） |
| [documents/tips.md](documents/tips.md) | トラブルシューティング Tips |
| [documents/unreal-mcp-setup.md](documents/unreal-mcp-setup.md) | UnrealMCP 導入手順 |

---

## 環境

| 項目 | バージョン |
|------|----------|
| Unreal Engine | 5.7 |
| Visual Studio | 2022 (17.10+) |
| UnrealMCP | [chongdashu/unreal-mcp](https://github.com/chongdashu/unreal-mcp) |
