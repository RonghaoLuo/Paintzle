using UnityEngine;

public class UIButton : MonoBehaviour
{
    public void OnButtonClick()
    {
        AudioManager.Instance.PlayUi("UIButtonClick");
    }
}
