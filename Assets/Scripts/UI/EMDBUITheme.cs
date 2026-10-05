using UnityEngine;

[CreateAssetMenu(
    fileName = "EMDBUITheme",
    menuName = "EMDB/UI/Theme"
)]
public class EMDBUITheme : ScriptableObject
{
    [Header("Base Colors")]
    public Color background = new Color32(6, 21, 34, 255);
    public Color panel = new Color32(11, 34, 51, 255);
    public Color secondaryPanel = new Color32(16, 45, 64, 255);

    [Header("Accent Colors")]
    public Color primary = new Color32(0, 140, 255, 255);
    public Color highlight = new Color32(0, 191, 255, 255);

    [Header("Status Colors")]
    public Color success = new Color32(32, 212, 122, 255);
    public Color warning = new Color32(255, 176, 32, 255);
    public Color error = new Color32(255, 64, 85, 255);

    [Header("Text Colors")]
    public Color primaryText = Color.white;
    public Color secondaryText = new Color32(184, 199, 209, 255);
    public Color disabledText = new Color32(96, 116, 130, 255);
}