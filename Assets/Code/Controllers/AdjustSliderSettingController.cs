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

    private bool TryReadValues(out float val, out float min, out float max)
    {
        val = 0.0f; min = 0.0f; max = 0.0f;
        return float.TryParse(defaultValue, out val) &&
               float.TryParse(minValue,     out min) &&
               float.TryParse(maxValue,     out max);
    }

    void Update()
    {
        float val, min, max;
        if (TryReadValues(out val, out min, out max) && MathUtils.IsWithinRange(val, min, max))
        {
            slider.value        = val;
            slider.minValue     = min;
            slider.maxValue     = max;
            slider.wholeNumbers = MathUtils.IsAllInteger(defaultValue, minValue, maxValue);
        }
        else
        {
            Debug.LogError($"Expected min <= default <= max as all floats or all ints," +
                           $"recieved {defaultValue}, {minValue}, {maxValue}` instead");
        }
    }
}
