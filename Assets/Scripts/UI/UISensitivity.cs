//using UnityEngine;
//using UnityEngine.UI;

//public class UISensitivity : MonoBehaviour
//{
//    [SerializeField] private Slider slider;

//    private void Start()
//    {
//        slider.value = GameManager.Instance.MouseSensitivity;
//        slider.onValueChanged.AddListener(OnChange);
//    }

//    private void OnDestroy()
//    {
//        slider.onValueChanged.RemoveListener(OnChange);
//    }

//    private void OnChange(float val)
//    {
//        GameManager.Instance.SetSettingValues(val);
//    }
//}
