# UI Extensions
uGUI用のUIコンポーネントやEventSystemの拡張をまとめたパッケージです。自分用に作成したものですが、自由に使用して構いません。

想定環境：Unity 6 + uGUI + Input System

# UPMからインストールできます
```
https://github.com/eviltwo/UIExtensions.git?path=Assets/UIExtensions
```

# 内容
|UI||
|---|---|
|ValueDriver|<img width="340" height="45" alt="image" src="https://github.com/user-attachments/assets/eb36823d-9d83-4934-ab3e-81fa11699f58" /><br>十字キー入力によってfloat値が増減する、見えないSliderのようなコンポーネントです。OnValueChangedイベントをText, Button, TextFieldなどに繋げれば、キーボード操作とマウス操作の両方で値の増減ができます。|
|InputActionEventTrigger|標準では用意されていない入力イベントを利用できるようになります。L/Rトリガーでページを切り替える際などに便利です。<br>ValueDriverにInputActionを登録して、イベントを受け取るUIにこのコンポーネントアタッチしてください。|

|Event Systems||
|---|---|
|EventRelaySystem|`OnSubmit`や`OnCancel`イベントを選択中のUIの親に伝達します。ウィンドウの内容を確定したり閉じたりする際に便利です。<br>シーンに1つ配置し、パネルなどの親UIは`ICancelRelayHandler`等を実装してください。|
|SelectionRecovery|UIの選択が外れると、直近で選択したUIを再選択します。マウス操作の後にキーボード操作に戻りやすくなります。<br>シーンに1つ配置してください。|

# ご支援いただけると助かります
- [Asset Store](https://assetstore.unity.com/publishers/12117)
- [Steam](https://store.steampowered.com/curator/45066588)
- [GitHub Sponsors](https://github.com/sponsors/eviltwo)
