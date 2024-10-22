using UnityEngine;
using UnityEngine.UI;

public class Error : MonoBehaviour
{

    public Text errorText;

    public void SetErrorText(string errorMessage)
    {

        errorText.text = errorMessage;
        Destroy(gameObject, 10f);
    }
}
