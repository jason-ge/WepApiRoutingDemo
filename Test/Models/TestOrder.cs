﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiRoutingDemo.IntegrationTests.Models
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestOrderAttribute : Attribute
    {
        public int Order { get; private set; }

        public TestOrderAttribute(int order) => Order = order;
    }
}
