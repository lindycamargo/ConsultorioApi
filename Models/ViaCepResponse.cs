namespace ConsultorioApi.Models
{
    public class ViaCepResponse
    {
        public string cep { get; set; }
        public string logradouro { get; set; }
        public string bairro { get; set; }
        public string localidade { get; set; }
        public string uf { get; set; }

        public string Logradouro => logradouro;
        public string Bairro => bairro;
        public string Localidade => localidade;
        public string Uf => uf;
    }
}