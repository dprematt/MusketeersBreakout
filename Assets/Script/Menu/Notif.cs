using UnityEngine;
using UnityEngine.UI;

public class Notif : MonoBehaviour
{

    public Text Notif_;

    public void SetNotifText(string errorMessage)
    {

        Notif_.text = errorMessage;
        Destroy(gameObject, 10f);
    }
}
