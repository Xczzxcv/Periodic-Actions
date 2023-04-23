using UnityEngine;

namespace UI
{
internal abstract class FollowUIAbstract : UIBaseBehaviour
{
    [SerializeField] private Transform followObject;
    [SerializeField] private Vector2 offset;
    private Camera _mainCam;
    
    protected override void Awake()
    {
        _mainCam = Camera.main;
    }

    protected void UpdatePosition()
    {
        Vector2 screenPoint = _mainCam.WorldToScreenPoint(followObject.position);
        transform.position = screenPoint + offset;
    }
}
}