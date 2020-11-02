using System;
using System.Collections.Generic;
using System.Text;

namespace OnSale.Common.Responses
{
    public class OrderDetailResponse
    {
        public int Id { get; set; }

        public CowResponse Cow { get; set; }

        public float Quantity { get; set; }

        public string Remarks { get; set; }

        public decimal? Value => (decimal)Quantity * Cow?.Price;
    }

}
