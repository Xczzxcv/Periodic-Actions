using System.Collections.Generic;
using JetBrains.Annotations;
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
        [CanBeNull] public string SpellName;
        [CanBeNull] public Dictionary<string, object> SpellProperties;
    }

    private Config _config;

    public void Setup(Config config)
    {
        _config = config;

        SetupName(_config.SpellName);
        SetupProperties(_config.SpellProperties);
    }

    private void SetupName(string spellName) => nameText.text = spellName;
    
    private void SetupProperties([CanBeNull] Dictionary<string, object> spellProperties)
    {
        if (spellProperties == null)
        {
            propertiesText.text = string.Empty;
            return;
        }

        var resultPropertiesText = "Properties:\n";
        foreach (var (key, value) in spellProperties)
        {
            resultPropertiesText += $"* {key}: {value}\n";
        }

        propertiesText.text = resultPropertiesText;
    }
}
}