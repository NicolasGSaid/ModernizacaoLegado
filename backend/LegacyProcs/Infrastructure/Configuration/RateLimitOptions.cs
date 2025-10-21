namespace LegacyProcs.Infrastructure.Configuration
{
    /// <summary>
    /// Configurações para Rate Limiting
    /// Implementa limites por método HTTP conforme Global Rules - Seção 9 (Segurança)
    /// </summary>
    public class RateLimitOptions
    {
        /// <summary>
        /// Limite padrão de requisições por janela de tempo
        /// </summary>
        public int DefaultMaxRequests { get; set; } = 100;

        /// <summary>
        /// Limite específico para requisições GET
        /// </summary>
        public int GetMaxRequests { get; set; } = 200;

        /// <summary>
        /// Limite específico para requisições POST
        /// </summary>
        public int PostMaxRequests { get; set; } = 50;

        /// <summary>
        /// Limite específico para requisições PUT
        /// </summary>
        public int PutMaxRequests { get; set; } = 30;

        /// <summary>
        /// Limite específico para requisições DELETE
        /// </summary>
        public int DeleteMaxRequests { get; set; } = 10;

        /// <summary>
        /// Janela de tempo em minutos para contagem de requisições
        /// </summary>
        public int DefaultWindowMinutes { get; set; } = 15;

        /// <summary>
        /// Obtém o limite máximo para um método HTTP específico
        /// </summary>
        /// <param name="method">Método HTTP</param>
        /// <returns>Limite máximo de requisições</returns>
        public int GetLimitForMethod(string method)
        {
            return method.ToUpperInvariant() switch
            {
                "GET" => GetMaxRequests,
                "POST" => PostMaxRequests,
                "PUT" => PutMaxRequests,
                "DELETE" => DeleteMaxRequests,
                _ => DefaultMaxRequests
            };
        }
    }
}
