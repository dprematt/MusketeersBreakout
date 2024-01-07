using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaWheel : MonoBehaviour
{
    private float stamina;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private Image greenWheel;
    [SerializeField] private Image redWheel;
    private bool staminaFullUsed;
    void Start()
    {
        stamina = maxStamina;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.B) && !staminaFullUsed)
        {
            if (stamina > 0)
            {
                stamina -= 30 * Time.deltaTime;
            } else {
                greenWheel.enabled = false;
                staminaFullUsed = true;
            }
            redWheel.fillAmount = (float)(stamina / maxStamina + 0.07f);
        } else {
            if (stamina < maxStamina)
            {
                stamina += 30 * Time.deltaTime;
            } else {
                greenWheel.enabled = true;
                staminaFullUsed = false;
            }
            redWheel.fillAmount = (float)(stamina / maxStamina);
        }
        greenWheel.fillAmount = (float)(stamina / maxStamina);
    }
}
