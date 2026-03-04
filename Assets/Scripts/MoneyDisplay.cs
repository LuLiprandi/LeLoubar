using TMPro;
using UnityEngine;

public class MoneyDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private void Start()
    {
        UpdateDisplay(0);
        GameManager.Instance.OnMoneyChanged += UpdateDisplay;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnMoneyChanged -= UpdateDisplay;
    }

    /// <summary>Met à jour l'affichage de l'argent.</summary>
    private void UpdateDisplay(int amount)
    {
        moneyText.text = $"{amount} €";
    }
}
