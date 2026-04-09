# Tips & トラブルシューティング

## Quixel Bridge / Fab 関連

### "Asset not available in UAsset format" が出る
**状況:** Quixel Bridge で 3D Plants などをダウンロードしようとするとエラーになる
**原因:** UE5.4以降でQuixel BridgeがFabに統合された際、一部の古いアセットがUAsset形式に未変換のまま
**対処:**
- UE Editor内の **Window → Fab** プラグインで同じアセットを検索する
- [fab.com](https://www.fab.com) のブラウザから検索・ダウンロードする

### Fab からアセットをダウンロードしようとすると "Missing Plugin" が出る
**状況:** Fab で植生アセットをダウンロードしようとするとプラグインが足りないと言われる
**原因:** そのアセットが特定のプラグインを必要としている
**対処:** 表示されたプラグインを Enable にして**エディターを再起動**する（正しい対処）
**例:** `ProceduralVegetationEditor` プラグインが必要な草アセット

---

## Landscape マテリアル / ペイント関連

### UE5.7 で Landscape Layer Blend が Legacy になっている
**状況:** マテリアルで `Landscape Layer Blend` ノードを使うと Legacy 扱いになる
**原因:** UE5.7 から推奨方式が変わった
**推奨方式:** `SetMaterialAttributes` + `BlendMaterialAttributes` + `Landscape Layer Sample` + `Landscape Layer Switch` の組み合わせを使う

### UE5.7 でペイントが効かない / 草が生えない
**状況:** Landscape Paint モードでペイントしても何も変化しない
**原因:** UE5.7 から LayerInfo がデフォルトで Non-Weight-Blend で作成されるようになり、ペイントが機能しない
**対処:** Project Settings で `Target Layer Default Blend Method` を **Weight-Blending** に変更してエディターを再起動する
**補足:** Weight-Blending は Legacy 扱いだが、現状 UE5.7 でペイントを動かすには必要

### UE5.7 で LayerInfo 作成時に Weight-Blend を選べない
**状況:** Paint モードで `+` ボタンを押すと選択ダイアログなしに Non-Weight-Blend で保存される
**原因:** UE5.7 の仕様変更でダイアログが廃止された
**対処:** 作成後に LayerInfo アセットをダブルクリックして開き、**Weight-Blended** にチェックを入れる（または上記の Project Settings を変更する）
