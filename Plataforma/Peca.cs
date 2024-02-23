namespace Plataforma
{
    internal class Peca
    {
        public Posicao Posicao { get; set; } = null;
        public Cor Cor { get; protected set; }
        public int QuantidadeMovimentos { get; protected set; } = 0;
        public Tabuleiro Tabuleiro { get; protected set; }

        public Peca(Cor cor, Tabuleiro tabuleiro)
        {            
            Cor = cor;
            Tabuleiro = tabuleiro;            
        }

        public void IncrementarQuantidadeMovimentos()
        {
            QuantidadeMovimentos++;
        }
    }
}
