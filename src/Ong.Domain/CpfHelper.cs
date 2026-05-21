namespace Ong.Domain;

public static class CpfHelper
{
    public static bool IsValid(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;

        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (cpf.Length != 11) return false;
        if (cpf.Distinct().Count() == 1) return false;

        int[] mult1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] mult2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

        var sum = mult1.Select((m, i) => m * (cpf[i] - '0')).Sum();
        var remainder = sum % 11;
        var digit1 = remainder < 2 ? 0 : 11 - remainder;

        sum = mult2.Select((m, i) => m * (cpf[i] - '0')).Sum();
        remainder = sum % 11;
        var digit2 = remainder < 2 ? 0 : 11 - remainder;

        return cpf[9] - '0' == digit1 && cpf[10] - '0' == digit2;
    }
}
