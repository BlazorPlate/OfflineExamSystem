using System;
using System.Collections.Generic;
using System.Linq;

namespace OfflineExamSystem.Helpers
{
    public static class TreeHelper
    {
        #region Public Methods
        public static IEnumerable<T> Traversal<T>(T root, Func<T, IEnumerable<T>> getChildren)
        {
            return new T[] { root }
                .Concat(getChildren(root)
                    .SelectMany(child => Traversal(child, getChildren)));
        }
        #endregion Public Methods
    }
}