﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;

namespace ARKBreedingStats.utils
{

    public class CreatureListSorter
    {
        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumnIndex { set; get; }

        /// <summary>
        /// Specifies the last column to be sorted (is used for sorting when current compare is equal)
        /// </summary>
        private int _lastSortColumnIndex;

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order { set; get; }

        /// <summary>
        /// Gets or sets the order of the last column sorting (for example, 'Ascending' or 'Descending').
        /// </summary>
        private SortOrder _lastOrder;

        /// <summary>
        /// Sort list by given column index. If the columnIndex is -1, use last sorting.
        /// </summary>
        public IEnumerable<Creature> DoSort(IEnumerable<Creature> list, int columnIndex = -1, Species[] orderBySpecies = null)
        {
            if (list == null) return null;

            // Determine if clicked column is already the column that is being sorted.
            if (columnIndex == SortColumnIndex)
            {
                // Reverse the current sort direction for this column.
                Order = Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else if (columnIndex != -1)
            {
                // Set the column number that is to be sorted; default to descending (except the name and owner column).
                _lastSortColumnIndex = SortColumnIndex;
                _lastOrder = Order;
                SortColumnIndex = columnIndex;
                Order = columnIndex > 1 ? SortOrder.Descending : SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            return OrderList(list, orderBySpecies);
        }

        private IEnumerable<Creature> OrderList(IEnumerable<Creature> list, Species[] orderBySpecies = null)
        {
            IOrderedEnumerable<Creature> listOrdered;
            if (orderBySpecies != null)
            {
                var dict = orderBySpecies.Select((s, i) => (s, i)).ToDictionary(s => s.s, s => s.i);
                listOrdered = list.OrderBy(c => dict.TryGetValue(c.Species, out var i) ? i : int.MaxValue);
                if (SortColumnIndex == -1 || SortColumnIndex >= _keySelectors.Length)
                    return listOrdered;
                listOrdered = Order == SortOrder.Ascending
                    ? listOrdered.ThenBy(_keySelectors[SortColumnIndex])
                    : listOrdered.ThenByDescending(_keySelectors[SortColumnIndex]);
            }
            else
            {
                if (SortColumnIndex == -1 || SortColumnIndex >= _keySelectors.Length)
                    return list;
                listOrdered = Order == SortOrder.Ascending
                    ? list.OrderBy(_keySelectors[SortColumnIndex])
                    : list.OrderByDescending(_keySelectors[SortColumnIndex]);
            }

            if (_lastSortColumnIndex == -1 || _lastSortColumnIndex >= _keySelectors.Length)
                return listOrdered;

            // sort by second column that was selected previously
            return _lastOrder == SortOrder.Ascending
                ? listOrdered.ThenBy(_keySelectors[_lastSortColumnIndex])
                : listOrdered.ThenByDescending(_keySelectors[_lastSortColumnIndex]);
        }

        /// <summary>
        /// keySelectors depending on the library columns
        /// </summary>
        private readonly Func<Creature, object>[] _keySelectors = {
            c => c.name,
            c => c.owner,
            c => c.note,
            c => c.server,
            c => c.sex,
            c => c.domesticatedAt,
            c => c.topness,
            c => c.topStatsCount,
            c => c.generation,
            c => c.levelFound,
            c => c.Mutations,
            c => c.growingUntil??c.cooldownUntil,
            c => c.levelsWild[0],
            c => c.levelsWild[1],
            c => c.levelsWild[2],
            c => c.levelsWild[3],
            c => c.levelsWild[4],
            c => c.levelsWild[5],
            c => c.levelsWild[6],
            c => c.levelsWild[7],
            c => c.levelsWild[8],
            c => c.levelsWild[9],
            c => c.levelsWild[10],
            c => c.levelsWild[11],
            c => c.colors[0],
            c => c.colors[1],
            c => c.colors[2],
            c => c.colors[3],
            c => c.colors[4],
            c => c.colors[5],
            c => c.Species?.DescriptiveNameAndMod,
            c => c.Status,
            c => c.tribe,
            c => c.Status
        };
    }
}
