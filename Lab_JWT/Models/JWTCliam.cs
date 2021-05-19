namespace Lab_JWT.Models
{
    public class JWTCliam
    {
        public string iss { set; get; }

        public string sub { set; get; }

        public string aud { set; get; }

        public string exp { set; get; }

        public string nbf { set; get; }

        public string iat { set; get; }

        public string jti { set; get; }
    }
}