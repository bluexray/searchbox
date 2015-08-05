using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchBox.Models
{
    //商品扩展字段
    public class ProductInfoExt:bma_products
    {
        public string StoreName { set; get; }

        public int Number { set; get; }

        public string CateName { set; get; }

        public string BrandName { set; get; }

        public string ExtInfoStr { set; get; }

    }
}
