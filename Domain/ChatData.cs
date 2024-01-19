using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain;

public class ChatData
{
    public long Id { get; init; }

    public TimeSpan UtcOffset { get; set; }

    public ChatData(long id, TimeSpan utcOffset=default)
    {
        Id = id;
        UtcOffset = utcOffset;
    }
}
