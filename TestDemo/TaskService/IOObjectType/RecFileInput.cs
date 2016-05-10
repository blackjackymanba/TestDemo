﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GPMGateway.Common.IOObjectType
{
    public class RecFileInput : ObjectType
    {
        [JsonProperty("filename")]
        public string FileName;
    }
}
