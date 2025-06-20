using UnityEngine.UIElements;

public sealed class PartPageController
{
    #region Part selector

    Button[] _partButtons = new Button[16];

    Button CreatePartButton(int index)
    {
        var button = new Button() { focusable = false };
        button.AddToClassList(UIHelper.NumButtonClass);
        button.clicked += () => SelectPart(index);
        button.text = (index + 1).ToString();
        return button;
    }

    void SelectPart(int index)
    {
        var data = PatternDataHandler.Data;

        // Part button highlight
        var prev = _partButtons[data.PartSelect - 1];
        var next = _partButtons[index];

        prev.RemoveFromClassList(UIHelper.NumButtonLitClass);
        next.AddToClassList(UIHelper.NumButtonLitClass);

        // Part selection update
        data.PartSelect = index + 1;
    }

    #endregion

    #region Constructor

    public PartPageController(VisualElement root)
    {
        var panel = root.Q<VisualElement>("part-selector");

        // 2 rows
        for (var i = 0; i < 2; i++)
        {
            var row = UIHelper.CreateRowContainer(panel);

            // 8 parts per row
            for (var j = 0; j < 8; j++)
            {
                var index = i * 8 + j;
                _partButtons[index] = CreatePartButton(index);
                row.Add(_partButtons[index]);
            }
        }

        SelectPart(0);
    }

    #endregion
}
