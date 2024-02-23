namespace Plataforma
{
    internal class Tabuleiro
    {
        private Peca[,] _pecas;
        public int Linhas { get; set; }
        public int Colunas { get; set; }

        public Tabuleiro(int linhas, int colunas)
        {            
            Linhas = linhas;
            Colunas = colunas;

            _pecas = new Peca[Linhas, Colunas];
        }

        public Peca Peca(int linha, int coluna)
        {
            return _pecas[linha, coluna];
        }

        public Peca Peca(Posicao pos)
        {
            return _pecas[pos.Linha, pos.Coluna];
        }

        public void ColocarPeca(Peca p, Posicao pos)
        {
            if (ExistePeca(pos))
                throw new TabuleiroException("Já existe uma peça nessa posição!");

            _pecas[pos.Linha, pos.Coluna] = p;
            p.Posicao = pos;
        }

        public Peca RetirarPeca(Posicao pos)
        {
            Peca peca = Peca(pos);

            if(peca is null)
                return null;

            peca.Posicao = null;
            _pecas[pos.Linha, pos.Coluna] = null;

            return peca;

        }

        public bool ExistePeca(Posicao pos)
        {
            ValidarPosicao(pos);
            return Peca(pos) is not null;
        }

        public bool PosicaoValida(Posicao pos)
        {
            return pos.Linha >= 0 && pos.Linha < Linhas && pos.Coluna >= 0 && pos.Coluna < Colunas;
        }

        public void ValidarPosicao(Posicao pos)
        {
            if (!PosicaoValida(pos))
                throw new TabuleiroException("Posição inválida!");
        }
    }
}
