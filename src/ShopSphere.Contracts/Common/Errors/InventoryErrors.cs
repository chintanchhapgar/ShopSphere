using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSphere.Contracts.Common.Errors
{
    public static class InventoryErrors
    {
        public static readonly Error NotFound =
            new(
                "INVENTORY_NOT_FOUND",
                "Inventory was not found.");
    }
}
