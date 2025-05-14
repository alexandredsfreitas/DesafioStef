using System.Globalization;

namespace Questao1
{
    class ContaBancaria {
        public int Numero { get; private set; }
        public string Titular { get; set; }
        public double Saldo { get; private set; }

        private const double TaxaSaque = 3.50;

        public ContaBancaria(int numero, string titular, double depositoInicial) {
            Numero = numero;
            Titular = titular;
            Saldo = depositoInicial;
        }

        public ContaBancaria(int numero, string titular) {
            Numero = numero;
            Titular = titular;
            Saldo = 0.0;
        }

        public void Deposito(double quantia) {
            Saldo += quantia;
        }

        public void Saque(double quantia) {
            Saldo -= quantia + TaxaSaque;
        }

        public override string ToString() {
            return $"Conta {Numero}, Titular: {Titular}, Saldo: $ {Saldo.ToString("F2", CultureInfo.InvariantCulture)}";
        }
    }
}
