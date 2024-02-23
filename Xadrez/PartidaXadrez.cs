using Chess.Xadrez;
using Plataforma;
using System.Xml;

namespace Xadrez
{
    internal class PartidaXadrez
    {
        public int Turno { get; private set; } = 1;
        public Cor JogadorAtual { get; private set; } = Cor.Branca;
        public Tabuleiro Tabuleiro { get; private set; }
        public bool Terminada { get; private set; } = false;
        public bool Xeque { get; private set; } = false;

        private HashSet<Peca> _pecas = new HashSet<Peca>();
        private HashSet<Peca> _pecasCapturadas = new HashSet<Peca>();

        public PartidaXadrez()
        {
            Tabuleiro = new Tabuleiro(8, 8);
            ColocarPecas();
        }

        private Cor Adversaria(Cor cor)
        {
            if (cor == Cor.Branca)
                return Cor.Preta;
            else
                return Cor.Branca;
        }

        private Peca Rei(Cor cor)
        {
            foreach (var x in PecasEmJogo(cor))
                if (x is Rei)
                    return x;

            return null;
        }

        public bool EstaEmXeque(Cor cor)
        {
            Peca r = Rei(cor);

            if (r == null)
                throw new TabuleiroException("Não tem rei da cor " + cor + " no tabuleiro!");

            foreach (var x in PecasEmJogo(Adversaria(cor)))
            {
                bool[,] mat = x.MovimentosPossiveis();
                if (mat[r.Posicao.Linha, r.Posicao.Coluna])
                    return true;
            }

            return false;
        }

        public void ColocarPeca(char coluna, int linha, Peca peca)
        {
            Tabuleiro.ColocarPeca(peca, new PosicaoXadrez(coluna, linha).ToPosicao());
            _pecas.Add(peca);
        }

        private void ColocarPecas()
        {
            ColocarPeca('c', 1, new Torre(Cor.Branca, Tabuleiro));
            ColocarPeca('c', 2, new Torre(Cor.Branca, Tabuleiro));
            ColocarPeca('d', 2, new Torre(Cor.Branca, Tabuleiro));
            ColocarPeca('e', 2, new Torre(Cor.Branca, Tabuleiro));
            ColocarPeca('e', 1, new Torre(Cor.Branca, Tabuleiro));
            ColocarPeca('d', 1, new Rei(Cor.Branca, Tabuleiro));

            ColocarPeca('c', 7, new Torre(Cor.Preta, Tabuleiro));
            ColocarPeca('c', 8, new Torre(Cor.Preta, Tabuleiro));
            ColocarPeca('d', 7, new Torre(Cor.Preta, Tabuleiro));
            ColocarPeca('e', 7, new Torre(Cor.Preta, Tabuleiro));
            ColocarPeca('e', 8, new Torre(Cor.Preta, Tabuleiro));
            ColocarPeca('d', 8, new Rei(Cor.Preta, Tabuleiro));
        }

        public Peca ExecutaMovimento(Posicao origem, Posicao destino)
        {
            Peca peca = Tabuleiro.Peca(origem);
            peca.IncrementarQuantidadeMovimentos();

            Tabuleiro.RetirarPeca(origem);

            Peca pecaCapturada = Tabuleiro.RetirarPeca(destino);
            Tabuleiro.ColocarPeca(peca, destino);

            if (pecaCapturada != null)
                _pecasCapturadas.Add(pecaCapturada);

            return pecaCapturada;
        }

        public HashSet<Peca> PecasCapturadas(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (var x in _pecasCapturadas)
                if (x.Cor == cor)
                    aux.Add(x);

            return aux;
        }

        public HashSet<Peca> PecasEmJogo(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (var x in _pecas)
                if (x.Cor == cor)
                    aux.Add(x);

            aux.ExceptWith(PecasCapturadas(cor));

            return aux;
        }

        public void RealizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = ExecutaMovimento(origem, destino);

            if (EstaEmXeque(JogadorAtual))
            {
                DesfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque!");
            }

            Xeque = EstaEmXeque(Adversaria(JogadorAtual));

            Turno++;
            MudarJogador();
        }

        private void DesfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = Tabuleiro.RetirarPeca(destino);
            p.DecrementarQuantidadeMovimentos();

            if (pecaCapturada != null)
            {
                Tabuleiro.ColocarPeca(pecaCapturada, destino);
                _pecasCapturadas.Remove(pecaCapturada);
            }

            Tabuleiro.ColocarPeca(p, origem);
        }

        private void MudarJogador()
        {
            if (JogadorAtual == Cor.Branca)
                JogadorAtual = Cor.Preta;
            else
                JogadorAtual = Cor.Branca;
        }

        public void ValidarPosicaoOrigem(Posicao pos)
        {
            if (Tabuleiro.Peca(pos) == null)
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");

            if (JogadorAtual != Tabuleiro.Peca(pos).Cor)
                throw new TabuleiroException("A peça de origem escolhida não é sua!");

            if (!Tabuleiro.Peca(pos).ExisteMovimentosPossiveis())
                throw new TabuleiroException("Não há movimentos possíveis para a peça de origem!");
        }

        public void ValidarPosicaoDestino(Posicao origem, Posicao destino)
        {
            if (!Tabuleiro.Peca(origem).PodeMoverPara(destino))
                throw new TabuleiroException("Posição de destino inválida!");
        }
    }
}
