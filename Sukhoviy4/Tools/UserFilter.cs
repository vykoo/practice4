using System;
using System.Collections.Generic;
using System.Linq;

namespace Sukhoviy4.Tools
{
    public static class UserFilter
    {
        public static readonly string[] FilterParams =
                 Array.ConvertAll(typeof(User).GetProperties(), (property) => property.Name);

        public static List<User> FilterByParam(this List<User> users, string property, string query)
        {
            if (Array.IndexOf(FilterParams, property) < 0) return new List<User>();

            query = query.ToLower();
            return (users.Where(p =>
                property != null && (p.GetType().GetProperty(property)?.GetValue(p, null)).ToString().ToLower()
                .Contains(query))).ToList();
        }
    }
}
