using UnityEngine;
using UnityEngine.UI;


[ExecuteAlways]
public class SliderSettingController : MonoBehaviour
{
    [SerializeField] private string description  = default;
    [SerializeField] private string numberSuffix = default;
    [SerializeField] private string initialValue = default;
    [SerializeField] private string minValue     = default;
    [SerializeField] private string maxValue     = default;

    [SerializeField] private TMPro.TextMeshProUGUI label = default;
    [SerializeField] private Slider slider = default;

    public float SliderValue    { get { return slider.value;    } }
    public float MinSliderValue { get { return slider.minValue; } }
    public float MaxSliderValue { get { return slider.maxValue; } }

    public string Description   { get { return label.text; } }

    public bool IsSliderValueAWholeNumber { get { return slider.wholeNumbers; } }

    void Awake()
    {
        if (SetSliderValues(initialValue, minValue, maxValue))
        {
            slider.value = float.Parse(initialValue);
            UpdateLabel(slider.value);
        }
    }
    void Update()
    {
        SetSliderValues(initialValue, minValue, maxValue);
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
