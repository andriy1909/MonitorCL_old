﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using MonitorCLClassLibrary;

namespace MonitorCLClassLibrary.JSON
{
    [DataContract]
    [KnownType(typeof(JSDRegister))]    
    [KnownType(typeof(JSDRegisterS))]
    [KnownType(typeof(JSDLogin))]
    [KnownType(typeof(JSDLoginS))]
    [KnownType(typeof(JSDMonitoring))]
    public abstract class JsonData
    {
        [IgnoreDataMember]
        public abstract EncriptLevel encriptLevel { get; }
        
        public abstract void EncriptData();

        public abstract void DecriptData();
    }
}
