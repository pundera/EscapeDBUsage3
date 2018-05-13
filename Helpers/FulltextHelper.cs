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
        public static bool DoFulltext<T>(bool includesPriority, ObservableCollection<T> nodes, IEnumerable<FulltextValue> inValues, IEnumerable<FulltextValue> exValues, int level = 0, bool isParentInIncludes = false, bool isParentInExcludes = false) where T: IFulltext
        {
            var result = false;

            if (nodes==null)
            {
                return result;
            }

            if (!includesPriority)
            {
                if (isParentInExcludes) return false;
            }

            foreach (var n in nodes)
            {
                var nodeName = n.Name.ToUpperInvariant();

                //var shouldBeVisible = ShouldBeVisible(inValues.Where(x => x.Level == level).Concat(exValues.Where(x => x.Level == level)), nodeName, parentVisibility);
                var includedAndExcluded = GetIncludedAndExcluded(inValues.Where(x => x.Level == level).Concat(exValues.Where(x => x.Level == level)), nodeName);
                var included = includedAndExcluded.Item1 || isParentInIncludes;
                var excluded = includedAndExcluded.Item2 || isParentInExcludes;

                //n.IsIncluded = included;
                //n.IsExcluded = excluded;

                var isSomeChildNodeVisible = DoFulltext<IFulltext>(includesPriority, n.Nodes, inValues, exValues, level + 1, included, excluded);

                var shouldBeVisible = false;

                if (included || isSomeChildNodeVisible)
                {
                    shouldBeVisible = true;
                    result = true;
                }
                if (includesPriority)
                {
                    if (excluded && !included && !isSomeChildNodeVisible) shouldBeVisible = false;
                }
                else
                {
                    if (excluded && !isSomeChildNodeVisible) shouldBeVisible = false;
                }

                n.IsVisible = shouldBeVisible;
            }

            return result;
        }

        private static Tuple<bool, bool> GetIncludedAndExcluded(IEnumerable<FulltextValue> list, string text)
        {
            var isOneConditionOk = false;
            var included = false; 
            var excluded = false; 

            //if (list.Count() == 0)
            //{
            //    included = true; // default --> visible!!! (node with NO conditions (no includes, no excludes)
            //}

            foreach (var v in list)
            {
                if (!string.IsNullOrEmpty(v.Value) && !string.IsNullOrWhiteSpace(v.Value) && v.IsInclude)
                {
                    if (text.Contains(v.Value))
                    {
                        included = true;
                        isOneConditionOk = true;
                    }
                    else
                    {
                        included = isOneConditionOk;
                    }
                }
            }

            isOneConditionOk = false;
            foreach (var v in list)
            {
                if (!string.IsNullOrEmpty(v.Value) && !string.IsNullOrWhiteSpace(v.Value) && !v.IsInclude)
                {
                    if (text.Contains(v.Value))
                    {
                        excluded = true;
                        isOneConditionOk = true;
                    }
                    else
                    {
                        excluded = isOneConditionOk;
                    }
                }
            }

            return new Tuple<bool, bool>(included, excluded); 
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
