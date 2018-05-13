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
        public static bool DoFulltext<T>(ObservableCollection<T> nodes, IEnumerable<FulltextValue> inValues, IEnumerable<FulltextValue> exValues, int level = 0, bool parentVisibility = false) where T: IFulltext
        {
            var ret = parentVisibility;
            if (nodes==null) {
                return ret;
            }
            //Console.WriteLine("------------------------------------------------------------");
            foreach (var n in nodes)
            {
                var nodeName = n.Name.ToUpperInvariant();

                //Console.WriteLine($"nodeName => {nodeName}");
                var shouldBeVisible = ShouldBeVisible(inValues.Where(x => x.Level == level).Concat(exValues.Where(x => x.Level == level)), nodeName, parentVisibility);
                //Console.WriteLine($"shouldBeVisible => {shouldBeVisible}");
                //Console.WriteLine($"DoFullText --> level: {level}");
                var isSomeChildNodeVisible = DoFulltext<IFulltext>(n.Nodes, inValues, exValues, level + 1, shouldBeVisible);
                //Console.WriteLine($"isSomeChildNodeVisible => {isSomeChildNodeVisible}");
                if (isSomeChildNodeVisible)
                {
                    shouldBeVisible = true;
                    ret = true;
                } else
                {
                    //shouldBeVisible = false;
                }
                n.IsVisible = shouldBeVisible;
                //Console.WriteLine($"n.IsVisible => {n.IsVisible}");
            }
            return ret;
        }

        private static bool ShouldBeVisible(IEnumerable<FulltextValue> list, string text, bool parentVisibility)
        {
            var visibility = parentVisibility;

            var isOneConditionOk = false;

            foreach (var v in list)
            {
                if (!string.IsNullOrEmpty(v.Value) && !string.IsNullOrWhiteSpace(v.Value) && v.IsInclude)
                {
                    if (text.Contains(v.Value))
                    {
                        visibility = true;
                        isOneConditionOk = true;
                    } else
                    {
                        visibility = isOneConditionOk;
                    }
                }
            }

            if (visibility)
            {
                foreach (var v in list)
                {
                    if (!string.IsNullOrEmpty(v.Value) && !string.IsNullOrWhiteSpace(v.Value) && !v.IsInclude)
                    {
                        if (text.Contains(v.Value)) visibility = false;
                    }
                }
            }

            return visibility;

            //if (parentVisibility && visibility) return true;
            //if (parentVisibility && !visibility) return false;
            //if (!parentVisibility && visibility) return true;
            //if (!parentVisibility && !visibility) return false;


        }

        public static void CreateIncludesAndExcludes(out string[] includes, out string[] excludes, string inValue, string exValue)
        {
            if (string.IsNullOrEmpty(inValue)) inValue = null;
            if (string.IsNullOrEmpty(exValue)) exValue = null;

            includes = inValue != null ? inValue.ToUpperInvariant().Split(' ') : new string[] { };
            excludes = exValue != null ? exValue.ToUpperInvariant().Split(' ') : new string[] { };
        }

        public static List<List<FulltextValue>> CreateFulltextValueLists(string[] includes, string[] excludes, int level = 0)
        {
            var valueListIncludes = includes.Select((x) => new FulltextValue() { IsInclude = true, Value = string.IsNullOrWhiteSpace(x) || string.IsNullOrEmpty(x) ? null : x, Level = level }).ToList();
            var valueListExcludes = excludes.Select((x) => new FulltextValue() { IsInclude = false, Value = string.IsNullOrWhiteSpace(x) || string.IsNullOrEmpty(x) ? null : x, Level = level }).ToList();
            return new List<List<FulltextValue>>() { valueListIncludes, valueListExcludes };
        }
    }
}
