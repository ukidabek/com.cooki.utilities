using System;
using UnityEngine;

namespace Utilities.Physic
{
    public class OnCollisionStayHandler : CollisionHandler<Collision>
    {
        protected virtual void OnCollisionStay(Collision other) => OnCollision.Invoke(other);
    }
}