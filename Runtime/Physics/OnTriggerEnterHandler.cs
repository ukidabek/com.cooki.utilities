using UnityEngine;

namespace Utilities.Physics
{
    public class OnTriggerEnterHandler : CollisionHandler<Collider>
    {
        protected virtual void OnTriggerEnter(Collider other) => OnCollision.Invoke(other);
    }
}