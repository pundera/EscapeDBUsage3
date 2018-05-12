using EscapeDBUsage.Classes;
using EscapeDBUsage.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.Helpers
{
    public static class FulltextHelper
    {
        public static bool DoFulltext<T>(ObservableCollection<T> nodes, IEnumerable<FulltextValue> inValues, IEnumerable<FulltextValue> exValues, int level = 0) where T: IFulltext
        {
            var ret = true;
            if (nodes==null) {
                return ret;
            }
            foreach (var n in nodes)
            {
                var nodeName = n.Name.ToUpperInvariant();

                var shouldBeVisible = ShouldBeVisible(inValues.Where(x => x.Level == level).Concat(exValues.Where(x => x.Level == level)), nodeName);

                var isVisible = DoFulltext<IFulltext>(n.Nodes, inValues, exValues, level++);
                shouldBeVisible &= isVisible;
                ret = shouldBeVisible;
                n.IsVisible = shouldBeVisible;
            }
            return ret;
        }

        private static bool ShouldBeVisible(IEnumerable<FulltextValue> list, string text)
        {
            var shouldBeVisible = false;
            if (list.Count() == 0) shouldBeVisible = true;

            foreach (var v in list)
            {
                if (!string.IsNullOrEmpty(v.Value) && v.IsInclude && text.Contains(v.Value)) shouldBeVisible = true;
            }

            if (shouldBeVisible)
            {
                foreach (var v in list)
                {
                    if (!string.IsNullOrEmpty(v.Value) && !v.IsInclude && text.Contains(v.Value)) shouldBeVisible = false;
                }
            }

            return shouldBeVisible;
        }

        public static void CreateIncludesAndExcludes(out string[] includes, out string[] excludes, string inValue, string exValue)
        {
            includes = inValue != null ? inValue.ToUpperInvariant().Split(' ') : new string[] { };
            excludes = exValue != null ? exValue.ToUpperInvariant().Split(' ') : new string[] { };
        }

        public static List<List<FulltextValue>> CreateFulltextValueLists(string[] includes, string[] excludes, int level = 0)
        {
            var valueListIncludes = includes.Select((x) => new FulltextValue() { IsInclude = true, Value = x, Level = level }).ToList();
            var valueListExcludes = excludes.Select((x) => new FulltextValue() { IsInclude = false, Value = x, Level = level }).ToList();
            return new List<List<FulltextValue>>() { valueListIncludes, valueListExcludes };
        }
    }
}
