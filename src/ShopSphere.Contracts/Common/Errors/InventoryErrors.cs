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

        public static readonly Error InsufficientStock =
            new(
                ErrorCodes.Validation,
                "Insufficient inventory available.");

        public static readonly Error AlreadyExists =
       new(
           "INVENTORY_ALREADY_EXISTS",
           "A Inventory with the same name already exists.",
           "name");
    }
}
