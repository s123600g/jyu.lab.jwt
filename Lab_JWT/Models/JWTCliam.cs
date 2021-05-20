namespace Lab_JWT.Models
{
    public class JWTCliam
    {
        /// <summary>
        /// 聲明資訊-發行者
        /// </summary>
        /// <value></value>
        public string iss { set; get; }

        /// <summary>
        /// 聲明資訊-User內容
        /// </summary>
        /// <value></value>
        public string sub { set; get; }

        /// <summary>
        /// 聲明資訊-接收者
        /// </summary>
        /// <value></value>
        public string aud { set; get; }

        /// <summary>
        /// 聲明資訊-有效期限
        /// </summary>
        /// <value></value>
        public string exp { set; get; }

        /// <summary>
        /// 聲明資訊-起始時間
        /// </summary>
        /// <value></value>
        public string nbf { set; get; }

        /// <summary>
        /// 聲明資訊-發行時間
        /// </summary>
        /// <value></value>
        public string iat { set; get; }

        /// <summary>
        /// 聲明資訊-獨立識別ID
        /// </summary>
        /// <value></value>
        public string jti { set; get; }
    }
}