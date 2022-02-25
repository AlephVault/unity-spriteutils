using System;
using AlephVault.Unity.Support.Generic.Authoring.Types;
using UnityEngine;


namespace AlephVault.Unity.SpriteUtils
{
    namespace Authoring
    {
        namespace Types
        {
            /// <summary>
            ///   <para>
            ///     An identified sprite grid is like a sprite grid but it is also identified
            ///     by a particular key (which must be implemented as immutable) and it is
            ///     also internally cached to avoid a lot of unnecessary loads and disposals
            ///     of a given resource when being used/unused.
            ///   </para>
            /// </summary>
            public class IdentifiedSpriteGrid<T> : SpriteGrid
            {
                // A mapping class which tells sprite grid by key. This holds
                // weak references, instead of strong ones.
                private class InstanceMapping : System.Collections.Generic.Dictionary<T, WeakReference<IdentifiedSpriteGrid<T>>> {}

                // A mapping instance which keeps alive instances by their key.
                private static InstanceMapping instances = new InstanceMapping();
                
                // A mapping class to retrieve the custom reference count for
                // this record. This count is modified when Use/Release methods
                // are invoked. When the counter reaches zero, it is removed
                // from this map and is moved to the ordered set of references
                // being kept alive "for last-second rescue" if they are ever
                // wanted again.
                private class RefCountMapping : System.Collections.Generic.Dictionary<IdentifiedSpriteGrid<T>, uint> {}

                // A ref-count instance which keeps alive their counts by reference.
                private static RefCountMapping refCounts = new RefCountMapping();
                
                // A last-second rescue class, implemented as an ordered set.
                // (this, to get the last element(s) and also in an indexed manner).
                private class LastSecondRescue : OrderedSet<IdentifiedSpriteGrid<T>>
                {
                    // Removes the first elements, keeping a given size.
                    public void ShiftUntil(int size)
                    {
                        while (Count > size) { Shift(); }
                    }
                }

                // A last-second instance which keeps the references that were totally
                // released in calls to Release. They may be used again from this list
                // without being garbage-collected.
                private static LastSecondRescue lastSecondRescue = new LastSecondRescue();

                /// <summary>
                ///   The length of the last-second rescue list. This value can be changed
                ///   later and only takes effect when an instance is totally released.
                /// </summary>
                public static int LastSecondRescueSize = 20;

                /// <summary>
                ///   The key this sprite grid is associated to.
                /// </summary>
                public readonly T Key;
                
                private IdentifiedSpriteGrid(
                    T key, Texture2D texture, uint frameWidth, uint frameHeight, float pixelsPerUnit,
                    Action onFinalized = null
                ) : base(texture, frameWidth, frameHeight, pixelsPerUnit, onFinalized)
                {
                    if (key == null) throw new ArgumentNullException(nameof(key));
                    Key = key;
                    instances[key] = new WeakReference<IdentifiedSpriteGrid<T>>(this);
                }
                
                /// <summary>
                ///   <para>
                ///     Gets or creates an instance of <see cref="IdentifiedSpriteGrid{T}"/>
                ///     for the current key and, if absent, initializes the new instance with
                ///     the parameters returned from the <see cref="ifAbsent"/> function.
                ///   </para>
                ///   <para>
                ///     When the instance is just created, it will be referred by whatever the
                ///     reference is held by. It is advisable to immediately make use of it,
                ///     or the reference, on being lost, will dispose the resources.
                ///   </para>
                /// </summary>
                /// <param name="key">The key to retrieve an instance for</param>
                /// <param name="ifAbsent">A function returning the parameters to use for its creation, if absent</param>
                /// <returns>The instance corresponding to that key</returns>
                public static IdentifiedSpriteGrid<T> Get(T key, Func<Tuple<Texture2D, uint, uint, float>> ifAbsent)
                {
                    if (instances.TryGetValue(key, out WeakReference<IdentifiedSpriteGrid<T>> instanceRef) && 
                        instanceRef.TryGetTarget(out IdentifiedSpriteGrid<T> target))
                    {
                        return target;
                    }
                    // Get the parameters to make a new instance.
                    // Make the instance and return it (it will
                    // automatically add the instance to the map).
                    Tuple<Texture2D, uint, uint, float> parameters = ifAbsent();
                    return new IdentifiedSpriteGrid<T>(
                        key, parameters.Item1, parameters.Item2,
                        parameters.Item3, parameters.Item4
                    );
                }

                /// <summary>
                ///   <para>
                ///     Gets or creates an instance of <see cref="IdentifiedSpriteGrid{T}"/>
                ///     for the current key and, if absent, initializes the new instance with
                ///     the parameters returned from the <see cref="ifAbsent"/> function.
                ///   </para>
                ///   <para>
                ///     When the instance is just created, it will be referred by whatever the
                ///     reference is held by. It is advisable to immediately make use of it,
                ///     or the reference, on being lost, will dispose the resources.
                ///   </para>
                /// </summary>
                /// <param name="key">The key to retrieve an instance for</param>
                /// <param name="ifAbsent">A function returning the parameters to use for its creation, if absent</param>
                /// <returns>The instance corresponding to that key</returns>
                public static IdentifiedSpriteGrid<T> Get(T key, Func<Tuple<Texture2D, uint, uint, float, Action>> ifAbsent)
                {
                    if (instances.TryGetValue(key, out WeakReference<IdentifiedSpriteGrid<T>> instanceRef) && 
                        instanceRef.TryGetTarget(out IdentifiedSpriteGrid<T> target))
                    {
                        return target;
                    }
                    // Get the parameters to make a new instance.
                    // Make the instance and return it (it will
                    // automatically add the instance to the map).
                    Tuple<Texture2D, uint, uint, float, Action> parameters = ifAbsent();
                    return new IdentifiedSpriteGrid<T>(
                        key, parameters.Item1, parameters.Item2,
                        parameters.Item3, parameters.Item4,
                        parameters.Item5
                    );
                }

                /// <summary>
                ///   Invoked when this sprite grid is "used".
                /// </summary>
                protected override void OnUsed()
                {
                    lastSecondRescue.Remove(this);
                    refCounts[this] = refCounts.TryGetValue(this, out uint value) ? value + 1 : 1;
                }

                /// <summary>
                ///   Invoked when this sprite grid is "released".
                /// </summary>
                protected override void OnReleased()
                {
                    if (refCounts.ContainsKey(this))
                    {
                        refCounts[this] -= 1;
                        if (refCounts[this] <= 0)
                        {
                            refCounts.Remove(this);
                            lastSecondRescue.Add(this);
                            lastSecondRescue.ShiftUntil(LastSecondRescueSize);
                        }
                    }
                }
            }
        }
    }
}
