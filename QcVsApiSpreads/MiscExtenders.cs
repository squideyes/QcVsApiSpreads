// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 9/18/2016

using System;

namespace QcVsApiSpreads
{
    public static partial class MiscExtenders
    {
        private const string EST_TZNAME = "Eastern Standard Time";

        private static readonly TimeZoneInfo estTzi;

        static MiscExtenders()
        {
            estTzi = TimeZoneInfo.FindSystemTimeZoneById(EST_TZNAME);
        }

        public static double ToPips(this double value, Symbol symbol) =>
            Math.Round(value * (symbol == Symbol.USDJPY ? 100.0 : 10000.0), 1);

        public static DateTime ToEstFromUtc(this DateTime dateTime)=>
            TimeZoneInfo.ConvertTime(dateTime, estTzi);

        public static double ToRoundedRate(this double value, Symbol symbol) =>
            Math.Round(value, symbol == Symbol.USDJPY ? 3 : 5);
    }
}