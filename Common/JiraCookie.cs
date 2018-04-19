using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class JiraCookie
    {
        public JiraCookie(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get;  }
    }
}
