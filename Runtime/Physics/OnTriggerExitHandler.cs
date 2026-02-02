using UnityEngine;

namespace Utilities.Physic
{
    public class OnTriggerExitHandler : CollisionHandler<Collider>
    {
        protected virtual void OnTriggerExit(Collider other) => OnCollision.Invoke(other);
    }
}