using System;
using System.Collections.Generic;
using System.Text;

namespace sizingservers.beholderv2.agent.shared {
    public interface IPayloadRetriever {
        IEnumerable<ComponentGroup> Retrieve();        
    }
}
