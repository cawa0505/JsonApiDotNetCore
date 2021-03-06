using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JsonApiDotNetCore.Internal;
using JsonApiDotNetCore.Models;

namespace JsonApiDotNetCore.Hooks
{
    /// <summary>
    /// The root node class of the breadth-first-traversal of entity data structures
    /// as performed by the <see cref="ResourceHookExecutor"/>
    /// </summary>
    internal class RootNode<TResource> : INode where TResource : class, IIdentifiable
    {
        private readonly IdentifiableComparer _comparer = new IdentifiableComparer();
        private readonly RelationshipProxy[] _allRelationshipsToNextLayer;
        private HashSet<TResource> _uniqueEntities;
        public Type ResourceType { get; internal set; }
        public IEnumerable UniqueEntities { get { return _uniqueEntities; } }
        public RelationshipProxy[] RelationshipsToNextLayer { get; }

        public Dictionary<Type, Dictionary<RelationshipAttribute, IEnumerable>> LeftsToNextLayerByRelationships()
        {
            return _allRelationshipsToNextLayer
                    .GroupBy(proxy => proxy.RightType)
                    .ToDictionary(gdc => gdc.Key, gdc => gdc.ToDictionary(p => p.Attribute, p => UniqueEntities));
        }

        /// <summary>
        /// The current layer entities grouped by affected relationship to the next layer
        /// </summary>
        public Dictionary<RelationshipAttribute, IEnumerable> LeftsToNextLayer()
        {
            return RelationshipsToNextLayer.ToDictionary(p => p.Attribute, p => UniqueEntities);
        }

        /// <summary>
        /// The root node does not have a parent layer and therefore does not have any relationships to any previous layer
        /// </summary>
        public IRelationshipsFromPreviousLayer RelationshipsFromPreviousLayer { get { return null; } }

        public RootNode(IEnumerable<TResource> uniqueEntities, RelationshipProxy[] poplatedRelationships, RelationshipProxy[] allRelationships)
        {
            ResourceType = typeof(TResource);
            _uniqueEntities = new HashSet<TResource>(uniqueEntities);
            RelationshipsToNextLayer = poplatedRelationships;
            _allRelationshipsToNextLayer = allRelationships;
        }

        /// <summary>
        /// Update the internal list of affected entities. 
        /// </summary>
        /// <param name="updated">Updated.</param>
        public void UpdateUnique(IEnumerable updated)
        {
            var casted = updated.Cast<TResource>().ToList();
            var intersected = _uniqueEntities.Intersect(casted, _comparer).Cast<TResource>();
            _uniqueEntities = new HashSet<TResource>(intersected);
        }

        public void Reassign(IEnumerable source = null)
        {
            var ids = _uniqueEntities.Select(ue => ue.StringId);
            ((List<TResource>)source).RemoveAll(se => !ids.Contains(se.StringId));
        }
    }

}
