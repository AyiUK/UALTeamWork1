using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EscapeButtonTrigger : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject menu;

    private bool SecClick=false;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame&&SecClick==false)
        {
            button.onClick.Invoke();
            SecClick = true;
        }

       
    }
}
