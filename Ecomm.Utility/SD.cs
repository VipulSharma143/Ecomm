using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm.Utility
{
    public static class SD
    {
        public const string SP_CreateCoverType = "Create_CoverType";
        public const string SP_UpdateCoverType = "Update_CoverType";
        public const string SP_DeleteCoverType = "Delete_CoverType";
        public const string SP_GetCoverTypes = "Get_CoverTypes";
        public const string SP_GetCoverType = "Get_CoverType";

        //Roles
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee User";
        public const string Role_Company = "Company User";
        public const string Role_Individual = "Individual User";

        //sesiiion
        public const string SS_SessionCount = "Session Cart Count";
        //
        public static double GetPriceBasedOnQuantity(double quantity,double price,double price50,double price100)
        {
            if (quantity < 50)
                return price;
            else if (quantity < 100)
                return price50;
            else return price100;
           
        }
        //order status
        public const string OrderStatusPending = "Pending";
        public const string OrderStatusApporved = "Approved";
        public const string OrderStatusInProgress = "Processing";
        public const string OrderStatusShipped = "Shipped";
        public const string OrderStatusCancelled = "Cancelled";
        public const string OrderStatusRefunded = "Refunded";
        //payment stattus
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayPayment = "PaymentStatusDelay";
        public const string PaymentStatusRejected = "Rejected";
    }
}
