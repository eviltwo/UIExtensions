# UI Extensions
uGUI用のUIコンポーネントやEventSystemの拡張をまとめたパッケージです。

想定環境：Unity 6 + uGUI + Input System

# UPMでインストールできます
```
https://github.com/eviltwo/UIExtensions.git?path=Assets/UIExtensions
```

# 内容
|クラス名|説明|
|---|---|
|EventRelaySystem|`OnSubmit`や`OnCancel`イベントを選択中のUIの親に伝達します。<br>シーンに1個配置し、パネルなどの親UIは`ICancelRelayHandler`等を実装してください。|
|SelectionRecovery|UIの選択が外れると、直近で選択したUIを再選択します。<br>シーンに1個配置してください。|

# ご支援いただけると助かります
- [Asset Store](https://assetstore.unity.com/publishers/12117)
- [Steam](https://store.steampowered.com/curator/45066588)
- [GitHub Sponsors](https://github.com/sponsors/eviltwo)
