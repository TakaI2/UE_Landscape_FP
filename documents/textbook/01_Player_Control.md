# AI Game Dev Textbook: Chapter 1 - Player Control

この章では、AI (Antigravity) を使用して、Unityでプレイヤーキャラクターを操作する機能を実装する方法を学びます。

## 1. 基本的な移動 (Basic Movement)

まずは、プレイヤーを矢印キーやWASDキーで上下左右に移動させる基本的な機能を実装します。

### Antigravityへのプロンプト (Prompt)

AIに依頼する際は、**「何を」「どうやって」「どのような要件で」**作りたいかを明確に伝えます。

> **プロンプト例:**
> "Unityで3Dアクションゲームを作っています。プレイヤーキャラクター(`Player.cs`)を矢印キーまたはWASDキーでXZ平面上（前後左右）に移動させるスクリプトを作成してください。移動速度(`speed`)をInspectorから調整できるように `public` 変数にしてください。"

### 重要なソースコード (Key Source Code)

AIが生成するコードの核となる部分は以下の通りです。`Input.GetAxis` で入力を取得し、`transform.Translate` で移動させます。

```csharp
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f; // 移動速度

    void Update()
    {
        // 入力の取得 (-1.0 から 1.0 の値)
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // 移動ベクトルの作成 (フレームレートに依存しないように Time.deltaTime を掛ける)
        Vector3 movement = new Vector3(moveX, 0, moveY) * speed * Time.deltaTime;

        // プレイヤーの位置を更新
        transform.Translate(movement);
    }
}
```

---

## 2. 移動範囲の制限 (Movement Constraints)

プレイヤーが画面の外に出てしまわないように、移動できる範囲を制限します。

### Antigravityへのプロンプト (Prompt)

既存のコードに対して修正を依頼する場合は、**「何を追加したいか」**を具体的に指示します。

> **プロンプト例:**
> "プレイヤーが画面外に出ないように移動制限を追加したいです。`Mathf.Clamp` を使用して、X座標を -8.5〜8.5、Z座標を -4.5〜4.5 の範囲に収めるように `Update` メソッドを修正してください。"

### 重要なソースコード (Key Source Code)

移動後の位置を計算し、その値を `Mathf.Clamp` で制限してから `transform.position` に適用します。

```csharp
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // 現在の位置に移動量を加えた仮の位置
        Vector3 newPos = transform.position + (new Vector3(moveX, 0, moveY) * speed * Time.deltaTime);

        // 画面範囲内に制限 (Clamp)
        newPos.x = Mathf.Clamp(newPos.x, -8.5f, 8.5f);
        newPos.z = Mathf.Clamp(newPos.z, -4.5f, 4.5f);

        // 制限された位置を適用
        transform.position = newPos;
    }
```

### ポイント (Tips)
*   **Time.deltaTime**: パソコンの性能によってゲームの速度が変わらないようにするために必須です。
*   **Mathf.Clamp**: 値を指定した最小値と最大値の間に収める便利な関数です。
