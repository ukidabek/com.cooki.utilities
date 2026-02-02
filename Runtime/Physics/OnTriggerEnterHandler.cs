using UnityEngine;

namespace Utilities.Physic
{
    public class OnTriggerEnterHandler : CollisionHandler<Collider>
    {
        protected virtual void OnTriggerEnter(Collider other) => OnCollision.Invoke(other);
    }
}