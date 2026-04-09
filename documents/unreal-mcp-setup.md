# Unreal MCP 導入手順

## 概要

CursorからUnreal Engineを自然言語で操作するためのMCP（Model Context Protocol）導入手順。

### 動作の仕組み

```
Cursor
  ↓ MCP経由でコマンド送信
Python サーバー (D:/dev/unreal-mcp/Python)
  ↓ TCP通信 (ポート55557)
UnrealMCP プラグイン (UE Editor内で動作)
  ↓
Unreal Engine を操作
```

### できること

- アクターの作成・削除・トランスフォーム操作
- Blueprintクラスの作成・コンポーネント追加
- Blueprintノードグラフの編集
- ビューポート・カメラ操作

---

## 前提条件

| 項目 | バージョン |
|------|-----------|
| Unreal Engine | 5.5以上（本プロジェクトは5.7） |
| Python | 3.12以上 |
| uv | 任意のバージョン（`pip install uv` でインストール可） |
| Visual Studio 2022 | 17.10以上（C++によるデスクトップ開発ワークロード必須） |
| MCPクライアント | Cursor |

---

## セットアップ手順

### 1. リポジトリのクローン

```bash
git clone https://github.com/chongdashu/unreal-mcp.git D:/dev/unreal-mcp
```

### 2. プラグインのコピー

`D:/dev/unreal-mcp/MCPGameProject/Plugins/UnrealMCP` を `<プロジェクト>/Plugins/` へコピー。

```
Landscape_FP/
└── Plugins/
    └── UnrealMCP/   ← ここへコピー
```

### 3. C++モジュールの追加

Blueprint専用プロジェクトはC++プラグインをビルドできないため、最小限のC++モジュールを手動追加する。

**作成するファイル:**

```
Source/
├── Landscape_FP/
│   ├── Landscape_FP.Build.cs
│   └── Landscape_FP.cpp
├── Landscape_FP.Target.cs
└── Landscape_FPEditor.Target.cs
```

**Landscape_FP.uproject にモジュール定義を追加:**

```json
"Modules": [
    {
        "Name": "Landscape_FP",
        "Type": "Runtime",
        "LoadingPhase": "Default"
    }
]
```

### 4. Visual Studio プロジェクトファイルの生成

`.uproject` を右クリック → **"Generate Visual Studio project files"**

> **注意:** VS2022 が 17.10 未満の場合は「VisualStudioUnsupported」と判定されエラーになる。Visual Studio Installer で最新版に更新すること。

### 5. ビルド

Visual Studio で `.sln` を開き、構成を `Development Editor` / `Win64` に設定してビルド。

### 6. UE Editor でプラグインを有効化

Edit → Plugins → `UnrealMCP` を検索 → 有効化 → エディター再起動

### 7. Cursor の MCP 設定

`C:/Users/<ユーザー名>/.cursor/mcp.json` を作成：

```json
{
  "mcpServers": {
    "unrealMCP": {
      "command": "uv",
      "args": [
        "--directory",
        "D:/dev/unreal-mcp/Python",
        "run",
        "unreal_mcp_server.py"
      ]
    }
  }
}
```

Cursor を再起動して設定を反映させる。

---

## 使用方法

1. **UE Editor を起動**（プラグインがポート55557でTCPサーバーを自動起動）
2. **Cursor を起動**（MCPが自動的にPythonサーバーを立ち上げて接続）
3. Cursorのチャットから自然言語でUEを操作

---

## トラブルシューティング

### "modules are missing or built with a different engine version"
→ ダイアログで **Yes** をクリックしてリビルド

### "could not be compiled. Try rebuilding from source manually"
→ `Source/` フォルダが存在しない（手順3を実施）

### "Visual Studio C++ 2022 installation not found"
→ VS2022 のバージョンが古い。Visual Studio Installer で最新版（17.10以上）に更新

### "This project does not have any source code"
→ `Source/` フォルダが存在しない（手順3を実施）

---

## 参考

- リポジトリ: https://github.com/chongdashu/unreal-mcp
- ローカルクローン: `D:/dev/unreal-mcp`
- ステータス: **実験的（Experimental）** - 破壊的変更の可能性あり
