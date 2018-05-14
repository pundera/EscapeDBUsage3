using EscapeDBUsage.Classes;
using EscapeDBUsage.Enums;
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
        public static FulltextResult DoFulltext<T>(bool includesPriority, ObservableCollection<T> nodes, IEnumerable<FulltextValue> inValues, IEnumerable<FulltextValue> exValues, int level = 0, bool isParentInIncludes = false, bool isParentInExcludes = false) where T : IFulltext
        {
            var result = FulltextResult.Unvisible;

            if (nodes == null || nodes.Count==0)
            {
                return FulltextResult.Leaf;
            }

            if (!includesPriority)
            {
                if (isParentInExcludes) return FulltextResult.Unvisible;
            }

            var noIncludes = 0;

            var inValuesForCurrentLevel = inValues.Where(x => x.Level == level);
            var exValuesForCurrentLevel = exValues.Where(x => x.Level == level);
            var inAndExValues = inValuesForCurrentLevel.Concat(exValuesForCurrentLevel);

            foreach (var n in nodes)
            {
                var nodeName = n.Name.ToUpperInvariant();

                var includedAndExcluded = GetIncludedAndExcluded(inAndExValues, nodeName);
                var included = includedAndExcluded.Item1;
                var excluded = includedAndExcluded.Item2;

                n.IsIncluded = included;
                n.IsExcluded = excluded;

                var isSomeChildNodeVisible = FulltextResult.Unvisible;

                if (isParentInIncludes)
                {
                    isSomeChildNodeVisible = DoFulltext(includesPriority, n.Nodes, inValues, exValues, level + 1, true, excluded);
                }
                else
                {
                    isSomeChildNodeVisible = DoFulltext(includesPriority, n.Nodes, inValues, exValues, level + 1, included, excluded);
                }

                if (isSomeChildNodeVisible == FulltextResult.Visible) n.IsIncluded = true;
                if (isSomeChildNodeVisible == FulltextResult.Leaf && isParentInIncludes)
                {
                    noIncludes++;
                }
            }

            foreach (var n in nodes)
                if (inValuesForCurrentLevel.Count() == 0)
                {
                    n.IsVisible = true;
                }
                else
                {
                    n.IsVisible = false;
                }

            if (noIncludes == nodes.Count())
            {
                foreach (var n in nodes)
                {
                    n.IsIncluded = true;
                }
            }

            var includedNodes = nodes.Where(n => n.IsIncluded);
            var excludedNodes = nodes.Where(n => n.IsExcluded);

            foreach (var n in includedNodes)
            {
                n.IsVisible = true;
                result = FulltextResult.Visible;
            }

            foreach (var n in excludedNodes)
            {
                if (n.IsIncluded)
                {
                    if (!includesPriority) n.IsVisible = false;
                }
                else
                {
                    n.IsVisible = false;
                }
            }        

            return result;
        }

        private static Tuple<bool, bool> GetIncludedAndExcluded(IEnumerable<FulltextValue> list, string text)
        {
            var isOneConditionOk = false;
            var included = false; 
            var excluded = false;

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
