namespace Authentication.Server
{
    public class JwtConfig
    {
        public string Key { get; set; }

        public string Iss { get; set; }

        public string Aud { get; set; }
    }
}
