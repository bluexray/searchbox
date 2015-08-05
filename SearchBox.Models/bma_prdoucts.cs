using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchBox.Models
{
        /// <summary>
        /// 商品表
        /// </summary>
        [Serializable]
        public partial class bma_products
        {
            public bma_products()
            { }
            #region Model
            private int _pid;
            private string _psn = "";
            private int _cateid = 0;
            private int _brandid = 0;
            private int _storeid = 0;
            private int _storecid = 0;
            private int _storestid = 0;
            private int _skugid = 0;
            private string _name = "";
            private decimal _shopprice = 0.00M;
            private decimal _marketprice = 0.00M;
            private decimal _costprice = 0.00M;
            private int _state = 0;
            private int _isbest = 0;
            private int _ishot = 0;
            private int _isnew = 0;
            private int _displayorder = 0;
            private int _weight = 0;
            private string _showimg = "";
            private int _salecount = 0;
            private int _visitcount = 0;
            private int _reviewcount = 0;
            private int _star1 = 0;
            private int _star2 = 0;
            private int _star3 = 0;
            private int _star4 = 0;
            private int _star5 = 0;
            private DateTime _addtime = DateTime.Now;
            private string _description = "";
            private decimal? _cashbackpoint;
            private string _images;
            private decimal? _commissionpoint;
            private string _video;
            private string _product360;
            private int? _isstorehot;
            private string _subhead;
            private string _tag;
            private DateTime? _lastupdatetime;
            /// <summary>
            /// 商品id
            /// </summary>
            public int pid
            {
                set { _pid = value; }
                get { return _pid; }
            }
            /// <summary>
            /// 商品编号
            /// </summary>
            public string psn
            {
                set { _psn = value; }
                get { return _psn; }
            }
            /// <summary>
            /// 分类id
            /// </summary>
            public int cateid
            {
                set { _cateid = value; }
                get { return _cateid; }
            }
            /// <summary>
            /// 品牌id
            /// </summary>
            public int brandid
            {
                set { _brandid = value; }
                get { return _brandid; }
            }
            /// <summary>
            /// 店铺id
            /// </summary>
            public int storeid
            {
                set { _storeid = value; }
                get { return _storeid; }
            }
            /// <summary>
            /// 店铺分类id
            /// </summary>
            public int storecid
            {
                set { _storecid = value; }
                get { return _storecid; }
            }
            /// <summary>
            /// 店铺配送模板id
            /// </summary>
            public int storestid
            {
                set { _storestid = value; }
                get { return _storestid; }
            }
            /// <summary>
            /// sku组id
            /// </summary>
            public int skugid
            {
                set { _skugid = value; }
                get { return _skugid; }
            }
            /// <summary>
            /// 名称
            /// </summary>
            public string name
            {
                set { _name = value; }
                get { return _name; }
            }
            /// <summary>
            /// 商城价
            /// </summary>
            public decimal shopprice
            {
                set { _shopprice = value; }
                get { return _shopprice; }
            }
            /// <summary>
            /// 市场价
            /// </summary>
            public decimal marketprice
            {
                set { _marketprice = value; }
                get { return _marketprice; }
            }
            /// <summary>
            /// 成本价
            /// </summary>
            public decimal costprice
            {
                set { _costprice = value; }
                get { return _costprice; }
            }
            /// <summary>
            /// 状态
            /// </summary>
            public int state
            {
                set { _state = value; }
                get { return _state; }
            }
            /// <summary>
            /// 是否为精品
            /// </summary>
            public int isbest
            {
                set { _isbest = value; }
                get { return _isbest; }
            }
            /// <summary>
            /// 是否为热销
            /// </summary>
            public int ishot
            {
                set { _ishot = value; }
                get { return _ishot; }
            }
            /// <summary>
            /// 是否为新品
            /// </summary>
            public int isnew
            {
                set { _isnew = value; }
                get { return _isnew; }
            }
            /// <summary>
            /// 排序
            /// </summary>
            public int displayorder
            {
                set { _displayorder = value; }
                get { return _displayorder; }
            }
            /// <summary>
            /// 重量(单位为克)
            /// </summary>
            public int weight
            {
                set { _weight = value; }
                get { return _weight; }
            }
            /// <summary>
            /// 主图
            /// </summary>
            public string showimg
            {
                set { _showimg = value; }
                get { return _showimg; }
            }
            /// <summary>
            /// 销售数量
            /// </summary>
            public int salecount
            {
                set { _salecount = value; }
                get { return _salecount; }
            }
            /// <summary>
            /// 访问数量
            /// </summary>
            public int visitcount
            {
                set { _visitcount = value; }
                get { return _visitcount; }
            }
            /// <summary>
            /// 评价数量
            /// </summary>
            public int reviewcount
            {
                set { _reviewcount = value; }
                get { return _reviewcount; }
            }
            /// <summary>
            /// 星星1
            /// </summary>
            public int star1
            {
                set { _star1 = value; }
                get { return _star1; }
            }
            /// <summary>
            /// 星星2
            /// </summary>
            public int star2
            {
                set { _star2 = value; }
                get { return _star2; }
            }
            /// <summary>
            /// 星星3
            /// </summary>
            public int star3
            {
                set { _star3 = value; }
                get { return _star3; }
            }
            /// <summary>
            /// 星星4
            /// </summary>
            public int star4
            {
                set { _star4 = value; }
                get { return _star4; }
            }
            /// <summary>
            /// 星星5
            /// </summary>
            public int star5
            {
                set { _star5 = value; }
                get { return _star5; }
            }
            /// <summary>
            /// 添加时间
            /// </summary>
            public DateTime addtime
            {
                set { _addtime = value; }
                get { return _addtime; }
            }
            /// <summary>
            /// 描述
            /// </summary>
            public string description
            {
                set { _description = value; }
                get { return _description; }
            }
            /// <summary>
            /// 
            /// </summary>
            public decimal? CashBackPoint
            {
                set { _cashbackpoint = value; }
                get { return _cashbackpoint; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string Images
            {
                set { _images = value; }
                get { return _images; }
            }
            /// <summary>
            /// 
            /// </summary>
            public decimal? CommissionPoint
            {
                set { _commissionpoint = value; }
                get { return _commissionpoint; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string Video
            {
                set { _video = value; }
                get { return _video; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string Product360
            {
                set { _product360 = value; }
                get { return _product360; }
            }
            /// <summary>
            /// 
            /// </summary>
            public int? isStoreHot
            {
                set { _isstorehot = value; }
                get { return _isstorehot; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string Subhead
            {
                set { _subhead = value; }
                get { return _subhead; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string Tag
            {
                set { _tag = value; }
                get { return _tag; }
            }
            /// <summary>
            /// 
            /// </summary>
            public DateTime? LastUpdateTime
            {
                set { _lastupdatetime = value; }
                get { return _lastupdatetime; }
            }
            #endregion Model

        }
}
