using Actors.Ai;
using Spells;
using UnityEngine;

namespace Actors
{
[CreateAssetMenu(menuName = "Configs/Actor Config", fileName = "ActorConfig", order = 0)]
internal class ActorConfig : ScriptableObject
{
    public string Name;
    public float Hp;
    public float Armor;
    public ActorSide Side;
    [SerializeReference, ReferencePicker(TypeGrouping = TypeGrouping.ByFlatName)] 
    public ActorAiBaseConfig AiConfig;
    public SpellConfig[] SpellConfigs;
}
}