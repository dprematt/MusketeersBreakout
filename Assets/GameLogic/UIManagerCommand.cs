using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerCommand : MonoBehaviour
{
    public InputField JumpKeyInputField;
    public InputField HUDKeyInputField;
    public InputField SprintKeyInputField;
    public InputField DodgeKeyInputField;
    /*public InputField ShootKeyInputField;
    public InputField DirectionKeyInputField;*/
    public Player playerMovements;


       private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerMovements = player.GetComponent<Player>();
    }

    private void Update()
    {
        ChangeJumpKey();
        ChangeHUDKey();
        ChangeSprintKey();
        ChangeDodgeKey();
    }

    private bool IsKeyCodeInUse(KeyCode key)
    {
        return key == playerMovements.jumpKey ||
               key == playerMovements.sprintKey ||
               key == playerMovements.HUDKey ||
               key == KeyCode.Escape||
               key == KeyCode.L||
               key == KeyCode.P||
               key == playerMovements.dodgeKey;
    }

    public void ChangeJumpKey()
    {

        KeyCode newJumpKey;
        if (!string.IsNullOrWhiteSpace(JumpKeyInputField.text))
        {
            if (System.Enum.TryParse(JumpKeyInputField.text, true, out newJumpKey))
            {
                if (IsKeyCodeInUse(newJumpKey) && newJumpKey != playerMovements.jumpKey)
                {
                    Debug.LogWarning($"La touche '{newJumpKey}' est déjà utilisée pour une autre action.");
                    playerMovements.SetJumpKey(KeyCode.Space);
                }
                else
                {
                    playerMovements.SetJumpKey(newJumpKey);
                }
            }
            else
            {
                Debug.LogWarning($"La valeur '{JumpKeyInputField.text}' n'est pas une touche valide.");
            }
        }
        else
        {
            newJumpKey = KeyCode.Space;
            playerMovements.SetJumpKey(KeyCode.Space);
        }
    }

    public void ChangeHUDKey()
    {
        if (!string.IsNullOrWhiteSpace(HUDKeyInputField.text))
        {
            KeyCode newHUDKey;
            
            if (System.Enum.TryParse(HUDKeyInputField.text, true, out newHUDKey))
            {
                if (IsKeyCodeInUse(newHUDKey) && newHUDKey != playerMovements.HUDKey)
                {
                    Debug.LogWarning($"La touche '{newHUDKey}' est déjà utilisée pour une autre action.");
                    playerMovements.SetHUDKey(KeyCode.U);
                }
                else
                {
                    playerMovements.SetHUDKey(newHUDKey);
                }
            }
            else
            {
                Debug.LogWarning($"La valeur '{HUDKeyInputField.text}' n'est pas une touche valide.");
            }
        }
        else
        {
            playerMovements.SetHUDKey(KeyCode.U);
        }
    }

    public void ChangeSprintKey()
    {
        if (!string.IsNullOrWhiteSpace(SprintKeyInputField.text))
        {
            KeyCode newSprintKey;
            
            if (System.Enum.TryParse(SprintKeyInputField.text, true, out newSprintKey))
            {
                if (IsKeyCodeInUse(newSprintKey) && newSprintKey != playerMovements.sprintKey)
                {
                    Debug.LogWarning($"La touche '{newSprintKey}' est déjà utilisée pour une autre action.");
                    playerMovements.SetSprintKey(KeyCode.LeftShift);
                }
                else
                {
                    playerMovements.SetSprintKey(newSprintKey);
                }
            }
            else
            {
                Debug.LogWarning($"La valeur '{SprintKeyInputField.text}' n'est pas une touche valide.");
            }
        }
        else
        {
            playerMovements.SetSprintKey(KeyCode.LeftShift);
        }
    }

    public void ChangeDodgeKey()
    {
        if (!string.IsNullOrWhiteSpace(DodgeKeyInputField.text))
        {
            KeyCode newDodgeKey;
            
            if (System.Enum.TryParse(DodgeKeyInputField.text, true, out newDodgeKey))
            {
                if (IsKeyCodeInUse(newDodgeKey) && newDodgeKey != playerMovements.dodgeKey)
                {
                    Debug.LogWarning($"La touche '{newDodgeKey}' est déjà utilisée pour une autre action.");
                    playerMovements.SetDodgeKey(KeyCode.F);
                }
                else
                {
                    playerMovements.SetDodgeKey(newDodgeKey);
                }
            }
            else
            {
                Debug.LogWarning($"La valeur '{DodgeKeyInputField.text}' n'est pas une touche valide.");
            }
        }
        else
        {
            playerMovements.SetDodgeKey(KeyCode.F);
        }
    }
    /*public void ChangeShootKey()
    {
       if (!string.IsNullOrWhiteSpace(ShootKeyInputField.text))
        {
            KeyCode newShootKey;
            
            if (System.Enum.TryParse(ShootKeyInputField.text, true, out newShootKey))
            {
                playerMovements.SetShootKey(newShootKey);
            }
            else
            {
                Debug.LogWarning($"La valeur '{ShootKeyInputField.text}' n'est pas une touche valide.");
            }
        }
    }
    public void ChangeDirectionKey()
    {
        if (!string.IsNullOrWhiteSpace(DirectionKeyInputField.text))
        {
            KeyCode newDirectionKey;
            
            if (System.Enum.TryParse(DirectionKeyInputField.text, true, out newDirectionKey))
            {
                playerMovements.SetDirectionKey(newDirectionKey);
            }
            else
            {
                Debug.LogWarning($"La valeur '{DirectionKeyInputField.text}' n'est pas une touche valide.");
            }
        }
    }*/
}