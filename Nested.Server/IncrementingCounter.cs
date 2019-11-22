﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nested.Server
{
    public class IncrementingCounter
    {
        public void Increment(int amount)
        {
            Count += amount;
        }

        public int Count { get; private set; }
    }
}
