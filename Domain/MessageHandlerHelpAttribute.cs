﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain;

public class MessageHandlerHelpAttribute : Attribute
{
    public readonly string Name;
    public readonly string Description;

    public MessageHandlerHelpAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}