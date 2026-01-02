using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UIPlayerHUD : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI TopBarText;
    [SerializeField] private Image[] paintIcons;
    [SerializeField] private Image selectionOutline;
    [SerializeField] private TextMeshProUGUI interactionPrompt;
    [SerializeField] private UIDurationDown powerUpDurationDown;
    [SerializeField] private TextMeshProUGUI HotbarText;
    [SerializeField] private Image crosshair;
    [SerializeField] private Image rainbowCrosshair;
    [SerializeField] private RectTransform hotBar;

    private TimeSpan formattedTime;

    public void SetCountdown(float timesLeft)
    {
        formattedTime = new TimeSpan(0, 0, (int)timesLeft);
        TopBarText.text = formattedTime.Minutes + ":" + formattedTime.Seconds;
    }

    private void OutlineIcon(int paintIconIndex)
    {
        selectionOutline.transform.position = paintIcons[paintIconIndex].transform.position;
    }

    private void SetPaintIconColour(int iconIndex, Color colour)
    {
        paintIcons[iconIndex].color = colour;
    }

    private void SetInteractionPromptVisibility(bool isVisible)
    {
        interactionPrompt.gameObject.SetActive(isVisible);
    }

    private void StartPowerUpCountdown(float duration)
    {
        powerUpDurationDown.StartCountdown(duration);
    }

    private void SetHotbarText(string text)
    {
        HotbarText.text = text;
    }

    private void SetCrosshairColor(Color color)
    {
        crosshair.color = color;
    }

    private void SetSelectionOutlineColour(Color colour)
    {
        selectionOutline.color = colour;
    }

    private void HandlePowerUpStart()
    {
        hotBar.gameObject.SetActive(false);
        rainbowCrosshair.gameObject.SetActive(true);
    }

    private void HandlePowerUpEnd()
    {
        hotBar.gameObject.SetActive(true);
        rainbowCrosshair.gameObject.SetActive(false);
    }

    private void Awake()
    {
        UIManager.Instance.RegisterPlayerHUD(this);
        gameObject.SetActive(false);

        UIManager.Instance.OnSelectPaint += SetCrosshairColor;
        UIManager.Instance.OnSelectPaint += SetSelectionOutlineColour;
        UIManager.Instance.OnSelectPaintWithIndex += OutlineIcon;
        UIManager.Instance.OnCollectPaint += SetPaintIconColour;
        UIManager.Instance.OnToggleInteractionPrompt += SetInteractionPromptVisibility;
        UIManager.Instance.OnPowerUpCountdown += StartPowerUpCountdown;
        UIManager.Instance.OnUpdateHotbarText += SetHotbarText;
        UIManager.Instance.OnPowerUpStart += HandlePowerUpStart;
        UIManager.Instance.OnPowerUpEnd += HandlePowerUpEnd;
    }

    private void OnDestroy()
    {
        UIManager.Instance.OnSelectPaint -= SetCrosshairColor;
        UIManager.Instance.OnSelectPaint -= SetSelectionOutlineColour;
        UIManager.Instance.OnSelectPaintWithIndex -= OutlineIcon;
        UIManager.Instance.OnCollectPaint -= SetPaintIconColour;
        UIManager.Instance.OnToggleInteractionPrompt -= SetInteractionPromptVisibility;
        UIManager.Instance.OnPowerUpCountdown -= StartPowerUpCountdown;
        UIManager.Instance.OnUpdateHotbarText -= SetHotbarText;
        UIManager.Instance.OnPowerUpStart -= HandlePowerUpStart;
        UIManager.Instance.OnPowerUpEnd -= HandlePowerUpEnd;
    }
}
