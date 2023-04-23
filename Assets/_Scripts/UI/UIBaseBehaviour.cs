using System;
using UnityEngine.EventSystems;

namespace UI
{
internal abstract class UIBaseBehaviour : UIBehaviour, IDisposable
{
    public virtual void Dispose()
    { }

    protected override void OnDestroy()
    {
        Dispose();
        base.OnDestroy();
    }
}
}