namespace Lab_JWT.Models
{
    public class Response
    {
        /// <summary>
        /// Token 字串
        /// </summary>
        /// <value></value>
        public string JwtToken { set; get; }

        /// <summary>
        /// 執行狀態
        /// </summary>
        /// <value></value>
        public bool Status { set; get; }

        /// <summary>
        /// 回應訊息
        /// </summary>
        /// <value></value>
        public string Msg { set; get; }
    }
}