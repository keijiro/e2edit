using System;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class PatternEditor : MonoBehaviour
{
    #region Pattern data handlers

    PatternDataView _pattern = new();
    MessageReceiver _receiver;
    MessageSender _sender;

    #endregion

    #region UI elements

    VisualElement _uiRoot;
    (Tab step, Tab pattern, Tab motion) _tab;

    #endregion

    #region UI callbacks

    void OnReceiveButton()
      => AsyncUtil.Forget(RequestReceivePattern());

    async Awaitable RequestReceivePattern()
    {
        try
        {
            var count = _receiver.PatternUpdateCount;
            _sender.SendCurrentPatternDataDumpRequest();
            while (count == _receiver.PatternUpdateCount)
                await Awaitable.NextFrameAsync();
            _receiver.PatternBuffer.CopyTo(_pattern.AsBytes);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        RefreshStepPage();
        RefreshMotionPage();
    }

    void OnSendButton()
      => _sender.SendPatternData(_pattern.AsBytes);

    void OnTabChanged(Tab prevTab, Tab newTab)
    {
        if (newTab == _tab.step) RefreshStepPage();
        if (newTab == _tab.motion) RefreshMotionPage();
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _receiver = new MessageReceiver();
        _sender = new MessageSender();

        _uiRoot = GetComponent<UIDocument>().rootVisualElement;
        _uiRoot.dataSource = _pattern;
        _uiRoot.Q<Button>("receive-button").clicked += OnReceiveButton;
        _uiRoot.Q<Button>("send-button").clicked += OnSendButton;
        _uiRoot.Q<TabView>().activeTabChanged += OnTabChanged;

        _tab.pattern = _uiRoot.Q<Tab>("pattern-tab");
        _tab.step = _uiRoot.Q<Tab>("step-tab");
        _tab.motion = _uiRoot.Q<Tab>("motion-tab");

        InitPatternPage();
        InitStepPage();
        InitMotionPage();
    }

    void OnDestroy()
    {
        _sender?.Dispose();
        _receiver?.Dispose();
        (_sender, _receiver) = (null, null);
    }

    #endregion
}
