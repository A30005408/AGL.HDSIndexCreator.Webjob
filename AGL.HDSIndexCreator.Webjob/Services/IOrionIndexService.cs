﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGL.HDSIndexCreator.Webjob.Services
{
    public interface IOrionIndexService
    {
        void CreateIndex<T>();
    }
}
