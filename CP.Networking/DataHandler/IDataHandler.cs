using System;
using System.Collections.Generic;
using System.Text;

namespace CP.Networking.DataHandler
{
    public interface IDataHandler
    {
        void Handle(Client client, byte[] data);

        byte[] Prepare(byte[] data);
    }
}
