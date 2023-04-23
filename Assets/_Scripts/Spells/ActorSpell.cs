using System;
using System.Collections.Generic;
using System.Linq;
using Actors;
using Actors.Stats;
using Spells.Aspects;
using UnityEngine;

namespace Spells
{
internal class ActorSpell : ISpell
{
    public string Id => Config.Id;

    public SpellCastTarget CastTarget { get; private set; }
    protected readonly SpellConfig Config;
    private readonly Actor _spellOwner;
    private readonly ISpellAspectsFactory _spellAspectsFactory;
    private readonly List<ISpellAspect> _aspects = new();

    public ActorSpell(SpellConfig config, Actor spellOwner, ISpellAspectsFactory spellAspectsFactory)
    {
        Config = config;
        _spellOwner = spellOwner;
        _spellAspectsFactory = spellAspectsFactory;
    }

    public void Init()
    {
        InitAspects();
        InitCastTarget();
    }

    private void InitAspects()
    {
        foreach (var spellAspectConfig in Config.Aspects)
        {
            var spellAspect = _spellAspectsFactory.Create(spellAspectConfig, Config.Id);
            _aspects.Add(spellAspect);
        }
    }

    private void InitCastTarget()
    {
        CastTarget = GetCastTarget(_aspects);
    }

    public void InitialCast(SpellCastInfo castInfo)
    {
        Debug.Assert(CastTarget == SpellCastTarget.Ally == Actor.IsAllies(castInfo.Caster, castInfo.Target),
            $"Wrong target side for '{Id}': {castInfo}");
        Debug.Assert(CastTarget != SpellCastTarget.NoTarget == (castInfo.Target != null),
            $"Wrong target value for {Id}: {castInfo} != {CastTarget}");

        foreach (var spellAspect in _aspects)
        {
            spellAspect.InitialCast(castInfo);
        }
    }

    public void MainCast(SpellCastInfo castInfo)
    {
        foreach (var spellAspect in _aspects)
        {
            spellAspect.MainCast(castInfo);
        }
    }

    public void PostCast(SpellCastInfo castInfo)
    {
        foreach (var spellAspect in _aspects)
        {
            spellAspect.PostCast(castInfo);
        }
    }

    public override string ToString() => $"Spell ({Config.Id})";

    public double GetDelay()
    {
        var delayReduction = _spellOwner.Stats.GetStat(ActorStat.DelayReduction);
        return Math.Max(0, Config.Delay - delayReduction);
    }

    public Dictionary<string, object> GetProperties()
    {
        var properties = new Dictionary<string, object>
        {
            {Constants.SpellProperties.DELAY, GetDelay()}
        };
        foreach (var spellAspect in _aspects)
        {
            var aspectProperties = spellAspect.GetProperties();
            foreach (var (propId, propValue) in aspectProperties)
            {
                properties.Add(propId, propValue);
            }
        }

        return properties;
    }

    private static SpellCastTarget GetCastTarget(IReadOnlyList<ISpellAspect> aspects)
    {
        var resultCastTarget = aspects.First().CastTarget;
        for (var i = 1; i < aspects.Count; i++)
        {
            var spellAspect = aspects[i];
            var spellAspectTarget = spellAspect.CastTarget;
            if (spellAspectTarget == resultCastTarget)
            {
                continue;
            }

            switch (resultCastTarget)
            {
                case SpellCastTarget.NoTarget:
                    resultCastTarget = spellAspectTarget;
                    break;
                case SpellCastTarget.Ally when spellAspectTarget == SpellCastTarget.Enemy:
                case SpellCastTarget.Enemy when spellAspectTarget == SpellCastTarget.Ally:
                    Debug.LogError($"Spell aspect {spellAspect.GetType()} looks inconsistent");
                    return SpellCastTarget.None;
            }
        }

        return resultCastTarget;
    }

    public void Dispose()
    {
        foreach (var spellAspect in _aspects)
        {
            spellAspect.Dispose();
        }
    }
}
}