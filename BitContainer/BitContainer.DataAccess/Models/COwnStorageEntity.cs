﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.DataAccess.Models
{
    public class COwnStorageEntity : IAccessWrapper
    {
        public IStorageEntity Entity { get; }
        
        public Boolean IsShared { get; }

        public COwnStorageEntity(IStorageEntity entity, Boolean isShared)
        {
            Entity = entity;
            IsShared = isShared;
        }
    }
}