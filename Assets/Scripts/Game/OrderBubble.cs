using UnityEngine;
using UnityEngine.UI;

public class OrderBubble : MonoBehaviour
{
    [SerializeField] private Image orderIcon;
    [SerializeField] private BeerConfig beerConfig;

    /// <summary>Displays the combined beer+topping sprite in the bubble.</summary>
    public void Setup(NPCOrder order)
    {
        orderIcon.sprite = beerConfig.GetSprite(order.BeerType, order.Topping);
        orderIcon.enabled = orderIcon.sprite != null;
    }
}
