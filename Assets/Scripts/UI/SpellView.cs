using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
internal class SpellView : UIBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI propertiesText;
    
    internal struct Config
    {
        public string SpellName;
        public Dictionary<string, object> SpellProperties;
    }

    private Config _config;

    public void Setup(Config config)
    {
        _config = config;

        SetupName(_config.SpellName);
        SetupProperties(_config.SpellProperties);
    }

    private void SetupName(string spellName) => nameText.text = spellName;
    
    private void SetupProperties(Dictionary<string, object> spellProperties)
    {
        var resultPropertiesText = "Properties:\n";
        foreach (var (key, value) in spellProperties)
        {
            resultPropertiesText += $"* {key}: {value}\n";
        }

        propertiesText.text = resultPropertiesText;
    }
}
}