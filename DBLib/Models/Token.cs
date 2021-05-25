using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DBLib.Models
{
    public class Token
    {
        /// <summary>
        /// Token ID
        /// </summary>
        /// <value></value>
        public string TokenID { set; get; }

        /// <summary>
        /// 更換狀態鎖定
        /// </summary>
        /// <value></value>
        public bool LockRefresh { set; get; }

        /// <summary>
        /// 建立時間
        /// </summary>
        /// <value></value>
        public DateTime CreateDateTime { set; get; }

        /// <summary>
        /// Token過期時間
        /// </summary>
        /// <value></value>
        public DateTime ExpireDateTime { set; get; }
    }
}