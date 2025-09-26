using System;

namespace CrazyRisk.Core
{
    public class Dado
    {
        protected static Random random = new Random();
        public virtual int Lanzar() => random.Next(1, 7);
    }

    public class DadoAtacante : Dado
    {
        public override int Lanzar() => base.Lanzar();
    }

    public class DadoDefensor : Dado
    {
        public override int Lanzar() => base.Lanzar();
    }
}

