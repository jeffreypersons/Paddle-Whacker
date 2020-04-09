using UnityEngine;
using UnityEngine.UI;


[ExecuteAlways]
public class SliderSettingController : MonoBehaviour
{
    [SerializeField] private string description;
    [SerializeField] private string numberSuffix;
    [SerializeField] private string defaultValue;
    [SerializeField] private string minValue;
    [SerializeField] private string maxValue;

    [SerializeField] private TMPro.TextMeshProUGUI label;
    [SerializeField] private Slider slider;

    public float SliderValue    { get { return slider.value;    } }
    public float MinSliderValue { get { return slider.minValue; } }
    public float MaxSliderValue { get { return slider.maxValue; } }

    public string Description   { get { return label.text; } }

    public bool IsSliderValueAWholeNumber { get { return slider.wholeNumbers; } }

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
        #if UNITY_EDITOR
            UpdateLabel(slider.value);
        #endif
    }

    void OnEnable()
    {
        slider.onValueChanged.AddListener(UpdateLabel);
    }
    void OnDisable()
    {
        slider.onValueChanged.RemoveListener(UpdateLabel);
    }
    void UpdateLabel(float value)
    {
        label.text = $"{description}: {value}{numberSuffix}";
    }

    private bool SetSliderValues(string initial, string min, string max)
    {
        bool isAllInt   = MathUtils.IsAllInteger(initial, min, max);
        bool isAllFloat = MathUtils.IsAllFloat(initial, min, max);

        if (isAllInt || isAllFloat)
        {
            slider.minValue = float.Parse(min);
            slider.maxValue = float.Parse(max);
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
