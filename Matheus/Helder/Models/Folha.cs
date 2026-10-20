public class Folha
{
    public int Id { get; set; }
    public string Nome { get; set; } = String.Empty;
    public string Cpf { get; set; } = String.Empty;
    public int Mes { get; set; }
    public int Ano { get; set; }
    public int HorasTrabalhadas { get; set; }
    public int ValorHora { get; set; }
    public double SalarioBruto { get; set; }
    public double Ir {get; set; }
    public double Inss {get; set; }
    public double Fgts {get; set; }
    public double SalarioLiquido {get; set; }
}