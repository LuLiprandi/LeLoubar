using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSFX : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (clickClip != null) audioSource.PlayOneShot(clickClip);
        });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverClip != null) audioSource.PlayOneShot(hoverClip);
    }
}
