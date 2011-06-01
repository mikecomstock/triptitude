﻿using System.Collections.Generic;
using System.Linq;

namespace Triptitude.Biz.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> e)
        {
            return e == null || !e.Any();
        }
    }
}