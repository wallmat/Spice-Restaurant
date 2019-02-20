using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Utility
{
    public static class SD
    {
        public const string DefaultFoodImage = "default_food.png";
        public const string OrderPlaced = "OrderPlaced.png";
        public const string InKitchen = "InKitchen.png";
        public const string ReadyForPickup = "ReadyForPickup.png";
        public const string Completed = "completed.png";

        public const string ManagerUser = "Manager";
        public const string KitchenUser = "Kitchen";
        public const string FrontDeskUser = "FrontDesk";
        public const string CustomerEndUser = "Customer";

        public const string ssShoppingCartCount = "ssCartCount";
        public const string ssCouponCode = "ssCouponCode";

        public const string OrderStatusSubmitted = "Submitted";
        public const string OrderStatusInProcess = "Being Prepared";
        public const string OrderStatusReady = "Ready for Pickup";
        public const string OrderStatusCompleted = "Completed";
        public const string OrderStatusCancelled = "Cancelled";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusRejected = "Rejected";


        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static double DiscountedPrice(Coupon couponFromDb, double OriginalOrderTotal)
        {
            //null bail out
            if (couponFromDb == null)
                return OriginalOrderTotal;

            //didnt meet reqirements for discount bail out
            if (couponFromDb.MinimumAmount > OriginalOrderTotal)
                return OriginalOrderTotal;

            //$ off
            if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Dollar)
                return Math.Round(OriginalOrderTotal - couponFromDb.Discount, 2);

            //% off
            if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Precent)
                return Math.Round(OriginalOrderTotal - (OriginalOrderTotal * couponFromDb.Discount / 100), 2);

            return OriginalOrderTotal;
        }
    }
}
