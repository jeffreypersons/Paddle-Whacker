using System;
using System.Globalization;
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
        if (UpdateSliderValues(initialValue, minValue, maxValue))
        {
            slider.value = float.Parse(initialValue);
            UpdateLabel(slider.value);
        }
    }

    void Update()
    {
        UpdateSliderValues(initialValue, minValue, maxValue);
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


    private void UpdateLabel(float value)
    {
        label.text = $"{description}: {value}{numberSuffix}";
    }

    private bool UpdateSliderValues(string initial, string min, string max)
    {
        bool isAllInt   = IsAllInteger(initial, min, max);
        bool isAllFloat = IsAllFloat  (initial, min, max);

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

    private static bool IsAllInteger(params string[] values)
    {
        foreach (string value in values)
        {
            if (!int.TryParse(value, out _))
            {
                return false;
            }
        }
        return true;
    }

    private static bool IsAllFloat(params string[] values)
    {
        foreach (string value in values)
        {
            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            {
                return false;
            }
        }
        return true;
    }
}
