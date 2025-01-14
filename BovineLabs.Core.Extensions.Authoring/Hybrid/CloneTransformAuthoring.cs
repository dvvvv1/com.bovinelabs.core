﻿// <copyright file="CloneTransformAuthoring.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if !BL_DISABLE_COPY_TRANSFORM
namespace BovineLabs.Core.Authoring.Hybrid
{
    using BovineLabs.Core.Hybrid;
    using Unity.Entities;
    using UnityEngine;

    public class CloneTransformAuthoring : MonoBehaviour
    {
        public GameObject? Target;
    }

    public class CloneTransformBaker : Baker<CloneTransformAuthoring>
    {
        public override void Bake(CloneTransformAuthoring authoring)
        {
            var entity = this.GetEntity(TransformUsageFlags.Dynamic);

            this.AddComponent(entity, new CloneTransform
            {
                Value = this.GetEntity(authoring.Target, TransformUsageFlags.Dynamic),
            });
        }
    }
}
#endif
