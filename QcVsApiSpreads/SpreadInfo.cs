﻿// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 9/18/2016

using System;

namespace QcVsApiSpreads
{
    public class SpreadInfo
    {
        public SpreadInfo(Symbol symbol)
        {
            Symbol = symbol;
        }

        public Symbol Symbol { get;  }

        public int Count { get; set; }
        public double Total { get; set; }

        public double AvgSpread => 
            (Total / Count).ToRoundedRate(Symbol);
    }
}
