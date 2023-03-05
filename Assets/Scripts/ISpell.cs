public interface ISpell
{
    public string Id { get; }
    public float Duration { get; }
    public void PreCast(SpellCastInfo castInfo);
    public void Cast(SpellCastInfo castInfo);
}