﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public class PaybackInput : ObjectType
    {
        [JsonProperty("acct_no")]
        public string AccountNo;

        [JsonProperty("rcv_org_no")]
        public string ChargeOrganizationNo;

        [JsonProperty("rcv_amt_no")]
        public int Amount;

        [JsonProperty("acct_date")]
        public string AccountDate;

        [JsonProperty("old_serial_no")]
        public string SerialNo;

    }
}
