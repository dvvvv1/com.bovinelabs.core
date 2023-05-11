﻿// <copyright file="ArchetypeChunkExtensions.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if !BL_DISABLE_LINKED_CHUNKS
namespace BovineLabs.Core.Chunks.Iterators
{
    using System;
    using BovineLabs.Core.Assertions;
    using BovineLabs.Core.Extensions;
    using Unity.Burst.CompilerServices;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Entities;

    public static unsafe class ArchetypeChunkExtensions
    {
        public static Entity* GetEntityDataPtrRO(this ArchetypeChunk archetypeChunk, ref VirtualEntityTypeHandle entityHandle)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(entityHandle.m_Safety);
#endif
            var chunk = GetChunk(archetypeChunk, ref entityHandle);

            var archetype = chunk.m_Chunk->Archetype;
            var buffer = chunk.m_Chunk->Buffer;
            var startOffset = archetype->Offsets[0];
            var result = buffer + startOffset;

            return (Entity*)result;
        }

        public static T* GetComponentDataPtrRO<T>(this ArchetypeChunk archetypeChunk, ref VirtualComponentTypeHandle<T> typeHandle)
            where T : unmanaged, IComponentData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(typeHandle.m_Safety);
#endif
            var chunk = GetChunk(archetypeChunk, ref typeHandle);

            // This updates the type handle's cache as a side effect, which will tell us if the archetype has the component or not.
            void* ptr = ChunkDataUtility.GetOptionalComponentDataWithTypeRO(
                chunk.m_Chunk, chunk.m_Chunk->Archetype, 0, typeHandle.TypeIndex, ref typeHandle.LookupCache);

            return (T*)ptr;
        }

        public static T* GetComponentDataPtrRW<T>(this ArchetypeChunk archetypeChunk, ref VirtualComponentTypeHandle<T> typeHandle)
            where T : unmanaged, IComponentData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(typeHandle.m_Safety);
#endif
#if ENABLE_UNITY_COLLECTIONS_CHECKS || UNITY_DOTS_DEBUG
            if (Hint.Unlikely(typeHandle.IsReadOnly))
            {
                throw new InvalidOperationException(
                    "Provided VirtualComponentTypeHandle is read-only; can't get a read/write pointer to component data");
            }
#endif
            var chunk = GetChunk(archetypeChunk, ref typeHandle);

            // This updates the type handle's cache as a side effect, which will tell us if the archetype has the component or not.
            void* ptr = ChunkDataUtility.GetOptionalComponentDataWithTypeRW(
                chunk.m_Chunk, chunk.m_Chunk->Archetype, 0, typeHandle.TypeIndex, typeHandle.GlobalSystemVersion, ref typeHandle.LookupCache);

#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !DISABLE_ENTITIES_JOURNALING
            if (Hint.Unlikely(chunk.m_EntityComponentStore->m_RecordToJournal != 0))
            {
                chunk.JournalAddRecord(
                    EntitiesJournaling.RecordType.GetComponentDataRW,
                    typeHandle.TypeIndex,
                    typeHandle.GlobalSystemVersion,
                    ptr,
                    typeHandle.LookupCache.ComponentSizeOf * chunk.Count);
            }
#endif

            return (T*)ptr;
        }

        public static BufferAccessor<T> GetBufferAccessor<T>(this ArchetypeChunk archetypeChunk, ref VirtualBufferTypeHandle<T> typeHandle)
            where T : unmanaged, IBufferElementData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(typeHandle.m_Safety0);
#endif
            var chunk = GetChunk(archetypeChunk, ref typeHandle);

            var archetype = chunk.m_Chunk->Archetype;
            var typeIndex = typeHandle.TypeIndex;
            if (Hint.Unlikely(typeHandle.LookupCache.Archetype != archetype))
            {
                typeHandle.LookupCache.Update(chunk.m_Chunk->Archetype, typeIndex);
            }

            byte* ptr = typeHandle.IsReadOnly
                ? ChunkDataUtility.GetOptionalComponentDataWithTypeRO(
                    chunk.m_Chunk, archetype, 0, typeIndex, ref typeHandle.LookupCache)
                : ChunkDataUtility.GetOptionalComponentDataWithTypeRW(
                    chunk.m_Chunk, archetype, 0, typeIndex, typeHandle.GlobalSystemVersion, ref typeHandle.LookupCache);

            if (Hint.Unlikely(ptr == null))
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                return new BufferAccessor<T>(null, 0, 0, true, typeHandle.m_Safety0, typeHandle.m_Safety1, 0);
#else
                return new BufferAccessor<T>(null, 0, 0, 0);
#endif
            }

            int typeIndexInArchetype = typeHandle.LookupCache.IndexInArchetype;
            int internalCapacity = archetype->BufferCapacities[typeIndexInArchetype];
            var length = chunk.Count;
            int stride = typeHandle.LookupCache.ComponentSizeOf;

#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !DISABLE_ENTITIES_JOURNALING
            if (Hint.Unlikely(chunk.m_EntityComponentStore->m_RecordToJournal != 0) && !typeHandle.IsReadOnly)
            {
                chunk.JournalAddRecord(EntitiesJournaling.RecordType.GetBufferRW, typeHandle.TypeIndex, typeHandle.GlobalSystemVersion);
            }
#endif

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            return new BufferAccessor<T>(ptr, length, stride, typeHandle.IsReadOnly, typeHandle.m_Safety0, typeHandle.m_Safety1, internalCapacity);
#else
            return new BufferAccessor<T>(ptr, length, stride, internalCapacity);
#endif
        }

        public static BufferAccessor<T> GetBufferAccessorRO<T>(this ArchetypeChunk archetypeChunk, ref VirtualBufferTypeHandle<T> typeHandle)
            where T : unmanaged, IBufferElementData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(typeHandle.m_Safety0);
#endif
            var chunk = GetChunk(archetypeChunk, ref typeHandle);

            var archetype = chunk.m_Chunk->Archetype;
            var typeIndex = typeHandle.TypeIndex;
            if (Hint.Unlikely(typeHandle.LookupCache.Archetype != archetype))
            {
                typeHandle.LookupCache.Update(chunk.m_Chunk->Archetype, typeIndex);
            }

            byte* ptr = ChunkDataUtility.GetOptionalComponentDataWithTypeRO(chunk.m_Chunk, archetype, 0, typeIndex, ref typeHandle.LookupCache);

            if (Hint.Unlikely(ptr == null))
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                return new BufferAccessor<T>(null, 0, 0, true, typeHandle.m_Safety0, typeHandle.m_Safety1, 0);
#else
                return new BufferAccessor<T>(null, 0, 0, 0);
#endif
            }

            int typeIndexInArchetype = typeHandle.LookupCache.IndexInArchetype;
            int internalCapacity = archetype->BufferCapacities[typeIndexInArchetype];
            var length = chunk.Count;
            int stride = typeHandle.LookupCache.ComponentSizeOf;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            return new BufferAccessor<T>(ptr, length, stride, typeHandle.IsReadOnly, typeHandle.m_Safety0, typeHandle.m_Safety1, internalCapacity);
#else
            return new BufferAccessor<T>(ptr, length, stride, internalCapacity);
#endif
        }

        public static BufferAccessor<T> GetBufferAccessorRW<T>(this ArchetypeChunk archetypeChunk, ref VirtualBufferTypeHandle<T> typeHandle)
            where T : unmanaged, IBufferElementData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(typeHandle.m_Safety0);
            Check.Assume(!typeHandle.IsReadOnly);
#endif
            var chunk = GetChunk(archetypeChunk, ref typeHandle);

            var archetype = chunk.m_Chunk->Archetype;
            var typeIndex = typeHandle.TypeIndex;
            if (Hint.Unlikely(typeHandle.LookupCache.Archetype != archetype))
            {
                typeHandle.LookupCache.Update(chunk.m_Chunk->Archetype, typeIndex);
            }

            byte* ptr = ChunkDataUtility.GetOptionalComponentDataWithTypeRW(
                chunk.m_Chunk, archetype, 0, typeIndex, typeHandle.GlobalSystemVersion, ref typeHandle.LookupCache);

            if (Hint.Unlikely(ptr == null))
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                return new BufferAccessor<T>(null, 0, 0, true, typeHandle.m_Safety0, typeHandle.m_Safety1, 0);
#else
                return new BufferAccessor<T>(null, 0, 0, 0);
#endif
            }

            int typeIndexInArchetype = typeHandle.LookupCache.IndexInArchetype;
            int internalCapacity = archetype->BufferCapacities[typeIndexInArchetype];
            var length = chunk.Count;
            int stride = typeHandle.LookupCache.ComponentSizeOf;

#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !DISABLE_ENTITIES_JOURNALING
            if (Hint.Unlikely(chunk.m_EntityComponentStore->m_RecordToJournal != 0))
            {
                chunk.JournalAddRecord(EntitiesJournaling.RecordType.GetBufferRW, typeHandle.TypeIndex, typeHandle.GlobalSystemVersion);
            }
#endif

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            return new BufferAccessor<T>(ptr, length, stride, false, typeHandle.m_Safety0, typeHandle.m_Safety1, internalCapacity);
#else
            return new BufferAccessor<T>(ptr, length, stride, internalCapacity);
#endif
        }

        private static ArchetypeChunk GetChunk(ArchetypeChunk chunk, ref VirtualEntityTypeHandle typeHandle)
        {
            return VirtualChunkDataUtility.GetChunk(ref chunk, 0, typeHandle.ChunkLinksTypeIndex, ref typeHandle.ChunkLinksLookupCache);
        }

        private static ArchetypeChunk GetChunk<T>(ArchetypeChunk chunk, ref VirtualComponentTypeHandle<T> typeHandle)
            where T : unmanaged, IComponentData
        {
            return VirtualChunkDataUtility.GetChunk(ref chunk, typeHandle.GroupIndex, typeHandle.ChunkLinksTypeIndex, ref typeHandle.ChunkLinksLookupCache);
        }

        private static ArchetypeChunk GetChunk<T>(ArchetypeChunk chunk, ref VirtualBufferTypeHandle<T> typeHandle)
            where T : unmanaged, IBufferElementData
        {
            return VirtualChunkDataUtility.GetChunk(ref chunk, typeHandle.GroupIndex, typeHandle.ChunkLinksTypeIndex, ref typeHandle.ChunkLinksLookupCache);
        }
    }
}
#endif