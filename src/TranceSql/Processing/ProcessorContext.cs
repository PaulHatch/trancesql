﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Processing
{
    internal class ProcessorContext
    {
        public IResultProcessor Processer { get; set; }
        public IDeferred Deferred { get; set; }
    }

}
