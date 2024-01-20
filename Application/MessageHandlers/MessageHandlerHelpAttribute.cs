using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application;

public class MessageHandlerHelpAttribute : Attribute
{
    public readonly string Name;
    public readonly string Description;
    public readonly string GetDetailedHelpMethodName;

    public MessageHandlerHelpAttribute(string name, string description, string getDetailedHelpMethodName)
    {
        Name = name;
        Description = description;
        GetDetailedHelpMethodName = getDetailedHelpMethodName;
    }
}
