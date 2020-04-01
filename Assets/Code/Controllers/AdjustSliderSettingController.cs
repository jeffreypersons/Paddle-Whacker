using UnityEngine;
using UnityEngine.UI;


[ExecuteAlways]
public class AdjustSliderSettingController : MonoBehaviour
{
    [SerializeField] public string description;
    [SerializeField] public string numberSuffix;
    [SerializeField] public string defaultValue;
    [SerializeField] public string minValue;
    [SerializeField] public string maxValue;

    public TMPro.TextMeshProUGUI label;
    public Slider slider;

    void Awake()
    {
        if (SetSliderValues(defaultValue, minValue, maxValue))
        {
            slider.value = float.Parse(defaultValue);
            UpdateLabel(slider.value);
        }
    }
    void Update()
    {
        SetSliderValues(defaultValue, minValue, maxValue);
    }

    void OnEnable()
    {
        slider.onValueChanged.AddListener(UpdateLabel);
    }
    void OnDisable()
    {
        slider.onValueChanged.RemoveListener(UpdateLabel);
    }
    public void UpdateLabel(float value)
    {
        label.text = $"{description}: {value}{numberSuffix}";
    }

    private bool SetSliderValues(string initial, string min, string max)
    {
        bool isAllInt   = MathUtils.IsAllInteger(initial, min, max);
        bool isAllFloat = MathUtils.IsAllFloat(initial, min, max);

        if (isAllInt || isAllFloat)
        {
            slider.minValue = float.Parse(minValue);
            slider.maxValue = float.Parse(maxValue);
            slider.wholeNumbers = isAllInt;
            return true;
        }
        else
        {
            Debug.LogError($"Expected min <= default <= max as all floats or all ints, " +
                           $"recieved {initial}, {min}, {max}` instead");
            return false;
        }
    }
}
