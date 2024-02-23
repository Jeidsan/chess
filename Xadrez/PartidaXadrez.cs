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
        public Peca VulneravelEnPassant { get; private set; }

        public PartidaXadrez()
        {
            Tabuleiro = new Tabuleiro(8, 8);
            ColocarPecas();
            VulneravelEnPassant = null;
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
            ColocarPeca('a', 1, new Torre(Cor.Branca, Tabuleiro));
            ColocarPeca('b', 1, new Cavalo(Cor.Branca, Tabuleiro));
            ColocarPeca('c', 1, new Bispo(Cor.Branca, Tabuleiro));
            ColocarPeca('d', 1, new Dama(Cor.Branca, Tabuleiro));
            ColocarPeca('e', 1, new Rei(Cor.Branca, Tabuleiro, this));
            ColocarPeca('f', 1, new Bispo(Cor.Branca, Tabuleiro));
            ColocarPeca('g', 1, new Cavalo(Cor.Branca, Tabuleiro));
            ColocarPeca('h', 1, new Torre(Cor.Branca, Tabuleiro));
            ColocarPeca('a', 2, new Peao(Cor.Branca, Tabuleiro, this));
            ColocarPeca('b', 2, new Peao(Cor.Branca, Tabuleiro, this));
            ColocarPeca('c', 2, new Peao(Cor.Branca, Tabuleiro, this));
            ColocarPeca('d', 2, new Peao(Cor.Branca, Tabuleiro, this));
            ColocarPeca('e', 2, new Peao(Cor.Branca, Tabuleiro, this));
            ColocarPeca('f', 2, new Peao(Cor.Branca, Tabuleiro, this));
            ColocarPeca('g', 2, new Peao(Cor.Branca, Tabuleiro, this));
            ColocarPeca('h', 2, new Peao(Cor.Branca, Tabuleiro, this));

            ColocarPeca('a', 8, new Torre(Cor.Preta, Tabuleiro));
            ColocarPeca('b', 8, new Cavalo(Cor.Preta, Tabuleiro));
            ColocarPeca('c', 8, new Bispo(Cor.Preta, Tabuleiro));
            ColocarPeca('d', 8, new Dama(Cor.Preta, Tabuleiro));
            ColocarPeca('e', 8, new Rei(Cor.Preta, Tabuleiro, this));
            ColocarPeca('f', 8, new Bispo(Cor.Preta, Tabuleiro));
            ColocarPeca('g', 8, new Cavalo(Cor.Preta, Tabuleiro));
            ColocarPeca('h', 8, new Torre(Cor.Preta, Tabuleiro));
            ColocarPeca('a', 7, new Peao(Cor.Preta, Tabuleiro, this));
            ColocarPeca('b', 7, new Peao(Cor.Preta, Tabuleiro, this));
            ColocarPeca('c', 7, new Peao(Cor.Preta, Tabuleiro, this));
            ColocarPeca('d', 7, new Peao(Cor.Preta, Tabuleiro, this));
            ColocarPeca('e', 7, new Peao(Cor.Preta, Tabuleiro, this));
            ColocarPeca('f', 7, new Peao(Cor.Preta, Tabuleiro, this));
            ColocarPeca('g', 7, new Peao(Cor.Preta, Tabuleiro, this));
            ColocarPeca('h', 7, new Peao(Cor.Preta, Tabuleiro, this));
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

            // #jogadaespecial roque pequeno
            if(peca is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemTorre = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoTorre = new Posicao(origem.Linha, origem.Coluna + 1);

                Peca torre = Tabuleiro.RetirarPeca(origemTorre);
                Tabuleiro.ColocarPeca(torre, destinoTorre);
            }

            // #jogadaespecial roque grande
            if (peca is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemTorre = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoTorre = new Posicao(origem.Linha, origem.Coluna - 1);

                Peca torre = Tabuleiro.RetirarPeca(origemTorre);
                Tabuleiro.ColocarPeca(torre, destinoTorre);
            }

            // #jogadaespecial En Passant
            if (peca is Peao)
            {
                if(origem.Coluna != destino.Coluna && pecaCapturada == null)
                {
                    Posicao posP;
                    if(peca.Cor == Cor.Branca)                    
                        posP = new Posicao(destino.Linha + 1, destino.Coluna);
                    else                    
                        posP = new Posicao(destino.Linha - 1, destino.Coluna);

                    pecaCapturada = Tabuleiro.RetirarPeca(posP);
                    _pecasCapturadas.Add(pecaCapturada);
                }
            }

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

            if (EstaEmXequeMate(Adversaria(JogadorAtual)))
            {
                Terminada = true;
            }
            else
            {
                Turno++;
                MudarJogador();
            }

            Peca p = Tabuleiro.Peca(destino);

            // #jogadaespecial En Passant
            if (p is Peao && (destino.Linha == origem.Linha - 2 || destino.Linha == origem.Linha + 2))
                VulneravelEnPassant = p;
            else
                VulneravelEnPassant = null;
        }

        public bool EstaEmXequeMate(Cor cor)
        {
            if (!EstaEmXeque(cor))
                return false;

            foreach (var x in PecasEmJogo(cor))
            {
                bool[,] mat = x.MovimentosPossiveis();
                for (int i = 0; i < Tabuleiro.Linhas; i++)
                {
                    for (int j = 0; j < Tabuleiro.Colunas; j++)
                    {
                        if (mat[i,j])
                        {
                            Posicao origem = x.Posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = ExecutaMovimento(origem, destino);
                            bool testeXeque = EstaEmXeque(cor);

                            DesfazMovimento(origem, destino, pecaCapturada);

                            if (!testeXeque)
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        private void DesfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca peca = Tabuleiro.RetirarPeca(destino);
            peca.DecrementarQuantidadeMovimentos();

            if (pecaCapturada != null)
            {
                Tabuleiro.ColocarPeca(pecaCapturada, destino);
                _pecasCapturadas.Remove(pecaCapturada);
            }

            Tabuleiro.ColocarPeca(peca, origem);

            // #jogadaespecial roque pequeno
            if (peca is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemTorre = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoTorre = new Posicao(origem.Linha, origem.Coluna + 1);

                Peca torre = Tabuleiro.RetirarPeca(destinoTorre);
                torre.DecrementarQuantidadeMovimentos();
                Tabuleiro.ColocarPeca(torre, origemTorre);
            }

            // #jogadaespecial roque grande
            if (peca is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemTorre = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoTorre = new Posicao(origem.Linha, origem.Coluna - 1);

                Peca torre = Tabuleiro.RetirarPeca(destinoTorre);
                torre.DecrementarQuantidadeMovimentos();
                Tabuleiro.ColocarPeca(torre, origemTorre);
            }

            // #jogadaespecial En Passant
            if (peca is Peao)
            {
                if (origem.Coluna != destino.Coluna && pecaCapturada == VulneravelEnPassant)
                {
                    Peca peao = Tabuleiro.RetirarPeca(destino);
                    Posicao posP;
                    if (peca.Cor == Cor.Branca)
                        posP = new Posicao(3, destino.Coluna);
                    else
                        posP = new Posicao(4, destino.Coluna);

                    Tabuleiro.ColocarPeca(peao, posP);
                }
            }
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
            if (!Tabuleiro.Peca(origem).MovimentoPossivel(destino))
                throw new TabuleiroException("Posição de destino inválida!");
        }
    }
}
