using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapEditorUIManager
{
    private readonly BaseView _settingsPanel;
    private readonly ListView _colorsPicker;
    private readonly SeatsSettingsView _seatsPicker;
    private readonly ListView _formsPicker;
    private readonly Sprite[] _forms;
    private readonly List<Color> _textColors;
    private readonly List<Color> _figureColors;
    private Color _textColor;
    private Color _viewColor;
    private Color _seatColor;
    private Sprite _currentForm;
    private EditorSeatView.Data _seatSettings;

    public Color TextColor => _textColor;
    public Color ViewColor => _viewColor;
    public Color SeatColor => _seatColor;
    public Sprite CurrentForm => _currentForm;
    public EditorSeatView.Data SeatSettings => _seatSettings;
    public Sprite[] Forms => _forms;

    public MapEditorUIManager(BaseView settingsPanel, ListView colorsPicker, SeatsSettingsView seatsPicker, ListView formsPicker, Sprite[] forms)
    {
        _settingsPanel = settingsPanel;
        _colorsPicker = colorsPicker;
        _seatsPicker = seatsPicker;
        _formsPicker = formsPicker;
        _forms = forms;
        _textColors = new List<Color>();
        _figureColors = new List<Color>();
    }

    public void InitializeColors()
    {
        _figureColors.AddRange(new[]
        {
            ColorUtility.TryParseHtmlString("#CDA6F8", out Color c1) ? c1 : Color.white,
            ColorUtility.TryParseHtmlString("#A6DEF8", out Color c2) ? c2 : Color.white,
            ColorUtility.TryParseHtmlString("#A6F8C7", out Color c3) ? c3 : Color.white,
            ColorUtility.TryParseHtmlString("#F8A6A7", out Color c4) ? c4 : Color.white,
            ColorUtility.TryParseHtmlString("#F8D4A6", out Color c5) ? c5 : Color.white,
            ColorUtility.TryParseHtmlString("#F8A6DE", out Color c6) ? c6 : Color.white
        });

        _textColors.AddRange(new[]
        {
            ColorUtility.TryParseHtmlString("#8C5AB0", out Color c7) ? c7 : Color.white,
            ColorUtility.TryParseHtmlString("#6198AF", out Color c8) ? c8 : Color.white,
            ColorUtility.TryParseHtmlString("#62B085", out Color c9) ? c9 : Color.white,
            ColorUtility.TryParseHtmlString("#000000", out Color c10) ? c10 : Color.white,
            ColorUtility.TryParseHtmlString("#AF8F5A", out Color c11) ? c11 : Color.white,
            ColorUtility.TryParseHtmlString("#AF5A9A", out Color c12) ? c12 : Color.white
        });

        _textColor = _textColors[0];
        _viewColor = _figureColors[0];
        _seatColor = _figureColors[0];
        _currentForm = _forms.Length > 0 ? _forms[0] : null;
    }

    public void ApplyColor(Color color, EditorView selectedView)
    {
        if (selectedView == null) return;

        selectedView.UpdateColor(color);
        if (selectedView is EditorTextView)
        {
            _textColor = color;
            _colorsPicker.UpdateViewsData(GetTextColorsList());
        }
        else if (selectedView is EditorFigureView)
        {
            _viewColor = color;
            _colorsPicker.UpdateViewsData(GetViewColorsList());
        }
        else if (selectedView is EditorSeatView seatView)
        {
            _seatColor = color;
            _seatSettings = new EditorSeatView.Data(_seatSettings?.numer, _seatSettings.countSeats, _seatSettings.countRow, color);
            _colorsPicker.UpdateViewsData(GetSeatsColorsList());
            UIContainer.InitView(seatView, _seatSettings);
        }
    }

    public void ApplyForm(Sprite form, EditorView selectedView)
    {
        if (selectedView is EditorFigureView figure)
        {
            figure.UpdateForm(form);
            _currentForm = form;
            _formsPicker.UpdateViewsData(GetFormsList());

        }
    }

    public void UpdateSeatSettings(EditorSeatView.Data settings, EditorView selectedView)
    {
        if (selectedView is EditorSeatView seatView)
        {
            _seatSettings = settings;
            UIContainer.InitView(seatView, settings);
        }
    }

    public void OpenTextSettings(bool show)
    {
        if (show)
        {
            _settingsPanel.Show();
            _colorsPicker.Show();
            _formsPicker.Hide();
            _seatsPicker.Hide();
            _colorsPicker.UpdateViewsData(GetTextColorsList());
        }
        else
        {
            _settingsPanel.Hide();
        }
    }

    public void OpenFigureSettings(bool show)
    {
        if (show)
        {
            _settingsPanel.Show();
            _colorsPicker.Show();
            _formsPicker.Show();
            _seatsPicker.Hide();
            _colorsPicker.UpdateViewsData(GetViewColorsList());
            _formsPicker.UpdateViewsData(GetFormsList());
        }
        else
        {
            _settingsPanel.Hide();
        }
    }

    public void OpenSeatsSettings(bool show, EditorSeatView.Data settings)
    {
        if (show)
        {
            _seatSettings = settings;
            _colorsPicker.UpdateViewsData(GetSeatsColorsList());
            UIContainer.InitView(_seatsPicker, _seatSettings);
            _seatsPicker.Show();
            _settingsPanel.Show();
            _colorsPicker.Show();
            _formsPicker.Hide();
        }
        else
        {
            _settingsPanel.Hide();
        }
    }

    public (bool settingsPanel, bool colorsPicker, bool formsPicker, bool seatsPicker) SavePanelStates()
    {
        return (
            _settingsPanel.gameObject.activeSelf,
            _colorsPicker.gameObject.activeSelf,
            _formsPicker.gameObject.activeSelf,
            _seatsPicker.gameObject.activeSelf
        );
    }

    public void RestorePanelStates((bool settingsPanel, bool colorsPicker, bool formsPicker, bool seatsPicker) states)
    {
        _settingsPanel.gameObject.SetActive(states.settingsPanel);
        _colorsPicker.gameObject.SetActive(states.colorsPicker);
        _formsPicker.gameObject.SetActive(states.formsPicker);
        _seatsPicker.gameObject.SetActive(states.seatsPicker);
    }

    public void HideAllPanels()
    {
        _settingsPanel.gameObject.SetActive(false);
        _colorsPicker.gameObject.SetActive(false);
        _formsPicker.gameObject.SetActive(false);
        _seatsPicker.gameObject.SetActive(false);
    }

    private List<ColorButtonView.Data> GetTextColorsList() =>
        _textColors.Select(c => new ColorButtonView.Data(c, c == _textColor)).ToList();

    private List<ColorButtonView.Data> GetViewColorsList() =>
        _figureColors.Select(c => new ColorButtonView.Data(c, c == _viewColor)).ToList();

    private List<ColorButtonView.Data> GetSeatsColorsList() =>
        _figureColors.Select(c => new ColorButtonView.Data(c, c == _seatColor)).ToList();

    private List<FormButtonView.Data> GetFormsList() =>
        _forms.Select(f => new FormButtonView.Data(f, f == _currentForm)).ToList();
}