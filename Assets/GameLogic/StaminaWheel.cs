using UnityEngine;
using UnityEngine.UI;

public class StaminaWheel : MonoBehaviour
{
    [SerializeField] private Image greenWheel;
    [SerializeField] private Image redWheel;

    private void Start()
    {
        PlayerMovements.OnStaminaChanged += UpdateStaminaDisplay;
    }

    private void OnDestroy()
    {
        PlayerMovements.OnStaminaChanged -= UpdateStaminaDisplay;
    }

    private void UpdateStaminaDisplay(float currentStamina, float maxStamina)
    {
        float fillAmount = currentStamina / maxStamina;
        greenWheel.fillAmount = fillAmount;
        redWheel.fillAmount = fillAmount + 0.07f;
    }
}
