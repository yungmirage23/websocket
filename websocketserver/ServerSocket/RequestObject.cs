using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace websocketserver.ServerSocket
{
    [MessagePackObject]
    public class RequestObject
    {
        [Key("id")]
        public long Id { get; set; }


        [Key("comment")]
        public string Comment { get; set; }

        [Key("img")]
        public byte[] Image { get; set; }

        [Key("list")]
        public List<string> Tags = new List<string>();
    }
}
