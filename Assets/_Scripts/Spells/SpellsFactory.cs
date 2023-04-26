using System.Collections.Generic;
using System.IO;
using System.Linq;
using Actors;
using Toolbox.Editor.Folders;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Spells
{
internal class SpellsFactory : MonoBehaviour, ISpellsFactory
{
    [SerializeField] private SpellConfig[] spellConfigs;
    [SerializeField, EditorButton(nameof(LinkConfigs))] private FolderData configsFolder;

    private readonly Dictionary<string, SpellConfig> _spellConfigsCollection = new();
    private ISpellAspectsFactory _spellAspectsFactory;

    [Inject]
    private void Construct(ISpellAspectsFactory spellAspectsFactory)
    {
        _spellAspectsFactory = spellAspectsFactory;
    }

    private void Awake()
    {
        foreach (var spellConfig in spellConfigs)
        {
            _spellConfigsCollection.Add(spellConfig.Id, spellConfig);
        }
    }

    public ISpell Create(string spellId, Actor spellOwner)
    {
        var spellConfig = _spellConfigsCollection[spellId];
        var actorSpell = new ActorSpell(spellConfig, spellOwner, _spellAspectsFactory);
        actorSpell.Init();
        return actorSpell;
    }

    private void LinkConfigs()
    {
        var configAssetsFolderPath = configsFolder.Path;
        var configAssetPaths = Directory.GetFiles(configAssetsFolderPath);
        var configs = configAssetPaths
            .Where(configAssetPath => !configAssetPath.EndsWith(".meta"))
            .Select(AssetDatabase.LoadAssetAtPath<SpellConfig>)
            .ToArray();
        spellConfigs = configs;
        EditorUtility.SetDirty(this);
    }
}
}