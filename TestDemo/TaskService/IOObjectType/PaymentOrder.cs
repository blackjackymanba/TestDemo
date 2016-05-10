using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPMGateway.Common.IOObjectType
{
    public static class PaymentOrder
    {

        #region
        /*
         0  异常           		  UNKNOWN
         11 未支付         		  UNPAID
         21 已支付未销账   		  PAYING
         22 已支付销账中   		  PAYING
         23 已支付退款中   		  CANCELING
         31 已销账         		  PAID
         32 已销账冲正中          CANCELING
         41 已冲正无需退款        CANCELED
         42 已冲正退款中          CANCELING
         50 已退款未冲正（异常）  UNKNOWN
         51 已退款         		  CANCELED
         */
        #endregion

        public static OrderStatusCode GetStatus(int status)
        {
            switch (status)
            {
                case 0:
                    return OrderStatusCode.UNKNOWN;
                case 50:
                    return OrderStatusCode.UNKNOWN;
                case 11:
                    return OrderStatusCode.UNPAID;
                case 21:
                    return OrderStatusCode.PAYING;
                case 22:
                    return OrderStatusCode.PAYING;
                case 23:
                    return OrderStatusCode.CANCELING;
                case 32:
                    return OrderStatusCode.CANCELING;
                case 42:
                    return OrderStatusCode.CANCELING;
                case 31:
                    return OrderStatusCode.PAID;
                case 41:
                    return OrderStatusCode.CANCELED;
                case 51:
                    return OrderStatusCode.CANCELED;
                default:
                    return OrderStatusCode.UNKNOWN;
            }
        }
    }

    public enum OrderStatusCode
    {
        UNKNOWN,
        UNPAID,
        PAYING,
        CANCELED,
        CANCELING,
        PAID,
        RECONCILIATING,
        RECONCILIATED
    }
}
