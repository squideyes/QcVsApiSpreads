// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 9/18/2016

using System;

namespace QcVsApiSpreads
{
    public class Tick
    {
        public Symbol Symbol { get; set; }
        public DateTime TickOnEst { get; set; }
        public double BidRate { get; set; }
        public double AskRate { get; set; }

        public double Spread => 
            (AskRate - BidRate).ToRoundedRate(Symbol);
    }
}
