using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Common.Utils.Hierarchy
{
    public delegate void VisitHierarchyAction<T>(T parent, T node);

    public static class HierarchyExtensions
    {
        public static bool IsRoot<T>(this IHierarchy<T> node) 
            where T : class, IHierarchy<T>
        {
            return node.Parent == null;
        }

        public static bool HasChildren<T>(this IHierarchy<T> node) 
            where T : class, IHierarchy<T>
        {
            return node.Children != null && node.Children.Any();
        }

        public static IEnumerable<IHierarchy<T>> FindAll<T>(this IHierarchy<T> node, Func<IHierarchy<T>, bool> predicate) 
            where T : class, IHierarchy<T>
        {
            if (predicate(node))
            {
                yield return node;
            }

            foreach (var found in node.Children.Cast<IHierarchy<T>>().FindAll(predicate))
            {
                yield return found;
            }
        }

        public static IEnumerable<IHierarchy<T>> FindAll<T>(this IEnumerable<IHierarchy<T>> nodes, Func<IHierarchy<T>, bool> predicate)
            where T : class, IHierarchy<T>
        {
            return nodes.SelectMany(node => node.FindAll(predicate));
        }

        public static IEnumerable<T> FlattenHierarchy<T>(this T hierarchy) 
            where T : class, IHierarchy<T>
        {
            return new [] {hierarchy}
                .Concat(hierarchy.Children != null
                            ? hierarchy.Children.SelectMany(FlattenHierarchy)
                            : Enumerable.Empty<T>());
        }

        public static IEnumerable<T> FlattenHierarchy<T>(this IEnumerable<T> hierarchies) 
            where T : class, IHierarchy<T>
        {
            return hierarchies.SelectMany(h => h.FlattenHierarchy());
        }

        public static void VisitHierarchy<T>(this IEnumerable<T> hierarchies, VisitHierarchyAction<T> action) 
            where T : class, IHierarchy<T>
        {
            foreach (var node in hierarchies.ToList())
            {
                node.VisitHierarchy(action);
            }
        }

        public static void VisitHierarchy<T>(this T hierarchy, VisitHierarchyAction<T> action) 
            where T : class, IHierarchy<T>
        {
            VisitHierarchyCore(hierarchy, default(T), action);
        }

        private static void VisitHierarchyCore<T>(T node, T parent, VisitHierarchyAction<T> action) 
            where T : class, IHierarchy<T>
        {
            if (node.Children != null)
            {
                foreach (var child in node.Children.ToList())
                {
                    VisitHierarchyCore(child, node, action);
                }
            }
            action(parent, node);
        }
    }
}