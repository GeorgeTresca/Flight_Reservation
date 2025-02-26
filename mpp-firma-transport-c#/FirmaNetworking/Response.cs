using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmaNetworking
{
    [Serializable]
    public class Response
    {
        public ResponseType Type { get; set; }
        public Object Data { get; set; }

        private Response() { }

        public class Builder
        {
            private Response response;

            public Builder()
            {
                response = new Response();
            }

            public Builder SetType(ResponseType type)
            {
                response.Type = type;
                return this;
            }

            public Builder SetData(Object data)
            {
                response.Data = data;
                return this;
            }

            public Response Build()
            {
                return response;
            }
        }


        public override string ToString()
        {
            return "Response{" +
                   "type=" + Type +
                   ", data=" + Data +
                   '}';
        }
    }
}
