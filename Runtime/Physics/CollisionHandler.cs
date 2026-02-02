using System;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.Physics
{
    public class CollisionHandler<T> : MonoBehaviour
    {
        public UnityEvent<T> OnCollision;
    }
}