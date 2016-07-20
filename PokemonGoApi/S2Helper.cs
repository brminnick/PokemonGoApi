using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Google.Common.Geometry;

namespace PokemonGoApi
{
    class S2Helper
    {
        public static byte [] GetNearbyCellIds(double longitude, double latitude)
        {
            var cellIds = new List<byte>();
            var cells = _GetNearbyCellIds(longitude, latitude);
            foreach (var cellId in cells.OrderBy(c => c))
            {
                cellIds.AddRange(VarintBitConverter.GetVarintBytes(cellId));
            }
            return cellIds.ToArray();
        }
        private static List<ulong> _GetNearbyCellIds(double longitude, double latitude)
        {
            var nearbyCellIds = new List<S2CellId>();

            var cellId = S2CellId.FromLatLng(S2LatLng.FromDegrees(latitude, longitude)).ParentForLevel(15);

            nearbyCellIds.Add(cellId);
            for (int i = 0; i < 10; i++)
            {
                nearbyCellIds.Add(GetPrevious(cellId, i));
                nearbyCellIds.Add(GetNext(cellId, i));
            }

            return nearbyCellIds.Select(c => c.Id).OrderBy(c => c).ToList();
        }

        private static S2CellId GetPrevious(S2CellId cellId, int depth)
        {
            if (depth < 0)
                return cellId;

            depth--;

            return GetPrevious(cellId.Previous, depth);
        }

        private static S2CellId GetNext(S2CellId cellId, int depth)
        {
            if (depth < 0)
                return cellId;

            depth--;

            return GetNext(cellId.Next, depth);
        }
    }
}
