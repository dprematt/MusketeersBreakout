using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public void OnClickQuit()
    {
        // Quitte l'application
        Application.Quit();

        // Note : L'application ne se fermera pas dans l'éditeur Unity, seulement lorsqu'elle est compilée.
    }
}
