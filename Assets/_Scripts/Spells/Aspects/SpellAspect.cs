using System.Collections.Generic;

namespace Spells.Aspects
{
internal abstract class SpellAspect<TConfig> : ISpellAspect
    where TConfig : SpellAspectConfig
{
    public abstract SpellCastTarget CastTarget { get; }
    public string SpellId { get; }

    protected readonly TConfig Config;

    protected SpellAspect(TConfig config, string spellId)
    {
        Config = config;
        SpellId = spellId;
    }

    public abstract void InitialCast(SpellCastInfo castInfo);

    public abstract void MainCast(SpellCastInfo castInfo);

    public abstract void PostCast(SpellCastInfo castInfo);

    public abstract Dictionary<string, object> GetProperties();

    public virtual void Dispose()
    { }
}
}